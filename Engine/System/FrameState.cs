using System;

namespace CustomEngine.Rendering.Models
{
    public delegate void TranslateChange(Vec3 oldTranslation);
    public delegate void RotateChange(Quaternion oldRotation);
    public delegate void ScaleChange(Vec3 oldScale);
    public delegate void MatrixChange(Matrix4 oldMatrix, Matrix4 oldInvMatrix);
    public class FrameState : ObjectBase
    {
        public static FrameState GetIdentity(Matrix4.MultiplyOrder order)
        {
            FrameState identity = Identity;
            identity._order = order;
            return identity;
        }
        public static readonly FrameState Identity = new FrameState(new Vec3(), Quaternion.Identity, new Vec3(1.0f));
        public FrameState(
            Vec3 translate, 
            Quaternion rotate, 
            Vec3 scale, 
            Matrix4.MultiplyOrder order = Matrix4.MultiplyOrder.SRT)
        {
            _translation = translate;
            _rotation = rotate;
            _scale = scale;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
            _order = order;
        }

        private Vec3 _translation;
        private Quaternion _rotation;
        private Vec3 _scale;
        private Matrix4 _transform;
        private Matrix4 _inverseTransform;
        private Matrix4.MultiplyOrder _order;

        public event TranslateChange OnTranslateChanged;
        public event RotateChange OnRotateChanged;
        public event ScaleChange OnScaleChanged;
        public event MatrixChange MatrixChanged;

        public Matrix4 Matrix { get { return _transform; } }
        public Matrix4 InverseMatrix { get { return _inverseTransform; } }
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
        /// <summary>
        /// Pitch, yaw, roll
        /// </summary>
        public Vec3 EulerRotation
        {
            get { return _rotation.ToEuler(); }
            set { SetRotate(value); }
        }
        public Matrix4.MultiplyOrder TransformOrder
        {
            get { return _order; }
            set { _order = value; CreateTransform(); }
        }
        public void ApplyForwardTranslation(Vec3 translation)
        {
            _transform.TranslateRelative(translation);
            SetTranslate(_transform.ExtractTranslation());
        }
        public void SetForwardTranslation(Vec3 translation)
        {
            _transform.ClearTranslation();
            ApplyForwardTranslation(translation);
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
            Matrix4 oldMatrix = _transform;
            Matrix4 oldInvMatrix = _inverseTransform;

            _transform = Matrix4.TransformMatrix(_scale, _rotation, _translation, _order);
            _inverseTransform = Matrix4.InverseTransformMatrix(_scale, _rotation, _translation, _order);

            MatrixChanged?.Invoke(oldMatrix, oldInvMatrix);
        }
        public void MultMatrix() { Engine.Renderer.MultMatrix(_transform); }
        public void MultInvMatrix() { Engine.Renderer.MultMatrix(_inverseTransform); }
    }
}
