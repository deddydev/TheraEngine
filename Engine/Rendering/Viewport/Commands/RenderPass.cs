using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public class VRPC_RenderPass : ViewportRenderCommand
    {
        public VRPC_RenderPass(ERenderPass pass) => Pass = pass;

        public ERenderPass Pass { get; set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
            => renderingPasses.Render(Pass);
    }
}
