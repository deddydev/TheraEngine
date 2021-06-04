using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Components.Scene.Transforms
{
    public interface ITransformComponent : ISceneComponent
    {
        TTransform Transform { get; set; }
    }
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [TFileDef("Transform Component")]
    public class TransformComponent : OriginRebasableComponent, ITransformComponent
    {
        public TransformComponent() : this(TTransform.GetIdentity(), true) { }
        public TransformComponent(TTransform transform, bool deferLocalRecalc = false) : base()
        {
            _transform = transform ?? TTransform.GetIdentity();
            _transform.MatrixChanged += _transform_MatrixChanged;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        private void _transform_MatrixChanged(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix)
            => RecalcLocalTransform();

        [TSerialize(nameof(Transform))]
        protected TTransform _transform;
        
        [Category("Transform")]
        public TTransform Transform
        {
            get => _transform;
            set
            {
                if (_transform != null)
                    _transform.MatrixChanged -= _transform_MatrixChanged;
                _transform = value ?? TTransform.GetIdentity();
                _transform.MatrixChanged += _transform_MatrixChanged;
                RecalcLocalTransform();
            }
        }

        [Browsable(false)]
        public override Matrix4 WorldMatrix
        {
            get => base.WorldMatrix;
            set
            {
                base.WorldMatrix = value;
                Transform.Matrix = WorldMatrix * InverseParentWorldMatrix;
            }
        }
        [Browsable(false)]
        public override Matrix4 InverseWorldMatrix
        {
            get => base.InverseWorldMatrix;
            set
            {
                base.InverseWorldMatrix = value;
                Transform.InverseMatrix = ParentWorldMatrix * InverseWorldMatrix; 
            }
        }

        [TPostDeserialize]
        protected internal virtual void OnDeserialized()
        {
            if (_transform is null)
                _transform = TTransform.GetIdentity();
            _transform.MatrixChanged += _transform_MatrixChanged;
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _transform.Matrix;
            inverseLocalTransform = _transform.InverseMatrix;
        }

        /// <summary>
        /// If false, this component will stay at the origin regardless of where it is shifted to.
        /// Otherwise, the component will be moved to appear in the same position despite where the origin is moved to.
        /// Defaults to true.
        /// </summary>
        [Browsable(false)]
        public bool AllowOriginRebase { get; set; } = true;

        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {
            //Engine.PrintLine("Rebasing {0}.", OwningActor.GetType().GetFriendlyName());
            if (AllowOriginRebase)
                HandleTranslation(-newOrigin);
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;

        public Vec3 Translation
        {
            get => Transform.Translation.Value; 
            set => Transform.Translation.Value = value;
        }
        public Quat Rotation
        {
            get => Transform.Rotation.Value;
            set => Transform.Rotation.Value = value;
        }
        public Vec3 Scale
        {
            get => Transform.Scale.Value;
            set => Transform.Scale.Value = value;
        }


        public override void HandleTranslation(Vec3 delta)
            => Transform.Translation += delta;
    }
}
