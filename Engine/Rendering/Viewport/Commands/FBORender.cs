using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class BlitFBOPassVPRC : ViewportFBORenderCommand
    {
        public QuadFrameBuffer Source { get; private set; }
        public FrameBuffer Destination { get; private set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            Source.RenderTo(Destination);
        }
    }
}
