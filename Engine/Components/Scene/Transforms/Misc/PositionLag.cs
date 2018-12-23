﻿using System;
using System.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;

namespace TheraEngine.Components.Scene.Transforms
{
    [TFileDef("Position Lag Component")]
    public class PositionLagComponent : OriginRebasableComponent
    {
        public PositionLagComponent() : this(20.0f, 2.0f) { }
        public PositionLagComponent(float interpSpeed) : this(interpSpeed, 2.0f) { }
        public PositionLagComponent(float interpSpeed, float maxLagDistance) : base()
        {
            InterpSpeed = interpSpeed;
            MaxLagDistance = maxLagDistance;
        }

        private float _delta;
        private Vec3 _currentPoint;
        private Vec3 _destPoint;
        private Vec3 _interpPoint;
        
        [TSerialize]
        [Category("Position Lag Component")]
        public float InterpSpeed { get; set; }
        [TSerialize]
        [Category("Position Lag Component")]
        public float MaxLagDistance { get; set; }
        [Browsable(false)]
        public float LaggingDistance { get; private set; }

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            _currentPoint -= newOrigin;
            _destPoint -= newOrigin;
            _interpPoint -= newOrigin;
            RecalcWorldTransform();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            //Matrix4 worldPosMtx = Matrix4.CreateTranslation(_interpPoint);
            //Matrix4 invWorldPosMtx = Matrix4.CreateTranslation(-_interpPoint);
            //localTransform = worldPosMtx * GetInverseParentMatrix();
            //inverseLocalTransform = GetParentMatrix() * invWorldPosMtx;
            localTransform = Matrix4.Identity;
            inverseLocalTransform = Matrix4.Identity;
        }
        public override void RecalcWorldTransform()
        {
            Matrix4 mtx = _interpPoint.AsTranslationMatrix();
            Matrix4 inv = (-_interpPoint).AsTranslationMatrix();
            Transform.SetWorldMatrices(mtx, inv);
        }
        protected internal void Tick(float delta)
        {
            _delta = delta;
            _currentPoint = Transform.WorldPoint;
            _destPoint = Transform.Parent.WorldPoint;
            LaggingDistance = _destPoint.DistanceToFast(_currentPoint);

            //if (_laggingDistance > _maxLagDistance)
            //    _interpPoint = CustomMath.InterpLinearTo(_destPoint, _currentPoint, _maxLagDistance / _laggingDistance);
            //else
                _interpPoint = Interp.InterpLinearTo(_currentPoint, _destPoint, _delta, InterpSpeed);
            
            RecalcWorldTransform();
        }
        public override void OnSpawned()
        {
            _currentPoint = Transform.WorldPoint;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Scene, Tick, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Scene, Tick, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
    }
}
