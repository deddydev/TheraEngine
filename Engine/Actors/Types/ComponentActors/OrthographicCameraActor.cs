using TheraEngine.Rendering.Cameras;
using TheraEngine.Components.Scene;

namespace TheraEngine.Actors.Types
{
    public class OrthographicCameraActor : Actor<CameraComponent>
    {
        public OrthographicCameraActor() : base() { }

        public OrthographicCamera Camera
        {
            get => (OrthographicCamera)RootComponent.CameraRef.File;
            set => RootComponent.CameraRef.File = value;
        }
        
        protected override CameraComponent OnConstruct()
        {
            OrthographicCamera camera = new OrthographicCamera(1.0f, 10000.0f);
            return new CameraComponent(camera);
        }
    }
}
