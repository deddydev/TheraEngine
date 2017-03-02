using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Worlds.Actors.Types
{
    public class CameraActor : Actor<CameraComponent>
    {
        public CameraActor() : base() { }

        private CameraComponent _cameraComponent;
        public CameraComponent CameraComponent
        {
            get { return _cameraComponent; }
            set { _cameraComponent = value; }
        }

        protected override CameraComponent SetupComponents()
        {
            _cameraComponent = new CameraComponent();
            PerspectiveCamera cam = (PerspectiveCamera)_cameraComponent.Camera;
            cam.FarZ = 500.0f;
            cam.VerticalFieldOfView = 30.0f;
            return _cameraComponent;
        }

        public override void OnSpawned(World world)
        {
            Engine.Renderer.Scene.AddRenderable(_cameraComponent.Camera);
            base.OnSpawned(world);
        }

        public override void OnDespawned()
        {
            Engine.Renderer.Scene.RemoveRenderable(_cameraComponent.Camera);
            base.OnDespawned();
        }
    }
}
