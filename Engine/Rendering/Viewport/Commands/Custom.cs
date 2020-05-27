using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public sealed class VPRC_Custom : ViewportRenderCommand
    {
        public VPRC_Custom(DelExecute method) => Method = method;

        public DelExecute Method { get; set; }

        public delegate void DelExecute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target);

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
            => Method?.Invoke(renderingPasses, scene, camera, viewport, target);

        public override void GenerateFBOs(Viewport viewport) { }
        public override void DestroyFBOs() { }
    }
}
