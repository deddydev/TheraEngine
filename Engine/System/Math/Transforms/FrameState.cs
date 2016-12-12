using CustomEngine;
using CustomEngine.Files;
using System.Xml.Serialization;

namespace System
{
    public delegate void TranslateChange(Vec3 oldTranslation);
    public delegate void RotateChange(float oldRotation);
    public delegate void ScaleChange(Vec3 oldScale);
    public delegate void MatrixChange(Matrix4 oldMatrix, Matrix4 oldInvMatrix);
    public class FrameState : FileObject
    {
        public static FrameState GetIdentity(Matrix4.MultiplyOrder transformationOrder, Rotator.Order rotationOrder)
        {
            FrameState identity = Identity;
            identity._transformOrder = transformationOrder;
            identity.RotationOrder = rotationOrder;
            return identity;
        }
        public static readonly FrameState Identity = new FrameState(Vec3.Zero, Rotator.GetZero(Rotator.Order.YPR), Vec3.One);
        public FrameState()
        {
            _translation = Vec3.Zero;
            _rotation = new Rotator(Rotator.Order.YPR);
            _scale = Vec3.One;
            _transformOrder = Matrix4.MultiplyOrder.TRS;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
        }
        public FrameState(
            Vec3 translate, 
            Rotator rotate,
            Vec3 scale,
            Matrix4.MultiplyOrder transformOrder = Matrix4.MultiplyOrder.TRS)
        {
            _translation = translate;
            _scale = scale;
            _transformOrder = transformOrder;
            _rotation = rotate;
            CreateTransform();
        }
        
        private Rotator _rotation;
        private Vec3 _translation = Vec3.Zero;
        private Vec3 _scale = Vec3.One;
        private Matrix4 _transform = Matrix4.Identity;
        private Matrix4 _inverseTransform = Matrix4.Identity;
        private Matrix4.MultiplyOrder _transformOrder = Matrix4.MultiplyOrder.TRS;

        public event TranslateChange TranslationChanged;
        public event RotateChange YawChanged;
        public event RotateChange PitchChanged;
        public event RotateChange RollChanged;
        public event ScaleChange ScaleChanged;
        public event MatrixChange MatrixChanged;
        public void Lookat(Vec3 point)
        {
            SetForwardVector(point - _transform.GetPoint());
        }
        public void SetForwardVector(Vec3 direction)
        {

        }
        public Vec3 GetForwardVector() { return _rotation.TransformVector(Vec3.Forward); }
        public Vec3 GetUpVector() { return _rotation.TransformVector(Vec3.Up); }
        public Vec3 GetRightVector() { return _rotation.TransformVector(Vec3.Right); }
        public Matrix4 GetRotationMatrix() { return _rotation.GetMatrix(); }

        public Matrix4 Matrix { get { return _transform; } }
        public Matrix4 InverseMatrix { get { return _inverseTransform; } }
        
        public Vec3 Translation
        {
            get { return _translation; }
            set { SetTranslate(value); }
        }
        public float Yaw
        {
            get { return _rotation.Yaw; }
            set { SetYaw(value); }
        }
        public float Pitch
        {
            get { return _rotation.Pitch; }
            set { SetPitch(value); }
        }
        public float Roll
        {
            get { return _rotation.Roll; }
            set { SetRoll(value); }
        }
        public Vec3 Scale
        {
            get { return _scale; }
            set { SetScale(value); }
        }
        public Matrix4.MultiplyOrder TransformationOrder
        {
            get { return _transformOrder; }
            set { _transformOrder = value; CreateTransform(); }
        }
        public Rotator.Order RotationOrder
        {
            get { return _rotation._rotationOrder; }
            set { _rotation._rotationOrder = value; CreateTransform(); }
        }
        private void SetTranslate(Vec3 value)
        {
            Vec3 oldTranslation = _translation;
            _translation = value;
            CreateTransform();
            TranslationChanged?.Invoke(oldTranslation);
        }
        private void SetYaw(float value)
        {
            float oldRotation = _rotation.Yaw;
            _rotation.Yaw = value;
            CreateTransform();
            YawChanged?.Invoke(oldRotation);
        }
        private void SetPitch(float value)
        {
            float oldRotation = _rotation.Pitch;
            _rotation.Pitch = value;
            CreateTransform();
            PitchChanged?.Invoke(oldRotation);
        }
        private void SetRoll(float value)
        {
            float oldRotation = _rotation.Roll;
            _rotation.Roll = value;
            CreateTransform();
            RollChanged?.Invoke(oldRotation);
        }
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

            _transform = Matrix4.TransformMatrix(_scale, _rotation, _translation, _transformOrder);
            _inverseTransform = Matrix4.InverseTransformMatrix(_scale, _rotation, _translation, _transformOrder);

            MatrixChanged?.Invoke(oldMatrix, oldInvMatrix);
        }

        //public void MultMatrix() { Engine.Renderer.MultMatrix(_transform); }
        //public void MultInvMatrix() { Engine.Renderer.MultMatrix(_inverseTransform); }

        //public void RotateInPlace(Quaternion rotation)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:

