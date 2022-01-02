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

        EventMatrix4 Matrix { get; }
        EventMatrix4 InverseMatrix { get; }

        ETransformOrder TransformationOrder { get; set; }
        EventVec3 Translation { get; }
        EventQuat Rotation { get; }
        EventVec3 Scale { get; }
        
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
            _matrix = new EventMatrix4(Matrix4.Identity);
            _invMatrix = new EventMatrix4(Matrix4.Identity);
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
            _rotation.Changed += CreateTransform;

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

        public bool IgnoreCreateTransform { get; private set; } = false;

        public void SetAll(Vec3 translate, Quat rotation, Vec3 scale)
        {
            IgnoreCreateTransform = true;
            _translation.Value = translate;
            _scale.Value = scale;
            _rotation.Value = rotation;
            IgnoreCreateTransform = false;
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

        public void Lookat(Vec3 point) => SetForwardVector(point - _matrix.Value.Translation);
        public void SetForwardVector(Vec3 direction) => Rotation.Value = direction.LookatAngles().ToQuaternion();

        public Vec3 GetForwardVector()
            => _rotation.Value * Vec3.Forward;
        public Vec3 GetUpVector()
            => _rotation.Value * Vec3.Up;
        public Vec3 GetRightVector()
            => _rotation.Value * Vec3.Right;

        public Matrix4 GetRotationMatrix()
            => Matrix4.CreateFromQuaternion(_rotation.Value);

        private EventMatrix4 _matrix = new EventMatrix4(Matrix4.Identity);
        [Browsable(false)]
        public EventMatrix4 Matrix
        {
            get => _matrix;
            set
            {
                _matrix = value;
                _invMatrix.Value = _matrix.Value.Inverted();
                _matrixChanged = true;
            }
        }

        private EventMatrix4 _invMatrix = new EventMatrix4(Matrix4.Identity);
        [Browsable(false)]
        public EventMatrix4 InverseMatrix
        {
            get => _invMatrix;
            set
            {
                _invMatrix = value;
                _matrix.Value = _invMatrix.Value.Inverted();
                _matrixChanged = true;
            }
        }

        private void MatrixUpdated()
        {
            _matrix.Value.Derive(out Vec3 t, out Vec3 s, out Quat r);
            IgnoreCreateTransform = true;
            _translation.Value = t;
            _scale.Value = s;
            _rotation.Value = r;
            IgnoreCreateTransform = false;
            _matrixChanged = false;
            CreateTransform();
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
            set => Set(ref _translation, value ?? new EventVec3(),
                () => _translation.Changed -= CreateTransform,
                () => _translation.Changed += CreateTransform);
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
            set => Set(ref _scale, value ?? new EventVec3(1.0f),
                () => _scale.Changed -= CreateTransform,
                () => _scale.Changed += CreateTransform);
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
        public EventQuat Rotation
        {
            get
            {
                if (_matrixChanged)
                    MatrixUpdated();
                return _rotation;
            }
            set => Set(ref _rotation, value ?? new EventQuat(),
                () => _rotation.Changed -= CreateTransform,
                () => _rotation.Changed += CreateTransform);
        }
        [TPostDeserialize]
        public void CreateTransform()
        {
            if (IgnoreCreateTransform || _matrixChanged)
                return;

            Matrix4 oldMatrix = _matrix.Value;
            Matrix4 oldInvMatrix = _invMatrix.Value;

            _matrix.Value = Matrix4.TransformMatrix(_scale.Value, _rotation.Value, _translation.Value, _transformOrder);
            _invMatrix.Value = Matrix4.InverseTransformMatrix(_scale.Value, _rotation.Value, _translation.Value, _transformOrder);

            MatrixChanged?.Invoke(this, oldMatrix, oldInvMatrix);
        }

        public void TranslateRelative(float x, float y, float z)
            => TranslateRelative(new Vec3(x, y, z));
        public void TranslateRelative(Vec3 translation)
        {
            _matrix.Value = Matrix.Value * translation.AsTranslationMatrix();
            //Setting translation's value will run CreateTransform
            _translation.Value = _matrix.Value.Translation;
        }
        public TTransform HardCopy()
            => new TTransform(Translation.Value, Rotation.Value, Scale.Value, TransformationOrder);
    }
}
