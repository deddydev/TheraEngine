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
            identity._transformOrder = order;
            return identity;
        }
        public static readonly FrameState Identity = new FrameState(Vec3.Zero, Vec3.Zero, Vec3.One);
        public FrameState(
            Vec3 translate, 
            Vec3 rotate,
            Vec3 scale,
            Vec3.EulerOrder rotateOrder = Vec3.EulerOrder.YPR,
            Matrix4.MultiplyOrder transformOrder = Matrix4.MultiplyOrder.SRT)
        {
            _translation = translate;
            _scale = scale;
            _transformOrder = transformOrder;
            _rotateOrder = rotateOrder;
            CreateTransform();
        }

        private Quaternion _yaw = Quaternion.Identity;
        private Quaternion _pitch = Quaternion.Identity;
        private Quaternion _roll = Quaternion.Identity;
        private Quaternion _finalRotation = Quaternion.Identity;
        private Vec3 _translation = Vec3.Zero;
        private Vec3 _scale = Vec3.One;
        private Matrix4 _transform = Matrix4.Identity;
        private Matrix4 _inverseTransform = Matrix4.Identity;
        private Matrix4.MultiplyOrder _transformOrder = Matrix4.MultiplyOrder.SRT;
        private Vec3.EulerOrder _rotateOrder = Vec3.EulerOrder.YPR;

        public event TranslateChange TranslationChanged;
        public event RotateChange YawChanged;
        public event RotateChange PitchChanged;
        public event RotateChange RollChanged;
        public event RotateChange RotationChanged;
        public event ScaleChange ScaleChanged;
        public event MatrixChange MatrixChanged;

        public Vec3 GetForwardVector() { return _finalRotation * Vec3.Forward; }
        public Vec3 GetUpVector() { return _finalRotation * Vec3.Up; }
        public Vec3 GetRightVector() { return _finalRotation * Vec3.Right; }
        
        public Matrix4 Matrix { get { return _transform; } }
        public Matrix4 InverseMatrix { get { return _inverseTransform; } }
        
        public Vec3 Translation
        {
            get { return _translation; }
            set { SetTranslate(value); }
        }
        public Quaternion Yaw
        {
            get { return _yaw; }
            set { SetYaw(value); }
        }
        public Quaternion Pitch
        {
            get { return _pitch; }
            set { SetPitch(value); }
        }
        public Quaternion Roll
        {
            get { return _roll; }
            set { SetRoll(value); }
        }
        public Vec3 Scale
        {
            get { return _scale; }
            set { SetScale(value); }
        }
        public Matrix4.MultiplyOrder Order
        {
            get { return _transformOrder; }
            set { _transformOrder = value; CreateTransform(); }
        }
        public Vec3.EulerOrder RotateOrder
        {
            get { return _rotateOrder; }
            set { _rotateOrder = value; CreateTransform(); }
        }
        private void SetTranslate(Vec3 value)
        {
            Vec3 oldTranslation = _translation;
            _translation = value;
            CreateTransform();
            TranslationChanged?.Invoke(oldTranslation);
        }
        private void SetYaw(Quaternion value)
        {
            Quaternion oldRotation = _yaw;
            _yaw = value;
            CreateTransform();
            YawChanged?.Invoke(oldRotation);
        }
        private void SetPitch(Quaternion value)
        {
            Quaternion oldRotation = _pitch;
            _pitch = value;
            CreateTransform();
            PitchChanged?.Invoke(oldRotation);
        }
        private void SetRoll(Quaternion value)
        {
            Quaternion oldRotation = _roll;
            _roll = value;
            CreateTransform();
            RollChanged?.Invoke(oldRotation);
        }
        //private void SetRotate(Vec3 value)
        //{
        //    Quaternion oldRotation = _rotation;
        //    _rotation = Quaternion.FromEulerAngles(value);
        //    CreateTransform();
        //    RotationChanged?.Invoke(oldRotation);
        //}
        private void SetScale(Vec3 value)
        {
            Vec3 oldScale = _scale;
            _scale = value;
            CreateTransform();
            ScaleChanged?.Invoke(oldScale);
        }
        public void CreateTransform()
        {
            Matrix4 oldMatrix = _transform;
            Matrix4 oldInvMatrix = _inverseTransform;

            CreateFinalRotation();

            _transform = Matrix4.TransformMatrix(_scale, _finalRotation, _translation, _transformOrder);
            _inverseTransform = Matrix4.InverseTransformMatrix(_scale, _finalRotation, _translation, _transformOrder);

            MatrixChanged?.Invoke(oldMatrix, oldInvMatrix);
        }

        private void CreateFinalRotation()
        {
            Quaternion oldRotation = _finalRotation;
            switch (_rotateOrder)
            {
                case Vec3.EulerOrder.PRY:
                    _finalRotation = _pitch * _roll * _yaw;
                    break;
                case Vec3.EulerOrder.PYR:
                    _finalRotation = _pitch * _yaw * _roll;
                    break;
                case Vec3.EulerOrder.RPY:
                    _finalRotation = _roll * _pitch * _yaw;
                    break;
                case Vec3.EulerOrder.RYP:
                    _finalRotation = _roll * _yaw * _pitch;
                    break;
                case Vec3.EulerOrder.YPR:
                    _finalRotation = _yaw * _pitch * _roll;
                    break;
                case Vec3.EulerOrder.YRP:
                    _finalRotation = _yaw * _roll * _pitch;
                    break;
            }
            RotationChanged?.Invoke(oldRotation);
        }

        public void MultMatrix() { Engine.Renderer.MultMatrix(_transform); }
        public void MultInvMatrix() { Engine.Renderer.MultMatrix(_inverseTransform); }

        public void RotateInPlace(Quaternion rotation)
        {
            switch (_transformOrder)
            {
                case Matrix4.MultiplyOrder.TRS:

                    break;
                case Matrix4.MultiplyOrder.TSR:

                    break;
                case Matrix4.MultiplyOrder.STR:

                    break;
                case Matrix4.MultiplyOrder.SRT:

                    break;
                case Matrix4.MultiplyOrder.RTS:

                    break;
                case Matrix4.MultiplyOrder.RST:

                    break;
            }
        }
        public void RotateAboutParent(Quaternion rotation, Vec3 point)
        {
            switch (_transformOrder)
            {
                case Matrix4.MultiplyOrder.TRS:

                    break;
                case Matrix4.MultiplyOrder.TSR:

                    break;
                case Matrix4.MultiplyOrder.STR:

                    break;
                case Matrix4.MultiplyOrder.SRT:

                    break;
                case Matrix4.MultiplyOrder.RTS:

                    break;
                case Matrix4.MultiplyOrder.RST:

                    break;
            }
        }
        public void RotateAboutPoint(Quaternion rotation, Vec3 point)
        {
            switch (_transformOrder)
            {
                case Matrix4.MultiplyOrder.TRS:

                    break;
                case Matrix4.MultiplyOrder.TSR:

                    break;
                case Matrix4.MultiplyOrder.STR:

                    break;
                case Matrix4.MultiplyOrder.SRT:

                    break;
                case Matrix4.MultiplyOrder.RTS:

                    break;
                case Matrix4.MultiplyOrder.RST:

                    break;
            }
        }
        //Translates relative to rotation.
        public void TranslateRelative(Vec3 translation)
        {
            switch (_transformOrder)
            {
                case Matrix4.MultiplyOrder.SRT:
                case Matrix4.MultiplyOrder.RST:
                    Translation += translation;
                    break;
                case Matrix4.MultiplyOrder.RTS:
                    Translation += translation / Scale;
                    break;
                case Matrix4.MultiplyOrder.STR:
                    Translation += _finalRotation.Inverted() * translation;
                    break;
                case Matrix4.MultiplyOrder.TRS:
                    Translation += (_finalRotation.Inverted() * translation) / Scale;
                    break;
                case Matrix4.MultiplyOrder.TSR:
                    Translation += _finalRotation.Inverted() * (translation / Scale);
                    break;
            }
        }
        //Translates relative to parent space.
        public void TranslateAbsolute(Vec3 translation)
        {
            switch (_transformOrder)
            {
                case Matrix4.MultiplyOrder.TRS:
                case Matrix4.MultiplyOrder.TSR:
                    Translation += translation;
                    break;
                case Matrix4.MultiplyOrder.STR:
                    Translation += translation / Scale;
                    break;
                case Matrix4.MultiplyOrder.RTS:
                    Translation += _finalRotation.Inverted() * translation;
                    break;
                case Matrix4.MultiplyOrder.SRT:
                    Translation += (_finalRotation.Inverted() * translation) / Scale;
                    break;
                case Matrix4.MultiplyOrder.RST:
                    Translation += _finalRotation.Inverted() * (translation / Scale);
                    break;
            }
        }

        #region Animation
        public void SetRotationRoll(float degreeAngle)
        {
            Roll = Quaternion.FromAxisAngle(Vec3.Forward, degreeAngle);
        }
        public void SetRotationYaw(float degreeAngle)
        {
            Yaw = Quaternion.FromAxisAngle(Vec3.Up, degreeAngle);
        }
        public void SetRotationPitch(float degreeAngle)
        {
            Pitch = Quaternion.FromAxisAngle(Vec3.Right, degreeAngle);
        }
        public void AddRotationRoll(float degreeAngle)
        {
            Roll *= Quaternion.FromAxisAngle(Vec3.Forward, degreeAngle);
        }
        public void AddRotationYaw(float degreeAngle)
        {
            Yaw *= Quaternion.FromAxisAngle(Vec3.Up, degreeAngle);
        }
        public void AddRotationPitch(float degreeAngle)
        {
            Pitch *= Quaternion.FromAxisAngle(Vec3.Right, degreeAngle);
        }
        public void SetTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void SetTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void SetTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void AddTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void AddTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void AddTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void MultTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void MultTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void MultTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void SetScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void SetScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void SetScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void AddScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void AddScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void AddScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void MultScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void MultScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void MultScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        #endregion
    }
}
