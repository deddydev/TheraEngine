using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CameraComponent : SceneComponent
    {
        private Camera _camera;

        public void SetCurrent()
        {
            Pawn pawn = Owner as Pawn;
            if (pawn != null)
            {
                PlayerController controller = pawn.Controller as PlayerController;
                if (controller != null)
                {
                    controller.CurrentCamera = _camera;
                }
            }
        }

        protected override void OnRender()
        {
            base.OnRender();
        }
    }
}
