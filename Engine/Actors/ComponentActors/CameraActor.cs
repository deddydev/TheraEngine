using TheraEngine.Rendering.Cameras;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using System.ComponentModel;

namespace TheraEngine.Actors.Types
{
    public class CameraActor : Actor<TransformComponent>
    {
        public CameraActor() : base() { }

        [Browsable(false)]
        public CameraComponent CameraComponent { get; private set; }
        public ICamera Camera
        {
            get => CameraComponent?.CameraRef?.File;
            set
            {
                var fref = CameraComponent?.CameraRef;
                if (fref != null)
                    fref.File = value;
            }
        }
        
        protected override TransformComponent OnConstructRoot()
        {
            Camera camera = new PerspectiveCamera(1.0f, 10000.0f, 45.0f, 16.0f / 9.0f);
            //Camera camera = new OrthographicCamera(1.0f, 10000.0f);
            CameraComponent = new CameraComponent(camera);
            TransformComponent tr = new TransformComponent();
            tr.ChildSockets.Add(CameraComponent);
            return tr;
        }
    }
}
