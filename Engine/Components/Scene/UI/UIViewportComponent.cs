using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public abstract class UIViewportComponent : UIMaterialComponent
    {
        public UIViewportComponent() : base(TMaterial.CreateUnlitTextureMaterialForward()) { }

        public GlobalFileRef<Camera> Camera { get; } = new GlobalFileRef<Camera>();
        public virtual Scene GetScene() => Engine.Scene;
        private Viewport _viewport = new Viewport(0.0f, 0.0f);

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
            _viewport.Resize(parentRegion.Width, parentRegion.Height);
            return base.Resize(parentRegion);
        }
        public override void Render()
        {
            Scene scene = GetScene();
            if (scene is Scene2D scene2d)
                scene2d.CollectVisibleRenderables();
            else if (scene is Scene3D scene3d)
                scene3d.CollectVisibleRenderables(Camera.File.Frustum, false);
            scene.Render(Camera.File, _viewport);
            base.Render();
        }
    }
}
