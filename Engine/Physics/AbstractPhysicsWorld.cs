using System;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Physics
{
    public abstract class AbstractPhysicsWorld : IDisposable
    {
        public virtual Vec3 Gravity { get; set; } = new Vec3(0.0f, -9.81f, 0.0f);
        public bool AllowIndividualAabbUpdates { get; set; }

        public abstract void StepSimulation(float delta);
        public abstract void AddCollisionObject(TCollisionObject collision);
        public abstract void RemoveCollisionObject(TCollisionObject collision);
        public abstract void AddConstraint(TConstraint constraint);
        public abstract void RemoveConstraint(TConstraint constraint);
        public abstract bool RayTrace(RayTrace result);
        public abstract bool ShapeTrace(ShapeTrace result);
        public abstract void UpdateAabbs();
        public void UpdateSingleAabb(TCollisionObject collision)
        {
            if (AllowIndividualAabbUpdates)
                OnUpdateSingleAabb(collision);
        }
        protected abstract void OnUpdateSingleAabb(TCollisionObject collision);

        public abstract void Dispose();
    }
}
