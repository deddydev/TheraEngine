using TheraEngine.Rendering.Cameras;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using System.ComponentModel;

namespace TheraEngine.Actors.Types
{
    public class CameraActor : Actor<TRComponent>
    {
        public CameraActor() : base() { }

        [Browsable(false)]
        public CameraComponent CameraComponent { get; private set; }
        public Camera Camera
        {
            get => CameraComponent.CameraRef.File;
            set => CameraComponent.CameraRef.File = value;
        }
        
        protected override TRComponent OnConstructRoot()
        {
            Camera camera = new PerspectiveCamera(1.0f, 10000.0f, 45.0f, 16.0f / 9.0f);
            //Camera camera = new OrthographicCamera(1.0f, 10000.0f);
            CameraComponent = new CameraComponent(camera);
            TRComponent tr = new TRComponent();
            tr.ChildComponents.Add(CameraComponent);
            return tr;
        }
    }
}
