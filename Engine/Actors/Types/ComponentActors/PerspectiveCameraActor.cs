using TheraEngine.Rendering.Cameras;
using TheraEngine.Components.Scene;

namespace TheraEngine.Actors.Types
{
    public class PerspectiveCameraActor : Actor<CameraComponent>
    {
        public PerspectiveCameraActor() : base() { }

        public PerspectiveCamera Camera
        {
            get => (PerspectiveCamera)RootComponent.CameraRef;
            set => RootComponent.CameraRef = value;
        }
        
        protected override CameraComponent OnConstruct()
        {
            PerspectiveCamera camera = new PerspectiveCamera(1.0f, 10000.0f, 45.0f, 16.0f / 9.0f);
            return new CameraComponent(camera);
        }
    }
}
