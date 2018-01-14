using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    [FileDef("Rotation Component")]
    public class RotationComponent : SceneComponent
    {
        public RotationComponent() : base()
        {
            Rotation = Rotator.GetZero();
        }
        public RotationComponent(RotationOrder order) : base()
        {
            Rotation = Rotator.GetZero(order);
        }
        public RotationComponent(Rotator rotation)
        {
            Rotation = rotation;
        }
        public RotationComponent(float pitch, float yaw, float roll, RotationOrder order = RotationOrder.YPR) : base()
        {
            Rotation = new Rotator(pitch, yaw, roll, order);
        }
        public RotationComponent(Vec3 pitchYawRoll, RotationOrder order = RotationOrder.YPR) : base()
        {
            Rotation = new Rotator(pitchYawRoll, order);
        }

        [TSerialize("Rotation")]
        protected Rotator _rotation;

        [Category("Transform")]
        public Rotator Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _rotation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        [PostDeserialize]
        protected internal virtual void OnDeserialized()
        {
            _rotation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _rotation.GetMatrix();
            inverseLocalTransform = _rotation.GetInverseMatrix();
        }

#if EDITOR
        public override bool IsRotatable => true;
        public override void HandleWorldRotation(Quat delta)
        {
            delta.ToYawPitchRoll();
        }
#endif

        protected internal override void OriginRebased(Vec3 newOrigin)
        {

        }
    }
}
