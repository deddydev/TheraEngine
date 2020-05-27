using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public class VPRC_BlitFBO : ViewportRenderCommand
    {
        public VPRC_BlitFBO(QuadFrameBuffer source, FrameBuffer destination)
        {
            Source = source;
            Destination = destination;
        }

        public QuadFrameBuffer Source { get; private set; }
        public FrameBuffer Destination { get; private set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
            => Source?.RenderTo(Destination);
    }
}
