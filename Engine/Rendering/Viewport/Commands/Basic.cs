using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class BasicVPRC : ViewportRenderCommand
    {
        public enum EDefaultViewportRenderCommand
        {
            PushTargetCamera,
            PopCamera,
            PushScene,
            PopScene,
            PushViewportRenderArea,
            PopRenderArea,
        }

        public EDefaultViewportRenderCommand Command { get; set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            var r = Engine.Renderer;
            if (r is null)
                return;

            switch (Command)
            {
                case EDefaultViewportRenderCommand.PushTargetCamera:
                    r.PushCamera(camera);
                    viewport?.PushRenderingCamera(camera);
                    break;
                case EDefaultViewportRenderCommand.PopCamera:
                    r.PopCamera();
                    viewport?.PopRenderingCamera();
                    break;
                case EDefaultViewportRenderCommand.PushScene:
                    {
                        if (scene is IScene3D s3d)
                            r.PushCurrent3DScene(s3d);

                        if (scene is IScene2D s2d)
                            r.PushCurrent2DScene(s2d);
                    }
                    break;
                case EDefaultViewportRenderCommand.PopScene:
                    {
                        if (scene is IScene3D)
                            r.PopCurrent3DScene();

                        if (scene is IScene2D)
                            r.PopCurrent3DScene();
                    }
                    break;
            }
        }
    }
}
