using CustomEngine;
using CustomEngine.Files;
using System.Xml.Serialization;

namespace System
{
    public delegate void TranslateChange(Vec3 oldTranslation);
    public delegate void RotateChange(Quaternion oldRotation);
    public delegate void ScaleChange(Vec3 oldScale);
    public delegate void MatrixChange(Matrix4 oldMatrix, Matrix4 oldInvMatrix);
    public class FrameState : FileObject
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
            _order = order;
            CreateTransform();
        }

        private Vec3 _translation;
        private Quaternion _rotation;
        private Vec3 _scale = Vec3.One;
        private Matrix4 _transform;
        private Matrix4 _inverseTransform;
        private Matrix4.MultiplyOrder _order;

        public event TranslateChange TranslationChanged;
        public event RotateChange RotationChanged;
        public event ScaleChange ScaleChanged;
        public event MatrixChange MatrixChanged;

        public Vec3 GetForwardVector() { return _rotation * Vec3.Forward; }
        public Vec3 GetUpVector() { return _rotation * Vec3.Up; }
        public Vec3 GetRightVector() { return _rotation * Vec3.Right; }
        
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
            TranslationChanged?.Invoke(oldTranslation);
        }
        private void SetRotate(Quaternion value)
        {
            Quaternion oldRotation = _rotation;
            _rotation = value;
            CreateTransform();
            RotationChanged?.Invoke(oldRotation);
        }
        private void SetRotate(Vec3 value)
        {
            Quaternion oldRotation = _rotation;
            _rotation = Quaternion.FromEulerAngles(value);
            CreateTransform();
            RotationChanged?.Invoke(oldRotation);
        }
        private void SetScale(Vec3 value)
        {
            Vec3 oldScale = _scale;
            _scale = value;
            CreateTransform();
            ScaleChanged?.Invoke(oldScale);
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

        #region Animation
        public void Anim_SetRotationZ(float degreeAngle)
        {
            Rotation = Quaternion.FromAxisAngle(Vec3.UnitZ, degreeAngle);
        }
        public void Anim_SetRotationY(float degreeAngle)
        {
            Rotation = Quaternion.FromAxisAngle(Vec3.UnitY, degreeAngle);
        }
        public void Anim_SetRotationX(float degreeAngle)
        {
            Rotation = Quaternion.FromAxisAngle(Vec3.UnitX, degreeAngle);
        }
        public void Anim_AddRotationZ(float degreeAngle)
        {
            Rotation *= Quaternion.FromAxisAngle(Vec3.UnitZ, degreeAngle);
        }
        public void Anim_AddRotationY(float degreeAngle)
        {
            Rotation *= Quaternion.FromAxisAngle(Vec3.UnitY, degreeAngle);
        }
        public void Anim_AddRotationX(float degreeAngle)
        {
            Rotation *= Quaternion.FromAxisAngle(Vec3.UnitX, degreeAngle);
        }
        public void Anim_SetTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_SetTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_SetTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_AddTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_AddTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_AddTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_MultTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_MultTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_MultTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void Anim_SetScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_SetScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_SetScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_AddScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_AddScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_AddScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_MultScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_MultScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void Anim_MultScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        #endregion
    }
}
