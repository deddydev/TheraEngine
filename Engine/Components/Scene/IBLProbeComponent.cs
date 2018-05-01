using System;
using System.Drawing;
using System.IO;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Actors.Types
{
    public class IBLProbeComponent : SceneCaptureComponent, I3DRenderable
    {
        private CubeFrameBuffer _irradianceFBO;
        private CubeFrameBuffer _prefilterFBO;
        private TexRefCube _irradianceTex;
        private TexRefCube _prefilterTex;

        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);
        public Shape CullingVolume => null;
        public IOctreeNode OctreeNode { get; set; }

        public TexRefCube IrradianceTex => _irradianceTex;
        public TexRefCube PrefilterTex => _prefilterTex;

        public IBLProbeComponent() : base()
        {
            _rc = new RenderCommandDebug3D(Render);
        }

        public override void OnSpawned()
        {
            OwningScene.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            OwningScene.Remove(this);
            base.OnDespawned();
        }

        //protected override FrameBuffer GetFBO(int cubeExtent)
        //{
        //    _cubeTex = new TexRefCube("SceneCaptureCubeMap", cubeExtent,
        //        EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
        //    {
        //        MinFilter = ETexMinFilter.LinearMipmapLinear,
        //        MagFilter = ETexMagFilter.Linear,
        //        UWrap = ETexWrapMode.ClampToEdge,
        //        VWrap = ETexWrapMode.ClampToEdge,
        //        WWrap = ETexWrapMode.ClampToEdge,
        //    };

        //    TMaterial mat = new TMaterial("PassThruEnvMat", new BaseTexRef[] { _cubeTex }, 
        //        Engine.LoadEngineShader(Path.Combine("Common", "UnlitTexturedForward.fs"), EShaderMode.Fragment));

        //    CubeFrameBuffer f = new CubeFrameBuffer(mat, true);
        //    f.SetRenderTargets((_cubeTex, EFramebufferAttachment.ColorAttachment0, 0));
        //    return f;
        //}
        protected override void Initialize(int resolution)
        {
            base.Initialize(resolution);

            _irradianceTex = new TexRefCube("IrradianceTex", resolution,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.Linear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
            };
            _prefilterTex = new TexRefCube("PrefilterTex", resolution,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.Linear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
            };

            RenderingParameters r = new RenderingParameters();
            r.DepthTest.Enabled = ERenderParamUsage.Disabled;

            ShaderVar[] prefilterVars = new ShaderVar[] { new ShaderFloat(0.0f, "Roughness") };

            GLSLShaderFile irrShader = Engine.LoadEngineShader(Path.Combine("Scene3D", "IrradianceConvoluton.fs"), EShaderMode.Fragment);
            GLSLShaderFile prefShader = Engine.LoadEngineShader(Path.Combine("Scene3D", "Prefilter.fs"), EShaderMode.Fragment);
            TMaterial irrMat = new TMaterial("IrradianceMat", r, new ShaderVar[0], new TexRefCube[] { _cubeTex }, irrShader);
            TMaterial prefMat = new TMaterial("PrefilterMat", r, prefilterVars, new TexRefCube[] { _cubeTex }, prefShader);
            _irradianceFBO = new CubeFrameBuffer(irrMat, true);
            _prefilterFBO = new CubeFrameBuffer(prefMat, true);
            _irradianceFBO.SetRenderTargets((_irradianceTex, EFramebufferAttachment.ColorAttachment0, 0));
            _prefilterFBO.SetRenderTargets((_prefilterTex, EFramebufferAttachment.ColorAttachment0, 0));
        }
        public void GenerateIrradianceMap()
        {
            //CubeFrameBuffer fbo = (CubeFrameBuffer)_renderFBO;

            _irradianceFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(Vec2.Zero, new Vec2(32, 32)));
            for (int i = 0; i < 6; ++i)
            {
                _irradianceTex.AttachFaceToFBO(_irradianceFBO.BindingId, ECubemapFace.PosX + i);
                Engine.Renderer.Clear(EBufferClear.Color);
                _irradianceFBO.RenderFullscreen(ECubemapFace.PosX + i);
            }
            _irradianceFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        public void GeneratePrefilterMap()
        {
            //CubeFrameBuffer fbo = (CubeFrameBuffer)_renderFBO;

            _prefilterFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            int maxMipLevels = 5;
            for (int mip = 0; mip < maxMipLevels; ++mip)
            {
                int mipWidth = (int)(128.0 * Math.Pow(0.5, mip));
                int mipHeight = (int)(128.0 * Math.Pow(0.5, mip));
                Engine.Renderer.PushRenderArea(new BoundingRectangle(Vec2.Zero, new Vec2(mipWidth, mipHeight)));
                float roughness = (float)mip / (maxMipLevels - 1);
                
                _prefilterFBO.Material.Parameter<ShaderFloat>(0).Value = roughness;

                for (int i = 0; i < 6; ++i)
                {
                    _prefilterTex.AttachFaceToFBO(_prefilterFBO.BindingId, ECubemapFace.PosX + i, mip);
                    Engine.Renderer.Clear(EBufferClear.Color);
                    _prefilterFBO.RenderFullscreen(ECubemapFace.PosX + i);
                }
            }
            _prefilterFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void Render()
        {
            Engine.Renderer.RenderPoint(WorldPoint, Color.Red);
        }
        private RenderCommandDebug3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
    }
}
