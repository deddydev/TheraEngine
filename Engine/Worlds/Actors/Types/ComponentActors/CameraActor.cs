using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds;

namespace CustomEngine.Rendering.Textures
{
    public class CameraActor : Actor
    {
        private CameraComponent _cameraComponent;
        public CameraComponent CameraComponent
        {
            get { return _cameraComponent; }
            set { _cameraComponent = value; }
        }

        protected override SceneComponent SetupComponents()
        {
            return _cameraComponent = new CameraComponent();
        }
    }
}
