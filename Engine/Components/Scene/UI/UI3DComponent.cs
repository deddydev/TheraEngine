using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a self-contained 3D scene within a 2D user interface.
    /// </summary>
    public class UI3DComponent : UIMaterialRectangleComponent
    {
        public UI3DComponent() : base(TMaterial.CreateUnlitTextureMaterialForward())
        {
            _camera = new PerspectiveCamera();
        }

        private Scene _scene;
        private Camera _camera;
        
        public override void OnSpawned()
        {
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
        }
    }
}
