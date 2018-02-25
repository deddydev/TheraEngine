using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public abstract class UIViewportComponent : UIDockableComponent, I2DRenderable
    {
        public UIViewportComponent() : base() { }
        
        public abstract Camera GetCamera();
        public abstract Scene GetScene();

        private Viewport _viewport = new Viewport(1.0f, 1.0f);

        public RenderInfo2D RenderInfo { get; } = new RenderInfo2D(ERenderPass2D.Opaque, 0, 0);

        public override void OnSpawned()
        {
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
        }

        public override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle r = base.Resize(parentRegion);
            _viewport.Resize(Width, Height);
            return r;
        }
        public void Render()
        {
            Scene scene = GetScene();
            Camera camera = GetCamera();

            _viewport.Render(scene, camera, camera.Frustum);
        }
    }
}
