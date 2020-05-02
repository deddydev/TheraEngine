using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    [TFileDef("Camera Component")]
    public class CameraComponent : BaseCameraComponent
    {
        public CameraComponent() : this(null) { }
        public CameraComponent(bool orthographic) : this(orthographic ? (ICamera)new OrthographicCamera() : new PerspectiveCamera()) { }
        public CameraComponent(ICamera camera)
        {
            _cameraRef = new GlobalFileRef<ICamera>(camera);
            _cameraRef.Loaded += CameraLoaded;
            _cameraRef.Unloaded += CameraUnloaded;
        }
        
        private GlobalFileRef<ICamera> _cameraRef;

        protected void CameraLoaded(ICamera camera)
        {
            camera.OwningComponent = this;
            camera.TransformChanged += RecalcLocalTransform;
        }
        protected void CameraUnloaded(ICamera camera)
        {
            camera.OwningComponent = null;
            camera.TransformChanged -= RecalcLocalTransform;
        }

        public ICamera Camera 
        {
            get => GenericCamera;
            set => GenericCamera = value;
        }

        [Browsable(false)]
        protected override ICamera GenericCamera
        {
            get => CameraRef?.File;
            set
            {
                if (CameraRef != null)
                    CameraRef.File = value;
                else
                    CameraRef = new GlobalFileRef<ICamera>(value);
            }
        }

        [DisplayName(nameof(Camera))]
        [TSerialize]
        public GlobalFileRef<ICamera> CameraRef
        {
            get => _cameraRef;
            set
            {
                if (_cameraRef != null)
                {
                    _cameraRef.Loaded -= CameraLoaded;
                    if (_cameraRef.IsLoaded && _cameraRef.File != null)
                    {
                        ICamera camera = _cameraRef.File;
                        camera.OwningComponent = null;
                        camera.TransformChanged -= RecalcLocalTransform;
                    }
                }
                _cameraRef = value;
            }
        }
    }
}
