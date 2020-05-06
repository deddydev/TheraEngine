using System;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public class RenderPipelineInfo
    {
        public RenderPasses RenderingPasses { get; set; }
        public IScene Scene { get; set; }
        public ICamera Camera { get; set; }
        public Viewport Viewport { get; set; }
        public FrameBuffer Target { get; set; }
    }
    public abstract class ViewportRenderCommand
    {
        public abstract void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target);
        public virtual void GenerateFBOs(Viewport viewport) { }
        public virtual void DestroyFBOs() { }
    }
    public abstract class ViewportFBORenderCommand : ViewportRenderCommand
    {
        public QuadFrameBuffer PreviousPassFBO { get; private set; }
    }
}
