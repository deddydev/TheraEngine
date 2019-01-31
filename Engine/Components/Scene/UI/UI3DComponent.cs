using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Houses a self-contained 3D scene within a 2D user interface.
    /// </summary>
    public class UI3DComponent : UIViewportComponent
    {
        public UI3DComponent() : base()
        {

        }

        public override Camera ViewportCamera
        {
            get => base.ViewportCamera;
            set => base.ViewportCamera = value;
        }

        private BaseScene _scene;
    }
}
