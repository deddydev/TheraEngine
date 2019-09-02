using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.ContactTesting;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Physics
{
    public class JitterPhysicsWorld : AbstractPhysicsWorld
    {
        public const uint Seed = 513u;

        public override bool ContactTest(ContactTest result) => throw new NotImplementedException();
        public override Vec3 Gravity
        {
            get => base.Gravity;
            set
            {
                base.Gravity = value;
            }
        }

        public override bool DrawCollisionAABBs
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override bool DrawConstraintLimits
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        public override bool DrawConstraints
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public JitterPhysicsWorld()
        {

        }

        public override void AddCollisionObject(TCollisionObject collision)
        {

        }
        public override void RemoveCollisionObject(TCollisionObject collision)
        {

        }
        public override void StepSimulation(float delta)
        {

        }
        public override void UpdateAabbs()
        {

        }
        protected override void OnUpdateSingleAabb(TCollisionObject collision)
        {

        }
        public override bool RayTrace(RayTrace trace)
        {
            throw new NotImplementedException();
        }
        public override bool ShapeTrace(ShapeTrace trace)
        {
            throw new NotImplementedException();
        }
        public override void AddConstraint(TConstraint constraint)
        {

        }
        public override void RemoveConstraint(TConstraint constraint)
        {

        }
        public override void Dispose()
        {

        }

        public override void DrawDebugWorld()
        {

        }
    }
}
