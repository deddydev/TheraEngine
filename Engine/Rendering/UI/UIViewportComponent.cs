using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using static TheraEngine.Worlds.Actors.Components.Scene.Mesh.SkeletalMeshComponent;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a viewport that renders a scene from a designated camera.
    /// </summary>
    public abstract class UIViewportComponent : UIMaterialComponent
    {
        public UIViewportComponent() : base(TMaterial.GetUnlitTextureMaterialForward()) { }

        public SingleFileRef<Camera> Camera { get; } = new SingleFileRef<Camera>();
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
            GetScene().Render(Camera.File, Camera.File.Frustum, _viewport, false);
            base.Render();
        }
    }
}
