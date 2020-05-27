using System;
using System.IO;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class VPRC_Bloom : ViewportRenderCommand
    {
        public QuadFrameBuffer BloomBlurFBO1 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO2 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO4 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO8 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO16 { get; private set; }
        public QuadFrameBuffer ForwardPassFBO { get; private set; }

        public BoundingRectangle BloomRect16;
        public BoundingRectangle BloomRect8;
        public BoundingRectangle BloomRect4;
        public BoundingRectangle BloomRect2;
        //public BoundingRectangle BloomRect1;

        public TexRef2D BloomBlurTexture { get; private set; }

        public override void DestroyFBOs()
        {
            BloomBlurFBO1?.Dispose();
            BloomBlurFBO1 = null;

            BloomBlurFBO2?.Dispose();
            BloomBlurFBO2 = null;

            BloomBlurFBO4?.Dispose();
            BloomBlurFBO4 = null;

            BloomBlurFBO8?.Dispose();
            BloomBlurFBO8 = null;

            BloomBlurFBO16?.Dispose();
            BloomBlurFBO16 = null;

            ForwardPassFBO = null;
        }
        public override void GenerateFBOs(Viewport viewport)
        {
            ForwardPassFBO = viewport.Pipeline.GetFBO<QuadFrameBuffer>("ForwardPass");

            int width = viewport.InternalResolution.Width;
            int height = viewport.InternalResolution.Height;

            BloomRect16.Width = (int)(width * 0.0625f);
            BloomRect16.Height = (int)(height * 0.0625f);
            BloomRect8.Width = (int)(width * 0.125f);
            BloomRect8.Height = (int)(height * 0.125f);
            BloomRect4.Width = (int)(width * 0.25f);
            BloomRect4.Height = (int)(height * 0.25f);
            BloomRect2.Width = (int)(width * 0.5f);
            BloomRect2.Height = (int)(height * 0.5f);
            //BloomRect1.Width = width;
            //BloomRect1.Height = height;

            BloomBlurTexture = TexRef2D.CreateFrameBufferTexture(
                "BloomBlur",
                width,
                height,
                EPixelInternalFormat.Rgb8,
                EPixelFormat.Rgb,
                EPixelType.UnsignedByte);

            BloomBlurTexture.MagFilter = ETexMagFilter.Linear;
            BloomBlurTexture.MinFilter = ETexMinFilter.LinearMipmapLinear;
            BloomBlurTexture.UWrap = ETexWrapMode.ClampToEdge;
            BloomBlurTexture.VWrap = ETexWrapMode.ClampToEdge;

            viewport.Pipeline.SetTexture(BloomBlurTexture);

            TMaterial bloomBlurMat = new TMaterial(
                "BloomBlurMat",
                new RenderingParameters()
                {
                    DepthTest =
                    {
                        Enabled = ERenderParamUsage.Unchanged,
                        UpdateDepth = false,
                        Function = EComparison.Always,
                    }
                },
                new ShaderVar[]
                {
                    new ShaderFloat(0.0f, "Ping"),
                    new ShaderInt(0, "LOD"),
                },
                new[] { BloomBlurTexture },
                Engine.Files.Shader(Path.Combine(SceneShaderPath, "BrightPass.fs"), EGLSLType.Fragment));

            BloomBlurFBO1 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO2 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO4 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO8 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO16 = new QuadFrameBuffer(bloomBlurMat);

            BloomBlurFBO1.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));
            BloomBlurFBO2.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 1, -1));
            BloomBlurFBO4.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 2, -1));
            BloomBlurFBO8.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 3, -1));
            BloomBlurFBO16.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 4, -1));

            viewport.Pipeline.SetFBO("BloomBlur1", BloomBlurFBO1);
            viewport.Pipeline.SetFBO("BloomBlur2", BloomBlurFBO2);
            viewport.Pipeline.SetFBO("BloomBlur4", BloomBlurFBO4);
            viewport.Pipeline.SetFBO("BloomBlur8", BloomBlurFBO8);
            viewport.Pipeline.SetFBO("BloomBlur16", BloomBlurFBO16);
        }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            ForwardPassFBO.RenderFullscreen();
            BloomBlurFBO1.Unbind(EFramebufferTarget.DrawFramebuffer);

            var tex = BloomBlurTexture.GetTexture(true);
            tex.Bind();
            tex.GenerateMipmaps();

            BloomScaledPass(BloomBlurFBO16, BloomRect16, 4);
            BloomScaledPass(BloomBlurFBO8, BloomRect8, 3);
            BloomScaledPass(BloomBlurFBO4, BloomRect4, 2);
            BloomScaledPass(BloomBlurFBO2, BloomRect2, 1);
            //Don't blur original image, barely makes a difference to result
        }
        private void BloomBlur(QuadFrameBuffer fbo, int mipmap, float dir)
        {
            fbo.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                fbo.Material.Parameter<ShaderFloat>(0).Value = dir;
                fbo.Material.Parameter<ShaderInt>(1).Value = mipmap;
                fbo.RenderFullscreen();
            }
            fbo.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void BloomScaledPass(QuadFrameBuffer fbo, BoundingRectangle rect, int mipmap)
        {
            Engine.Renderer.PushRenderArea(rect);
            {
                BloomBlur(fbo, mipmap, 0.0f);
                BloomBlur(fbo, mipmap, 1.0f);
            }
            Engine.Renderer.PopRenderArea();
        }
    }
}
