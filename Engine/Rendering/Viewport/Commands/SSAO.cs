using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class SSAOPassVPRC : ViewportRenderCommand
    {
        public QuadFrameBuffer SSAOFBO { get; private set; }
        public QuadFrameBuffer SSAOBlurFBO { get; private set; }
        public FrameBuffer GBufferFBO { get; private set; }

        private readonly SSAOGenerator _ssaoInfo = new SSAOGenerator();

        private ICamera _renderingCamera;
        private Vec2 _noiseScale;
        private IUniformable3Float[] _noiseSamples;

        public override void DestroyFBOs()
        {
            SSAOBlurFBO?.Dispose();
            SSAOBlurFBO = null;
            SSAOFBO?.Dispose();
            SSAOFBO = null;
            GBufferFBO?.Dispose();
            GBufferFBO = null;
        }
        public override unsafe void GenerateFBOs(Viewport viewport)
        {
            TexRef2D normalTex = viewport.Pipeline.GetTexture<TexRef2D>("Normal");
            TexRefView2D depthViewTex = viewport.Pipeline.GetTexture<TexRefView2D>("DepthView");
            TexRef2D albedoTex = viewport.Pipeline.GetTexture<TexRef2D>("AlbedoOpacity");
            TexRef2D rmsiTex = viewport.Pipeline.GetTexture<TexRef2D>("RMSI");
            TexRef2D depthStencilTex = viewport.Pipeline.GetTexture<TexRef2D>("DepthStencil");

            int width = viewport.InternalResolution.Width;
            int height = viewport.InternalResolution.Height;

            _ssaoInfo.Generate();

            _noiseScale = viewport.InternalResolution.Extents / 4.0f;
            _noiseSamples = _ssaoInfo.Kernel.Select(x => (IUniformable3Float)x).ToArray();

            RenderingParameters renderParams = new RenderingParameters()
            {
                DepthTest =
                {
                    Enabled = ERenderParamUsage.Unchanged,
                    UpdateDepth = false,
                    Function = EComparison.Always,
                }
            };

            TexRef2D noiseTex = new TexRef2D(
                "SSAONoise",
                _ssaoInfo.NoiseWidth,
                _ssaoInfo.NoiseHeight,
                EPixelInternalFormat.Rg32f, 
                EPixelFormat.Rg, 
                EPixelType.Float,
                PixelFormat.Format64bppArgb)
            {
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.Repeat,
                VWrap = ETexWrapMode.Repeat,
                Resizable = false,
            };
            Bitmap bmp = noiseTex.Mipmaps[0].File.Bitmaps[0];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Vec2* values = (Vec2*)data.Scan0;
            Vec2[] noise = _ssaoInfo.Noise;
            foreach (Vec2 v in noise)
                *values++ = v;
            bmp.UnlockBits(data);

            viewport.Pipeline.SetTexture(noiseTex);

            TexRef2D ssaoTex = TexRef2D.CreateFrameBufferTexture(
                "SSAO",
                width, 
                height,
               EPixelInternalFormat.R16f,
               EPixelFormat.Red,
               EPixelType.HalfFloat,
               EFramebufferAttachment.ColorAttachment0);

            ssaoTex.MinFilter = ETexMinFilter.Nearest;
            ssaoTex.MagFilter = ETexMagFilter.Nearest;

            viewport.Pipeline.SetTexture(ssaoTex);

            GLSLScript ssaoShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "SSAOGen.fs"), EGLSLType.Fragment);
            GLSLScript ssaoBlurShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "SSAOBlur.fs"), EGLSLType.Fragment);

            TexRef2D[] ssaoRefs = new TexRef2D[]
            {
                normalTex,
                noiseTex,
                depthViewTex,
            };
            TexRef2D[] ssaoBlurRefs = new TexRef2D[]
            {
                ssaoTex
            };

            TMaterial ssaoMat = new TMaterial("SSAOMat", renderParams, ssaoRefs, ssaoShader);
            TMaterial ssaoBlurMat = new TMaterial("SSAOBlurMat", renderParams, ssaoBlurRefs, ssaoBlurShader);

            SSAOFBO = new QuadFrameBuffer(ssaoMat);
            SSAOFBO.SettingUniforms += SSAO_SetUniforms;
            SSAOFBO.SetRenderTargets(
                (albedoTex, EFramebufferAttachment.ColorAttachment0, 0, -1),
                (normalTex, EFramebufferAttachment.ColorAttachment1, 0, -1),
                (rmsiTex, EFramebufferAttachment.ColorAttachment2, 0, -1),
                (depthStencilTex, EFramebufferAttachment.DepthStencilAttachment, 0, -1));

            viewport.Pipeline.SetFBO("SSAO", SSAOFBO);

            SSAOBlurFBO = new QuadFrameBuffer(ssaoBlurMat);
            viewport.Pipeline.SetFBO("SSAOBlur", SSAOBlurFBO);

            GBufferFBO = new FrameBuffer();
            GBufferFBO.SetRenderTargets((ssaoTex, EFramebufferAttachment.ColorAttachment0, 0, -1));
            viewport.Pipeline.SetFBO("GBuffer", GBufferFBO);
        }

        private void SSAO_SetUniforms(RenderProgram program)
        {
            if (_renderingCamera is null)
                return;

            program.Uniform("NoiseScale", _noiseScale);
            program.Uniform("Samples", _noiseSamples);

            _renderingCamera.SetUniforms(program);
            _renderingCamera.SetAmbientOcclusionUniforms(program);
        }
        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            _renderingCamera = viewport.RenderingCamera;


        }
        public class SSAOGenerator : TObjectSlim
        {
            public const int DefaultSamples = 64;
            const int DefaultNoiseWidth = 4, DefaultNoiseHeight = 4;
            const float DefaultMinSampleDist = 0.1f, DefaultMaxSampleDist = 1.0f;

            public Vec2[] Noise { get; private set; }
            public Vec3[] Kernel { get; private set; }
            public int Samples { get; private set; }
            public int NoiseWidth { get; private set; }
            public int NoiseHeight { get; private set; }
            public float MinSampleDist { get; private set; }
            public float MaxSampleDist { get; private set; }

            public void Generate(
                //int width, int height,
                int samples = DefaultSamples,
                int noiseWidth = DefaultNoiseWidth,
                int noiseHeight = DefaultNoiseHeight,
                float minSampleDist = DefaultMinSampleDist,
                float maxSampleDist = DefaultMaxSampleDist)
            {
                Samples = samples;
                NoiseWidth = noiseWidth;
                NoiseHeight = noiseHeight;
                MinSampleDist = minSampleDist;
                MaxSampleDist = maxSampleDist;

                Random r = new Random();

                Kernel = new Vec3[samples];
                Noise = new Vec2[noiseWidth * noiseHeight];

                float scale;
                Vec3 sample;
                Vec2 noise;

                for (int i = 0; i < samples; ++i)
                {
                    sample = new Vec3(
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() + 0.1f).Normalized();
                    scale = i / (float)samples;
                    sample *= Interp.Lerp(minSampleDist, maxSampleDist, scale * scale);
                    Kernel[i] = sample;
                }

                for (int i = 0; i < Noise.Length; ++i)
                {
                    noise = new Vec2((float)r.NextDouble(), (float)r.NextDouble());
                    noise.Normalize();
                    Noise[i] = noise;
                }
            }
        }
    }
}
