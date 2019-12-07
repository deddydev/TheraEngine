using TheraEngine.Core.Files;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using System;

namespace TheraEngine.Core.Maths.Transforms
{
    public enum ETransformOrder
    {
        TRS,
        TSR,
        RST,
        RTS,
        STR,
        SRT,
    }
    public interface ITransform : IFileObject
    {
        event MatrixChange MatrixChanged;

        Matrix4 Matrix { get; set; }
        Matrix4 InverseMatrix { get; set; }

        EventVec3 Translation { get; set; }
        float Yaw { get; set; }
        float Pitch { get; set; }
        float Roll { get; set; }
        EventVec3 Scale { get; set; }
        ETransformOrder TransformationOrder { get; set; }
        ERotationOrder RotationOrder { get; set; }
        Rotator Rotation { get; set; }
        Quat Quaternion { get; set; }

        void CreateTransform();

        void SetAll(Vec3 translate, Rotator rotation, Vec3 scale);
        void SetAll(Vec3 translate, Quat rotation, Vec3 scale);

        void Lookat(Vec3 point);
        void SetForwardVector(Vec3 direction);

        Vec3 GetForwardVector();
        Vec3 GetUpVector();
        Vec3 GetRightVector();
        Matrix4 GetRotationMatrix();

        Transform HardCopy();
    }

    //public delegate void TranslationChange(Vec3 oldTranslation);
    //public delegate void RotationChange(float oldRotation);
    //public delegate void ScaleChange(Vec3 oldScale);
    public delegate void MatrixChange(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix);

    [TFileExt("transform")]
    [TFileDef("Transform")]
    public class Transform : TFileObject, ITransform
    {
        public event MatrixChange MatrixChanged;

        public static Transform GetIdentity(
            ETransformOrder transformationOrder = ETransformOrder.TRS,
            ERotationOrder rotationOrder = ERotationOrder.YPR)
        {
            Transform identity = GetIdentity();
            identity._transformOrder = transformationOrder;
            identity.RotationOrder = rotationOrder;
            return identity;
        }
        public static Transform GetIdentity() => new Transform(Vec3.Zero, Quat.Identity, Vec3.One);
        public Transform()
        {
            _translation = Vec3.Zero;
            _translation.Changed += CreateTransform;

            _quaternion = Quat.Identity;
            _rotation = new Rotator(ERotationOrder.YPR);
            _rotation.Changed += CreateTransform;

            _scale = Vec3.One;
            _scale.Changed += CreateTransform;

            _transformOrder = ETransformOrder.TRS;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
        }
        
        public Transform(
            Vec3 translate, 
            Rotator rotate,
            Vec3 scale,
            ETransformOrder transformOrder = ETransformOrder.TRS)
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
        public Transform(
            Vec3 translate,
            Quat rotate,
            Vec3 scale,
            ETransformOrder transformOrder = ETransformOrder.TRS)
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

