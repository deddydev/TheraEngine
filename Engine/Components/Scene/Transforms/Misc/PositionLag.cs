using System;
using System.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;

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

        protected internal override void OnOriginRebased(Vec3 newOrigin)
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
        public override void RecalcWorldTransform(bool recalcChildWorldTransformsNow = true)
        {
            _previousWorldMatrix = _worldMatrix;
            _previousInverseWorldMatrix = _inverseWorldMatrix;

            _worldMatrix = _interpPoint.AsTranslationMatrix();
            _inverseWorldMatrix = (-_interpPoint).AsTranslationMatrix();

            OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }
        protected internal void Tick(float delta)
        {
            _delta = delta;
            _currentPoint = _worldMatrix.Translation;
            _destPoint = ParentMatrix.Translation;
            LaggingDistance = _destPoint.DistanceToFast(_currentPoint);

            //if (_laggingDistance > _maxLagDistance)
            //    _interpPoint = CustomMath.InterpLinearTo(_destPoint, _currentPoint, _maxLagDistance / _laggingDistance);
            //else
                _interpPoint = Interp.Lerp(_currentPoint, _destPoint, _delta, InterpSpeed);
            
            RecalcWorldTransform();
        }
        public override void OnSpawned()
        {
            _currentPoint = _worldMatrix.Translation;
            RegisterTick(ETickGroup.DuringPhysics, ETickOrder.Scene, Tick, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.DuringPhysics, ETickOrder.Scene, Tick, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }
    }
}
