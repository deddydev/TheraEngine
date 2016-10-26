using System;

namespace CustomEngine.Rendering.Models
{
    public delegate void TranslateChanged(Vec3 oldTranslation);
    public delegate void RotateChanged(Quaternion oldRotate);
    public delegate void ScaleChanged(Vec3 oldScale);
    public class FrameState : ObjectBase
    {
        public static readonly FrameState Identity = new FrameState(new Vec3(), Quaternion.Identity, new Vec3(1.0f));
        public FrameState(Vec3 translate, Quaternion rotate, Vec3 scale)
        {
            _translation = translate;
            _rotation = rotate;
            _scale = scale;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
        }

        private Vec3 _translation;
        private Quaternion _rotation;
        private Vec3 _eulerRotation;
        private Vec3 _scale;
        private Matrix4 _transform;
        private Matrix4 _inverseTransform;

        public event TranslateChanged OnTranslateChanged;
        public event RotateChanged OnRotateChanged;
        public event ScaleChanged OnScaleChanged;

        public Matrix4 Transform { get { return _transform; } }
        public Matrix4 InverseTransform { get { return _inverseTransform; } }
        public Vec3 Translation
        {
            get { return _translation; }
            set { SetTranslate(value); }
        }
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { SetRotate(value); }
        }
        public Vec3 Scale
        {
            get { return _scale; }
            set { SetScale(value); }
        }
        public Vec3 EulerRotation
        {
            get { return _eulerRotation; }
            set { SetRotate(value); }
        }
        public void ApplyRelativeTranslation(Vec3 translation)
        {
            _transform.Translate(translation);
            SetTranslate(_transform.ExtractTranslation());
        }
        public void SetRelativeTranslation(Vec3 translation)
        {
            _transform.ClearTranslation();
            ApplyRelativeTranslation(translation);
        }

        private void SetTranslate(Vec3 value)
        {
            Vec3 oldTranslation = _translation;
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
        private void SetRotate(Vec3 value)
        {
            Quaternion oldRotation = _rotation;
            _eulerRotation = value;
            _rotation = Quaternion.FromEulerAngles(value);
            CreateTransform();
            OnRotateChanged?.Invoke(oldRotation);
        }
        private void SetScale(Vec3 value)
        {
            Vec3 oldScale = _scale;
            _scale = value;
            CreateTransform();
            OnScaleChanged?.Invoke(oldScale);
        }

        public void AddTranslation(Vec3 translation)
        {
            SetTranslate(_translation + translation);
        }
        public void ApplyRotation(Vec3 rotation)
        {
            SetRotate(_rotation * Quaternion.FromEulerAngles(rotation));
        }
        public void ApplyRotation(Quaternion rotation)
        {
            SetRotate(_rotation * rotation);
        }
        public void MultiplyScale(Vec3 scale)
        {
            SetScale(_scale * scale);
        }
        public void SetAll(Vec3 translation, Quaternion rotation, Vec3 scale)
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