        public static ETransformOrder OppositeOrder(ETransformOrder order)
        {
            switch (order)
            {
                case ETransformOrder.TRS: return ETransformOrder.SRT;
                case ETransformOrder.TSR: return ETransformOrder.RST;
                case ETransformOrder.RST: return ETransformOrder.RST;
                case ETransformOrder.RTS: return ETransformOrder.RTS;
                case ETransformOrder.STR: return ETransformOrder.STR;
                case ETransformOrder.SRT: return ETransformOrder.SRT;
            }
            return ETransformOrder.TRS;
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
        [TSerialize("Rotation")]
        private Rotator _rotation;
        [TSerialize("Translation")]
        private EventVec3 _translation;
        [TSerialize("Scale")]
        private EventVec3 _scale;
        [TSerialize("Order", NodeType = ENodeType.Attribute)]
        private ETransformOrder _transformOrder = ETransformOrder.TRS;
        private bool _matrixChanged = false;

        private Matrix4 _transform = Matrix4.Identity;
        private Matrix4 _inverseTransform = Matrix4.Identity;

        public void Lookat(Vec3 point)
        {
            SetForwardVector(point - _transform.Translation);
        }
        public void SetForwardVector(Vec3 direction)
        {

        }

        public Vec3 GetForwardVector() => _quaternion * Vec3.Forward;
        public Vec3 GetUpVector() => _quaternion * Vec3.Up;
        public Vec3 GetRightVector() => _quaternion * Vec3.Right;
        public Matrix4 GetRotationMatrix() => _rotation.GetMatrix();

        [Browsable(false)]
        public Matrix4 Matrix
        {
            get => _transform;
            set
            {
                _transform = value;
                _inverseTransform = _transform.Inverted();
                _matrixChanged = true;
            }
        }

        [Browsable(false)]
        public Matrix4 InverseMatrix
        {
            get => _inverseTransform;
            set
            {
                _inverseTransform = value;
                _transform = _inverseTransform.Inverted();
                _matrixChanged = true;
            }
        }

        private void MatrixUpdated()
        {
            _matrixChanged = false;
            DeriveTRS(_transform, out Vec3 t, out Vec3 s, out Quat r);
            _translation.SetRawNoUpdate(t);
            _scale.SetRawNoUpdate(s);
            _quaternion = r;
            Rotation.SetRotationsNoUpdate(_quaternion.ToYawPitchRoll());
        }

        [Category("Transform")]
        public EventVec3 Translation
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _translation;
            }
            set
            {
                _translation = value ?? new EventVec3();
                _translation.Changed += CreateTransform;
            }
        }
        [Browsable(false)]
        public float Yaw
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _rotation.Yaw;
            }
            set => _rotation.Yaw = value;
        }
        [Browsable(false)]
        public float Pitch
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _rotation.Pitch;
            }
            set => _rotation.Pitch = value;
        }
        [Browsable(false)]
        public float Roll
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _rotation.Roll;
            }
            set => _rotation.Roll = value;
        }
        [Category("Transform")]
        public EventVec3 Scale
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _scale;
            }
            set
            {
                _scale = value ?? new EventVec3(1.0f);
                _scale.Changed += CreateTransform;
            }
        }
        [Category("Transform")]
        public ETransformOrder TransformationOrder
        {
            get => _transformOrder;
            set
            {
                _transformOrder = value;
                CreateTransform();
            }
        }
        [Category("Transform")]
        public ERotationOrder RotationOrder
        {
            get => _rotation.Order;
            set => _rotation.Order = value;
        }
        [Category("Transform")]
        public Rotator Rotation
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _rotation;
            }
            set
            {
                _rotation = value ?? new Rotator();
                _rotation.Changed += CreateTransform;
            }
        }
        [Browsable(false)]
        public Quat Quaternion
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _quaternion;
            }
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

        [TPostDeserialize]
        public void CreateTransform()
        {
            Matrix4 oldMatrix = _transform;
            Matrix4 oldInvMatrix = _inverseTransform;

            _quaternion = _rotation.ToQuaternion();
            _transform = Matrix4.TransformMatrix(_scale, _rotation, _translation, _transformOrder);
            _inverseTransform = Matrix4.InverseTransformMatrix(_scale, _rotation, _translation, _transformOrder);

            MatrixChanged?.Invoke(this, oldMatrix, oldInvMatrix);
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

        /// <summary>
        /// Converts this transform's matrix back into its translation, rotation and scale components.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="translation"></param>
        /// <param name="scale"></param>
        /// <param name="rotation"></param>
        public static void DeriveTRS(Matrix4 m, out Vec3 translation, out Vec3 scale, out Quat rotation)
        {
            translation = m.Row3.Xyz;
            scale = new Vec3(m.Row0.Xyz.Length, m.Row1.Xyz.Length, m.Row2.Xyz.Length);
            rotation = m.ExtractRotation(true);
            //translation.Round(5);
            //scale.Round(5);
        }
        public static void DeriveTR(Matrix4 m, out Vec3 translation, out Quat rotation)
        {
            translation = m.Row3.Xyz;
            rotation = m.ExtractRotation(true);
        }
        public static void DeriveT(Matrix4 m, out Vec3 translation)
        {
            translation = m.Row3.Xyz;
        }
        public static unsafe Transform DeriveTRS(Matrix4 m)
        {
            Transform state = new Transform()
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

        //#region Animation
        //public void SetRotationRoll(float degreeAngle) { Roll = degreeAngle; }
        //public void SetRotationYaw(float degreeAngle) { Yaw = degreeAngle; }
        //public void SetRotationPitch(float degreeAngle) { Pitch = degreeAngle; }
        //public void AddRotationRoll(float degreeAngle) { Roll += degreeAngle; }
        //public void AddRotationYaw(float degreeAngle) { Yaw += degreeAngle; }
        //public void AddRotationPitch(float degreeAngle) { Pitch += degreeAngle; }
        //public void SetTranslationZ(float value)
        //{
        //    _translation.Z = value;
        //}
        //public void SetTranslationY(float value)
        //{
        //    _translation.Y = value;
        //}
        //public void SetTranslationX(float value)
        //{
        //    _translation.X = value;
        //}
        //public void AddTranslationZ(float value)
        //{
        //    _translation.Z += value;
        //}
        //public void AddTranslationY(float value)
        //{
        //    _translation.Y += value;
        //}
        //public void AddTranslationX(float value)
        //{
        //    _translation.X += value;
        //}
        //public void MultTranslationZ(float value)
        //{
        //    _translation.Z *= value;
        //}
        //public void MultTranslationY(float value)
        //{
        //    _translation.Y *= value;
        //}
        //public void MultTranslationX(float value)
        //{
        //    _translation.X *= value;
        //}
        //public void SetScaleZ(float value)
        //{
        //    _scale.Z = value;
        //}
        //public void SetScaleY(float value)
        //{
        //    _scale.Y = value;
        //}
        //public void SetScaleX(float value)
        //{
        //    _scale.X = value;
        //}
        //public void AddScaleZ(float value)
        //{
        //    _scale.Z += value;
        //}
        //public void AddScaleY(float value)
        //{
        //    _scale.Y += value;
        //}
        //public void AddScaleX(float value)
        //{
        //    _scale.X += value;
        //}
        //public void MultScaleZ(float value)
        //{
        //    _scale.Z *= value;
        //}
        //public void MultScaleY(float value)
        //{
        //    _scale.Y *= value;
        //}
        //public void MultScaleX(float value)
        //{
        //    _scale.X *= value;
        //}
        //#endregion

        public Transform HardCopy()
            => new Transform(Translation, Rotation, Scale, TransformationOrder);
    }
}
