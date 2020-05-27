using System;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public class VPRC_BindFBO : ViewportRenderCommand
    {
        public VPRC_BindFBO(FrameBuffer fbo, bool unbind, EFramebufferTarget target)
        {
            FBO = fbo;
            Unbind = unbind;
            Target = target;
        }

        public FrameBuffer FBO { get; set; }
        public bool Unbind { get; set; } = false;
        public EFramebufferTarget Target { get; set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
            => (Unbind ? (Action<EFramebufferTarget>)FBO.Unbind : FBO.Bind)(Target);
    }
}
