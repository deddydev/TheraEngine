using TheraEngine.Files;
using System.ComponentModel;

namespace System
{
    public enum TransformOrder
    {
        TRS,
        TSR,
        RST,
        RTS,
        STR,
        SRT,
    }
    [FileClass("TSFM", "Transform")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class FrameState : FileObject
    {
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(float oldRotation);
        public delegate void ScaleChange(Vec3 oldScale);
        public delegate void MatrixChange(Matrix4 oldMatrix, Matrix4 oldInvMatrix);

        public static FrameState GetIdentity(
            TransformOrder transformationOrder = TransformOrder.TRS,
            RotationOrder rotationOrder = RotationOrder.YPR)
        {
            FrameState identity = GetIdentity();
            identity._transformOrder = transformationOrder;
            identity.RotationOrder = rotationOrder;
            return identity;
        }
        public static FrameState GetIdentity() => new FrameState(Vec3.Zero, Rotator.GetZero(), Vec3.One);
        public FrameState()
        {
            _translation = Vec3.Zero;
            _translation.Changed += CreateTransform;

            _quaternion = Quat.Identity;
            _rotation = new Rotator(RotationOrder.YPR);
            _rotation.Changed += CreateTransform;

            _scale = Vec3.One;
            _scale.Changed += CreateTransform;

            _transformOrder = TransformOrder.TRS;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
        }
        
        public FrameState(
            Vec3 translate, 
            Rotator rotate,
            Vec3 scale,
            TransformOrder transformOrder = TransformOrder.TRS)
        {
            _translation = translate;
            _translation.Changed += CreateTransform;

            _scale = scale;
            _scale.Changed += CreateTransform;

            _rotation = rotate;
            _rotation.Changed += CreateTransform;
            _quaternion = _rotation.ToQuaternion();

            _transformOrder = transformOrder;
            CreateTransform();
        }
        public FrameState(
            Vec3 translate,
            Quat rotate,
            Vec3 scale,
            TransformOrder transformOrder = TransformOrder.TRS)
        {
            _translation = translate;
            _translation.Changed += CreateTransform;

            _scale = scale;
            _scale.Changed += CreateTransform;

            _quaternion = rotate;
            _rotation = _quaternion.ToYawPitchRoll();

            _transformOrder = transformOrder;
            CreateTransform();
        }
        
        public void SetAll(Vec3 translate, Rotator rotation, Vec3 scale)
        {
            _translation.SetRawNoUpdate(translate);
            _scale.SetRawNoUpdate(scale);
            _rotation.SetRotationsNoUpdate(rotation);
            _quaternion = _rotation.ToQuaternion();
            CreateTransform();
        }
        public void SetAll(Vec3 translate, Quat rotation, Vec3 scale)
        {
            _translation.SetRawNoUpdate(translate);
            _scale.SetRawNoUpdate(scale);
            _quaternion = rotation;
            _rotation.SetRotationsNoUpdate(_quaternion.ToYawPitchRoll());
            CreateTransform();
        }

        private Quat _quaternion = Quat.Identity;
        [Serialize("Rotation")]
        private Rotator _rotation;
        [Serialize("Translation")]
        private EventVec3 _translation;
        [Serialize("Scale")]
        private EventVec3 _scale;
        [Serialize("Order", IsXmlAttribute = true)]
        private TransformOrder _transformOrder = TransformOrder.TRS;

        private Matrix4 _transform = Matrix4.Identity;
        private Matrix4 _inverseTransform = Matrix4.Identity;

        public event MatrixChange MatrixChanged;
        public void Lookat(Vec3 point)
        {
            SetForwardVector(point - _transform.GetPoint());
        }
        public void SetForwardVector(Vec3 direction)
        {

        }
        public Vec3 GetForwardVector() => _quaternion * Vec3.Forward;
        public Vec3 GetUpVector() => _quaternion * Vec3.Up;
        public Vec3 GetRightVector() => _quaternion * Vec3.Right;
        public Matrix4 GetRotationMatrix() => _rotation.GetMatrix();

        public Matrix4 Matrix
        {
            get => _transform;
            set
            {
                _transform = value;
                _inverseTransform = _transform.Inverted();
                //DeriveTRS(_transform, out Vec3 t, out Vec3 s, out Quat r);
                //_translation = t;
                //_scale = s;
                //Quaternion = r;
            }
        }
        public Matrix4 InverseMatrix => _inverseTransform;

        public EventVec3 Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                _translation.Changed += CreateTransform;
            }
        }
        public float Yaw
        {
            get => _rotation.Yaw;
            set => _rotation.Yaw = value;
        }
        public float Pitch
        {
            get => _rotation.Pitch;
            set => _rotation.Pitch = value;
        }
        public float Roll
        {
            get => _rotation.Roll;
            set => _rotation.Roll = value;
        }
        public EventVec3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _scale.Changed += CreateTransform;
            }
        }
        
        public TransformOrder TransformationOrder
        {
            get => _transformOrder;
            set
            {
                _transformOrder = value;
                CreateTransform();
            }
        }
        public RotationOrder RotationOrder
        {
            get => _rotation.Order;
            set => _rotation.Order = value;
        }
        public Rotator Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _rotation.Changed += CreateTransform;
            }
        }
        public Quat Quaternion
        {
            get => _quaternion;
            set
            {
                _quaternion = value;
                Rotation.SetRotations(_quaternion.ToYawPitchRoll());
            }
        }

        //private void SetTranslate(Vec3 value)
        //{
        //    Vec3 oldTranslation = _translation;
        //    _translation.Raw = value;
        //    CreateTransform();
        //    TranslationChanged?.Invoke(oldTranslation);
        //}
        //private void SetYaw(float value)
        //{
        //    float oldRotation = _rotation.Yaw;
        //    _rotation.Yaw = value;
        //    CreateTransform();
        //    YawChanged?.Invoke(oldRotation);
        //}
        //private void SetPitch(float value)
        //{
        //    float oldRotation = _rotation.Pitch;
        //    _rotation.Pitch = value;
        //    CreateTransform();
        //    PitchChanged?.Invoke(oldRotation);
        //}
        //private void SetRoll(float value)
        //{
        //    float oldRotation = _rotation.Roll;
        //    _rotation.Roll = value;
        //    CreateTransform();
        //    RollChanged?.Invoke(oldRotation);
        //}
        //private void SetScale(Vec3 value)
        //{
        //    Vec3 oldScale = _scale;
        //    _scale.Raw = value;
        //    CreateTransform();
        //    ScaleChanged?.Invoke(oldScale);
        //}

        [PostDeserialize]
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

        public static void DeriveTRS(Matrix4 m, out Vec3 translation, out Vec3 scale, out Quat rotation)
        {
            translation = m.Row3.Xyz;
            scale = new Vec3(m.Row0.Xyz.Length, m.Row1.Xyz.Length, m.Row2.Xyz.Length);
            rotation = m.ExtractRotation(true);
            translation.Round(5);
            scale.Round(5);
        }
        public static unsafe FrameState DeriveTRS(Matrix4 m)
        {
            FrameState state = new FrameState()
            {
                _translation = m.Row3.Xyz,
                _scale = new Vec3(m.Row0.Xyz.Length, m.Row1.Xyz.Length, m.Row2.Xyz.Length),
                Quaternion = m.ExtractRotation(true)
            };

            //float x, y, z, c;
            //float* p = m.Data;

            ////m.Row0.Xyz = m.Row0.Xyz.Normalized();
            ////m.Row1.Xyz = m.Row1.Xyz.Normalized();
            ////m.Row2.Xyz = m.Row2.Xyz.Normalized();
            ////m.Row3.Xyz = m.Row3.Xyz.Normalized();

            //y = (float)Math.Asin(-p[2]);
            //if ((Math.PI / 2.0f - Math.Abs(y)) < 0.0001f)
            //{
            //    //Gimbal lock, occurs when the y rotation falls on pi/2 or -pi/2
            //    z = 0.0f;
            //    if (y > 0)
            //        x = (float)Math.Atan2(p[4], p[8]);
            //    else
            //        x = (float)Math.Atan2(p[4], -p[8]);
            //}
            //else
            //{
            //    c = (float)Math.Cos(y);
            //    x = (float)Math.Atan2(p[6] / c, p[10] / c);
            //    z = (float)Math.Atan2(p[1] / c, p[0] / c);

            //    //180 z/x inverts y, use second option
            //    if (Math.PI - Math.Abs(z) < 0.05f)
            //    {
            //        y = (float)Math.PI - y;
            //        c = (float)Math.Cos(y);
            //        x = (float)Math.Atan2(p[6] / c, p[10] / c);
            //        z = (float)Math.Atan2(p[1] / c, p[0] / c);
            //    }
            //}

            //state._rotation = new Rotator(CustomMath.RadToDeg(new Vec3(x, y, z)), Rotator.Order.YPR);

            if (state._rotation.Pitch == float.NaN ||
                state._rotation.Yaw == float.NaN ||
                state._rotation.Roll == float.NaN)
                throw new Exception("Something went wrong when deriving rotation values.");

            state._translation.Raw.Round(5);
            state._scale.Raw.Round(5);
            state._rotation.Round(5);
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
            _translation.Z = value;
        }
        public void SetTranslationY(float value)
        {
            _translation.Y = value;
        }
        public void SetTranslationX(float value)
        {
            _translation.X = value;
        }
        public void AddTranslationZ(float value)
        {
            _translation.Z += value;
        }
        public void AddTranslationY(float value)
        {
            _translation.Y += value;
        }
        public void AddTranslationX(float value)
        {
            _translation.X += value;
        }
        public void MultTranslationZ(float value)
        {
            _translation.Z *= value;
        }
        public void MultTranslationY(float value)
        {
            _translation.Y *= value;
        }
        public void MultTranslationX(float value)
        {
            _translation.X *= value;
        }
        public void SetScaleZ(float value)
        {
            _scale.Z = value;
        }
        public void SetScaleY(float value)
        {
            _scale.Y = value;
        }
        public void SetScaleX(float value)
        {
            _scale.X = value;
        }
        public void AddScaleZ(float value)
        {
            _scale.Z += value;
        }
        public void AddScaleY(float value)
        {
            _scale.Y += value;
        }
        public void AddScaleX(float value)
        {
            _scale.X += value;
        }
        public void MultScaleZ(float value)
        {
            _scale.Z *= value;
        }
        public void MultScaleY(float value)
        {
            _scale.Y *= value;
        }
        public void MultScaleX(float value)
        {
            _scale.X *= value;
        }
        #endregion
    }
}
