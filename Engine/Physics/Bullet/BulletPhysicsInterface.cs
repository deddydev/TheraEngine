using BulletSharp;
using BulletSharp.SoftBody;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics.Bullet
{
    public class BulletPhysicsInterface : AbstractPhysicsInterface
    {
        public override AbstractPhysicsWorld NewScene() 
            => new BulletPhysicsWorld();
        public override TCollisionBox NewBox(Vec3 halfExtents)
            => new BulletBox(halfExtents);
        public override TCollisionSphere NewSphere(float radius)
            => new BulletSphere(radius);

        public override TRigidBody NewRigidBody(IRigidCollidable owner, TRigidBodyConstructionInfo info)
        {
            TBulletMotionState state = null;

            if (info.UseMotionState)
                state = new TBulletMotionState(info.InitialWorldTransform, info.CenterOfMassOffset);

            RigidBodyConstructionInfo bulletInfo = new RigidBodyConstructionInfo(
                info.Mass, state, ((IBulletShape)info.CollisionShape).Shape, info.LocalInertia)
            {
                AdditionalDamping = info.AdditionalDamping,
                AdditionalDampingFactor = info.AdditionalDampingFactor,
                AdditionalLinearDampingThresholdSqr = info.AdditionalLinearDampingThresholdSqr,
                AngularDamping = info.AngularDamping,
                AngularSleepingThreshold = info.AngularSleepingThreshold,
                Friction = info.Friction,
                LinearDamping = info.LinearDamping,
                AdditionalAngularDampingThresholdSqr = info.AdditionalAngularDampingThresholdSqr,
                Restitution = info.Restitution,
                RollingFriction = info.RollingFriction,
                StartWorldTransform = info.InitialWorldTransform,
                LinearSleepingThreshold = info.LinearSleepingThreshold,
                AdditionalAngularDampingFactor = info.AdditionalAngularDampingFactor,
            };

            BulletRigidBody rigidBody = new BulletRigidBody(owner, bulletInfo, info.CollisionShape);
            rigidBody.CollisionGroup = info.CollisionGroup;
            rigidBody.CollidesWith = info.CollidesWith;
            rigidBody.SimulatingPhysics = info.SimulatePhysics;
            rigidBody.CollisionEnabled = info.CollisionEnabled;
            rigidBody.SleepingEnabled = info.SleepingEnabled;

            if (state != null)
                state.Body = rigidBody;

            return rigidBody;
        }

        public override TSoftBody NewSoftBody(TSoftBodyConstructionInfo info)
        {
            SoftBodyWorldInfo bulletInfo = new SoftBodyWorldInfo()
            {
                AirDensity = info.AirDensity,
                Gravity = info.Gravity,
                MaxDisplacement = info.MaxDisplacement,
                WaterDensity = info.WaterDensity,
                WaterNormal = info.WaterNormal,
                WaterOffset = info.WaterOffset,
            };

            BulletSoftBody softBody = new BulletSoftBody(bulletInfo);
            
            return softBody;
        }
    }
}
