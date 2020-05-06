using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class BloomPassVPRC : ViewportFBORenderCommand
    {
        public QuadFrameBuffer BloomBlurFBO1 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO2 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO4 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO8 { get; private set; }
        public QuadFrameBuffer BloomBlurFBO16 { get; private set; }

        public BoundingRectangle BloomRect16;
        public BoundingRectangle BloomRect8;
        public BoundingRectangle BloomRect4;
        public BoundingRectangle BloomRect2;
        //public BoundingRectangle BloomRect1;

        public TexRef2D BloomBlurTexture { get; private set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            PreviousPassFBO.RenderFullscreen();
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