        //            break;
        //        case Matrix4.MultiplyOrder.TSR:

        //            break;
        //        case Matrix4.MultiplyOrder.STR:

        //            break;
        //        case Matrix4.MultiplyOrder.SRT:

        //            break;
        //        case Matrix4.MultiplyOrder.RTS:

        //            break;
        //        case Matrix4.MultiplyOrder.RST:

        //            break;
        //    }
        //}
        //public void RotateAboutParent(Quaternion rotation, Vec3 point)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:

        //            break;
        //        case Matrix4.MultiplyOrder.TSR:

        //            break;
        //        case Matrix4.MultiplyOrder.STR:

        //            break;
        //        case Matrix4.MultiplyOrder.SRT:

        //            break;
        //        case Matrix4.MultiplyOrder.RTS:

        //            break;
        //        case Matrix4.MultiplyOrder.RST:

        //            break;
        //    }
        //}
        //public void RotateAboutPoint(Quaternion rotation, Vec3 point)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:

        //            break;
        //        case Matrix4.MultiplyOrder.TSR:

        //            break;
        //        case Matrix4.MultiplyOrder.STR:

        //            break;
        //        case Matrix4.MultiplyOrder.SRT:

        //            break;
        //        case Matrix4.MultiplyOrder.RTS:

        //            break;
        //        case Matrix4.MultiplyOrder.RST:

        //            break;
        //    }
        //}
        ////Translates relative to rotation.
        //public void TranslateRelative(Vec3 translation)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.SRT:
        //        case Matrix4.MultiplyOrder.RST:
        //            Translation += translation;
        //            break;
        //        case Matrix4.MultiplyOrder.RTS:
        //            Translation += translation / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.STR:
        //            Translation += _finalRotation.Inverted() * translation;
        //            break;
        //        case Matrix4.MultiplyOrder.TRS:
        //            Translation += (_finalRotation.Inverted() * translation) / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.TSR:
        //            Translation += _finalRotation.Inverted() * (translation / Scale);
        //            break;
        //    }
        //}
        ////Translates relative to parent space.
        //public void TranslateAbsolute(Vec3 translation)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:
        //        case Matrix4.MultiplyOrder.TSR:
        //            Translation += translation;
        //            break;
        //        case Matrix4.MultiplyOrder.STR:
        //            Translation += translation / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.RTS:
        //            Translation += _finalRotation.Inverted() * translation;
        //            break;
        //        case Matrix4.MultiplyOrder.SRT:
        //            Translation += (_finalRotation.Inverted() * translation) / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.RST:
        //            Translation += _finalRotation.Inverted() * (translation / Scale);
        //            break;
        //    }
        //}

        public static unsafe FrameState DeriveTRS(Matrix4 m)
        {
            FrameState state = new FrameState();

            float* p = m.Data;
            
            //Translation is easy!
            state._translation = *(Vec3*)&p[12];

            //Scale, use sqrt of rotation columns
            state._scale.X = (float)Math.Round(Math.Sqrt(p[0] * p[0] + p[1] * p[1] + p[2] * p[2]), 4);
            state._scale.Y = (float)Math.Round(Math.Sqrt(p[4] * p[4] + p[5] * p[5] + p[6] * p[6]), 4);
            state._scale.Z = (float)Math.Round(Math.Sqrt(p[8] * p[8] + p[9] * p[9] + p[10] * p[10]), 4);

            float x, y, z, c;

            y = (float)Math.Asin(-p[2]);
            if ((CustomMath.PIf / 2.0f - Math.Abs(y)) < 0.0001f)
            {
                //Gimbal lock, occurs when the y rotation falls on pi/2 or -pi/2
                z = 0.0f;
                if (y > 0)
                    x = (float)Math.Atan2(p[4], p[8]);
                else
                    x = (float)Math.Atan2(p[4], -p[8]);
            }
            else
            {
                c = (float)Math.Cos(y);
                x = (float)Math.Atan2(p[6] / c, p[10] / c);
                z = (float)Math.Atan2(p[1] / c, p[0] / c);

                //180 z/x inverts y, use second option
                if (CustomMath.PIf - Math.Abs(z) < 0.05f)
                {
                    y = CustomMath.PIf - y;
                    c = (float)Math.Cos(y);
                    x = (float)Math.Atan2(p[6] / c, p[10] / c);
                    z = (float)Math.Atan2(p[1] / c, p[0] / c);
                }
            }
            
            state._rotation.PitchYawRoll = CustomMath.RadToDeg(new Vec3(x, y, z));

            state.CreateTransform();
            return state;
        }

        #region Animation
        public void SetRotationRoll(float degreeAngle) { Roll = degreeAngle; }
        public void SetRotationYaw(float degreeAngle) { Yaw = degreeAngle; }
        public void SetRotationPitch(float degreeAngle) { Pitch = degreeAngle; }
        public void AddRotationRoll(float degreeAngle) { Roll += degreeAngle; }
        public void AddRotationYaw(float degreeAngle) { Yaw += degreeAngle; }
        public void AddRotationPitch(float degreeAngle) { Pitch += degreeAngle; }
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
