﻿using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Components.Scene.Transforms
{
    [TFileDef("Rotation Component")]
    public class RotationComponent : SceneComponent
    {
        public RotationComponent() : this(Rotator.GetZero(), true) { }
        public RotationComponent(Rotator rotation, bool deferLocalRecalc = false) : base()
        {
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }
        public RotationComponent(ERotationOrder order, bool deferLocalRecalc = false)
            : this(Rotator.GetZero(order), deferLocalRecalc) { }
        public RotationComponent(float pitch, float yaw, float roll, ERotationOrder order = ERotationOrder.YPR, bool deferLocalRecalc = false)
              : this(new Rotator(pitch, yaw, roll, order), deferLocalRecalc) { }
        public RotationComponent(Vec3 pitchYawRoll, ERotationOrder order = ERotationOrder.YPR, bool deferLocalRecalc = false)
             : this(new Rotator(pitchYawRoll, order), deferLocalRecalc) { }

        [TSerialize(nameof(Rotation), UseCategory = true, OverrideCategory = "Transform")]
        protected Rotator _rotation;
        
        [Category("Transform")]
        public Rotator Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value ?? new Rotator();
                _rotation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        [TPostDeserialize]
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
        
        public override bool IsRotatable => true;
        public override void HandleRotation(Quat delta)
        {
            _rotation.SetRotations(delta.ToRotator());
        }
    }
}
