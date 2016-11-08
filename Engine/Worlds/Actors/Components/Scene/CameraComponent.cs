using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CameraComponent : SceneComponent
    {
        private Camera _camera;

        public CameraComponent(bool orthographic)
        {
            _camera = orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera();
        }

        public void SetCurrent()
        {
            Pawn pawn = Owner as Pawn;
            if (pawn != null)
            {
                LocalPlayerController controller = pawn.Controller as LocalPlayerController;
                if (controller != null)
                    controller.CurrentCamera = _camera;
            }
        }
    }
}
