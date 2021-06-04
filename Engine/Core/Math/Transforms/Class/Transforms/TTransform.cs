using TheraEngine.Core.Files;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using System;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Mesh;

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

        ETransformOrder TransformationOrder { get; set; }
        EventVec3 Translation { get; set; }
        EventQuat Rotation { get; set; }
        EventVec3 Scale { get; set; }
        
        void CreateTransform();
        void SetAll(Vec3 translate, Quat rotation, Vec3 scale);

        void Lookat(Vec3 point);
        void SetForwardVector(Vec3 direction);

        Vec3 GetForwardVector();
        Vec3 GetUpVector();
        Vec3 GetRightVector();
        Matrix4 GetRotationMatrix();

        TTransform HardCopy();

        //bool IsTranslatable { get; }
        //bool IsScalable { get; }
        //bool IsRotatable { get; }

        //void HandleTranslation(Vec3 delta);
        //void HandleScale(Vec3 delta);
        //void HandleRotation(Quat delta);
    }

    //public delegate void TranslationChange(Vec3 oldTranslation);
    //public delegate void RotationChange(float oldRotation);
    //public delegate void ScaleChange(Vec3 oldScale);
    public delegate void MatrixChange(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix);

    [TFileExt("transform")]
    [TFileDef("Transform")]
    public class TTransform : TFileObject, ITransform
    {
        public event MatrixChange MatrixChanged;

        public static TTransform GetIdentity(ETransformOrder transformationOrder = ETransformOrder.TRS)
        {
            TTransform identity = GetIdentity();
            identity._transformOrder = transformationOrder;
            return identity;
        }
        public static TTransform GetIdentity() => new TTransform(Vec3.Zero, Quat.Identity, Vec3.One);
        public TTransform()
        {
            _translation = Vec3.Zero;
            _translation.Changed += CreateTransform;

            _rotation = Quat.Identity;
            _rotation.Changed += CreateTransform;

            _scale = Vec3.One;
            _scale.Changed += CreateTransform;

            _transformOrder = ETransformOrder.TRS;
            _matrix = Matrix4.Identity;
            _invMatrix = Matrix4.Identity;
        }
        
        public TTransform(
            Vec3 translation,
            Quat rotation,
            Vec3 scale,
            ETransformOrder transformOrder = ETransformOrder.TRS)
        {
            _translation = translation;
            _translation.Changed += CreateTransform;

            _scale = scale;
            _scale.Changed += CreateTransform;

            _rotation = rotation;

            _transformOrder = transformOrder;
            CreateTransform();
        }

        public static ETransformOrder OppositeOrder(ETransformOrder order)
            => order switch
            {
                ETransformOrder.TRS => ETransformOrder.SRT,
                ETransformOrder.TSR => ETransformOrder.RST,
                ETransformOrder.RST => ETransformOrder.RST,
                ETransformOrder.RTS => ETransformOrder.RTS,
                ETransformOrder.STR => ETransformOrder.STR,
                ETransformOrder.SRT => ETransformOrder.SRT,
                _ => ETransformOrder.TRS,
            };

        public void SetAll(Vec3 translate, Quat rotation, Vec3 scale)
        {
            _translation.SetValueSilent(translate);
            _scale.SetValueSilent(scale);
            _rotation.SetValueSilent(rotation);
            CreateTransform();
        }

        [TSerialize("Rotation")]
        private EventQuat _rotation;
        [TSerialize("Translation")]
        private EventVec3 _translation;
        [TSerialize("Scale")]
        private EventVec3 _scale;
        [TSerialize("Order", NodeType = ENodeType.Attribute)]
        private ETransformOrder _transformOrder = ETransformOrder.TRS;
        private bool _matrixChanged = false;

        private Matrix4 _matrix = Matrix4.Identity;
        private Matrix4 _invMatrix = Matrix4.Identity;

        public void Lookat(Vec3 point)
        {
            SetForwardVector(point - _matrix.Translation);
        }
        public void SetForwardVector(Vec3 direction)
        {
            Rotation.Value = direction.LookatAngles().ToQuaternion();
        }

        public Vec3 GetForwardVector()
            => _rotation.Value * Vec3.Forward;
        public Vec3 GetUpVector()
            => _rotation.Value * Vec3.Up;
        public Vec3 GetRightVector()
            => _rotation.Value * Vec3.Right;

        public Matrix4 GetRotationMatrix()
            => Matrix4.CreateFromQuaternion(_rotation.Value);

        [Browsable(false)]
        public Matrix4 Matrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                _invMatrix = _matrix.Inverted();
                _matrixChanged = true;
            }
        }

        [Browsable(false)]
        public Matrix4 InverseMatrix
        {
            get => _invMatrix;
            set
            {
                _invMatrix = value;
                _matrix = _invMatrix.Inverted();
                _matrixChanged = true;
            }
        }

        private void MatrixUpdated()
        {
            _matrixChanged = false;
            _matrix.DeriveTRS(out Vec3 t, out Vec3 s, out Quat r);
            _translation.SetValueSilent(t);
            _scale.SetValueSilent(s);
            _rotation.SetValueSilent(r);
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
        //[Browsable(false)]
        //public float Yaw
        //{
        //    get
        //    {
        //        if (_matrixChanged)
        //            MatrixUpdated();
        //        return _rotation.Yaw;
        //    }
        //    set => _rotation.Yaw = value;
        //}
        //[Browsable(false)]
        //public float Pitch
        //{
        //    get
        //    {
        //        if (_matrixChanged)
        //            MatrixUpdated();
        //        return _rotation.Pitch;
        //    }
        //    set => _rotation.Pitch = value;
        //}
        //[Browsable(false)]
        //public float Roll
        //{
        //    get
        //    {
        //        if (_matrixChanged)
        //            MatrixUpdated();
        //        return _rotation.Roll;
        //    }
        //    set => _rotation.Roll = value;
        //}
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
        //[Category("Transform")]
        //public ERotationOrder RotationOrder
        //{
        //    get => _rotation.Order;
        //    set => _rotation.Order = value;
        //}
        [Category("Transform")]
        public EventQuat Rotation
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _rotation;
            }
            set
            {
                _rotation = value ?? new EventQuat();
                _rotation.Changed += CreateTransform;
            }
        }
        //[Browsable(false)]
        //public Quat Quaternion
        //{
        //    get
        //    {
        //        if (_matrixChanged)
        //            MatrixUpdated();
        //        return _quaternion;
        //    }
        //    set
        //    {
        //        _quaternion = value;
        //        Rotation.SetRotations(_quaternion.ToRotator());
        //    }
        //}

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
            Matrix4 oldMatrix = _matrix;
            Matrix4 oldInvMatrix = _invMatrix;

            _matrix = Matrix4.TransformMatrix(_scale, _rotation.Value, _translation, _transformOrder);
            _invMatrix = Matrix4.InverseTransformMatrix(_scale, _rotation.Value, _translation, _transformOrder);

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

        public void TranslateRelative(float x, float y, float z)
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            //Matrix4 oldMatrix = _transform;
            //Matrix4 oldInvMatrix = _inverseTransform;

            _matrix = Matrix * translation.AsTranslationMatrix();
            //_inverseTransform = (-translation).AsTranslationMatrix() * InverseMatrix;
            //_translation.SetValueSilent(_transform.Translation);

            //MatrixChanged?.Invoke(this, oldMatrix, oldInvMatrix);

            _translation.Value = _matrix.Translation;
        }
        public void Pivot(float pitch, float yaw, float distance)
            => ArcBallRotate(pitch, yaw, _translation + _matrix.ForwardVec * distance);
        public void ArcBallRotate(float pitch, float yaw, Vec3 focusPoint)
        {
            //"Arcball" rotation
            //All rotation is done within local component space
            _translation.Value = TMath.ArcballTranslation(pitch, yaw, focusPoint, _translation.Value, _matrix.RightVec);
            _rotation *= Quat.Euler(pitch, yaw, 0.0f);
        }

        public TTransform HardCopy()
            => new TTransform(Translation.Value, Rotation.Value, Scale.Value, TransformationOrder);
    }
}
