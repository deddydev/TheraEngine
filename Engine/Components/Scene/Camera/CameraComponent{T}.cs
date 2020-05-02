using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    [TFileDef("Camera Component")]
    public class CameraComponent<T> : BaseCameraComponent where T : Camera
    {
        public CameraComponent() : this(null) { }
        public CameraComponent(T camera)
        {
            _cameraRef = new GlobalFileRef<T>(camera);
            _cameraRef.Loaded += CameraLoaded;
            _cameraRef.Unloaded += CameraUnloaded;
        }
        
        private GlobalFileRef<T> _cameraRef;

        [Browsable(false)]
        protected override ICamera GenericCamera
        {
            get => Camera;
            set => Camera = value as T;
        }
        [Browsable(false)]
        public T Camera
        {
            get => CameraRef?.File;
            set
            {
                if (CameraRef != null)
                    CameraRef.File = value;
                else
                    CameraRef = new GlobalFileRef<T>(value);
            }
        }

        [DisplayName(nameof(Camera))]
        [TSerialize]
        public GlobalFileRef<T> CameraRef
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

        private void CameraLoaded(T camera)
        {
            camera.OwningComponent = this;
            camera.TransformChanged += RecalcLocalTransform;
        }
        private void CameraUnloaded(T camera)
        {
            camera.OwningComponent = null;
            camera.TransformChanged -= RecalcLocalTransform;
        }
    }
}
