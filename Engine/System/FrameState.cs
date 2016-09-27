using OpenTK;

namespace CustomEngine.Rendering.Models
{
    public delegate void TranslateChanged(Vector3 oldTranslation);
    public delegate void RotateChanged(Quaternion oldRotate);
    public delegate void ScaleChanged(Vector3 oldScale);
    public class FrameState
    {
        public static readonly FrameState Identity = new FrameState(new Vector3(), Quaternion.Identity, new Vector3(1.0f));
        public FrameState(Vector3 translate, Quaternion rotate, Vector3 scale)
        {
            _translation = translate;
            _rotation = rotate;
            _scale = scale;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
        }

        private Vector3 _translation;
        private Quaternion _rotation;
        private Vector3 _scale;
        private Matrix4 _transform;
        private Matrix4 _inverseTransform;

        public event TranslateChanged OnTranslateChanged;
        public event RotateChanged OnRotateChanged;
        public event ScaleChanged OnScaleChanged;

        public Matrix4 Transform { get { return _transform; } }
        public Matrix4 InverseTransform { get { return _inverseTransform; } }
        public Vector3 Translation
        {
            get { return _translation; }
            set { SetTranslate(value); }
        }
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { SetRotate(value); }
        }
        public Vector3 Scale
        {
            get { return _scale; }
            set { SetScale(value); }
        }

        private void SetTranslate(Vector3 value)
        {
            Vector3 oldTranslation = _translation;
            _translation = value;
            CreateTransform();
            OnTranslateChanged?.Invoke(oldTranslation);
        }
        private void SetRotate(Quaternion value)
        {
            Quaternion oldRotation = _rotation;
            _rotation = value;
            CreateTransform();
            OnRotateChanged?.Invoke(oldRotation);
        }
        private void SetScale(Vector3 value)
        {
            Vector3 oldScale = _scale;
            _scale = value;
            CreateTransform();
            OnScaleChanged?.Invoke(oldScale);
        }

        public void AddTranslation(Vector3 translation)
        {
            SetTranslate(_translation + translation);
        }
        public void ApplyRotation(Vector3 rotation)
        {
            SetRotate(_rotation * Quaternion.FromEulerAngles(rotation));
        }
        public void ApplyRotation(Quaternion rotation)
        {
            SetRotate(_rotation * rotation);
        }
        public void MultiplyScale(Vector3 scale)
        {
            SetScale(_scale * scale);
        }
        public void SetAll(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            SetTranslate(translation);
            SetRotate(rotation);
            SetScale(scale);
            CreateTransform();
        }
        public void CreateTransform()
        {
            _transform = Matrix4.TransformMatrix(_scale, _rotation, _translation);
            _inverseTransform = Matrix4.InverseTransformMatrix(_scale, _rotation, _translation);
        }
        public void MultMatrix() { Engine.Renderer.MultMatrix(_transform); }
        public void MultInvMatrix() { Engine.Renderer.MultMatrix(_inverseTransform); }
    }
}
