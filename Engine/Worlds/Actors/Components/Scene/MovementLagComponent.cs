using System;

namespace TheraEngine.Worlds.Actors
{
    public class MovementLagComponent : TRSComponent
    {
        Vec3 _destPoint;
        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            throw new NotImplementedException();
        }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
        }
        protected internal void Tick(float delta)
        {
            Vec3 currentPoint = _worldTransform.GetPoint();
            Vec3 destPoint = (GetParentMatrix() * LocalMatrix).GetPoint();

            _destPoint = CustomMath.InterpCosineTo(currentPoint, destPoint, delta);
        }
        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Logic, Tick);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Logic, Tick);
            base.OnDespawned();
        }
    }
}
