using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public enum EVPRCommand
    {
        PushViewportCamera,
        PopCamera,
        PushViewportScene,
        PopScene,
        PushViewportInternalResolutionRenderArea,
        PushFullResolutionRenderArea,
        PopRenderArea,
    }

    public sealed class VPRC_Basic : ViewportRenderCommand
    {
        public VPRC_Basic(EVPRCommand command) => Command = command;

        public EVPRCommand Command { get; set; }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            var r = Engine.Renderer;
            if (r is null)
                return;

            switch (Command)
            {
                case EVPRCommand.PushViewportCamera:
                    r.PushCamera(camera);
                    viewport?.PushRenderingCamera(camera);
                    break;
                case EVPRCommand.PopCamera:
                    r.PopCamera();
                    viewport?.PopRenderingCamera();
                    break;
                case EVPRCommand.PushViewportScene:
                    {
                        if (scene is IScene3D s3d)
                            r.PushCurrent3DScene(s3d);

                        if (scene is IScene2D s2d)
                            r.PushCurrent2DScene(s2d);
                    }
                    break;
                case EVPRCommand.PopScene:
                    {
                        if (scene is IScene3D)
                            r.PopCurrent3DScene();

                        if (scene is IScene2D)
                            r.PopCurrent3DScene();
                    }
                    break;
                case EVPRCommand.PushViewportInternalResolutionRenderArea:
                    r.PushRenderArea(viewport.InternalResolution);
                    break;
                case EVPRCommand.PushFullResolutionRenderArea:
                    r.PushRenderArea(viewport.Region);
                    break;
                case EVPRCommand.PopRenderArea:
                    r.PopRenderArea();
                    break;
            }
        }

        public override void GenerateFBOs(Viewport viewport) { }
        public override void DestroyFBOs() { }
    }
}
