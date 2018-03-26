using BulletSharp;
using BulletSharp.SoftBody;
using System;
using System.IO;
using TheraEngine.Physics.Bullet.Constraints;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    public class BulletPhysicsInterface : AbstractPhysicsInterface
    {
        public override AbstractPhysicsWorld NewScene()  => new BulletPhysicsWorld();
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

            if (state != null)
                state.Body = rigidBody;

            rigidBody.CollisionGroup = info.CollisionGroup;
            rigidBody.CollidesWith = info.CollidesWith;
            rigidBody.SimulatingPhysics = info.SimulatePhysics;
            rigidBody.CollisionEnabled = info.CollisionEnabled;
            rigidBody.SleepingEnabled = info.SleepingEnabled;
            rigidBody.IsKinematic = info.IsKinematic;
            rigidBody.CustomMaterialCallback = info.CustomMaterialCallback;
            rigidBody.CcdMotionThreshold = info.CcdMotionThreshold;
            rigidBody.DeactivationTime = info.DeactivationTime;
            rigidBody.CcdSweptSphereRadius = info.CcdSweptSphereRadius;
            rigidBody.ContactProcessingThreshold = info.ContactProcessingThreshold;

            return rigidBody;
        }
        public override TSoftBody NewSoftBody(ISoftCollidable owner, TSoftBodyConstructionInfo info)
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

            BulletSoftBody softBody = new BulletSoftBody(owner, bulletInfo, null);
            
            return softBody;
        }

        #region Constraints
        public override TPointPointConstraint NewPointPointConstraint(TRigidBody rigidBodyA, TRigidBody rigidBodyB, Vec3 pivotInA, Vec3 pivotInB)
            => new BulletPointPointConstraint((BulletRigidBody)rigidBodyA, (BulletRigidBody)rigidBodyB, pivotInA, pivotInB);
        public override TPointPointConstraint NewPointPointConstraint(TRigidBody rigidBodyA, Vec3 pivotInA)
            => new BulletPointPointConstraint((BulletRigidBody)rigidBodyA, pivotInA);
        #endregion

        #region Shapes
        public override TCollisionBox NewBox(Vec3 halfExtents)
            => new BulletBox(halfExtents);
        public override TCollisionSphere NewSphere(float radius)
            => new BulletSphere(radius);
        public override TCollisionConeX NewConeX(float radius, float height)
            => new BulletConeX(radius, height);
        public override TCollisionConeY NewConeY(float radius, float height)
            => new BulletConeY(radius, height);
        public override TCollisionConeZ NewConeZ(float radius, float height)
            => new BulletConeZ(radius, height);
        public override TCollisionCylinderX NewCylinderX(float radius, float height)
            => new BulletCylinderX(radius, height);
        public override TCollisionCylinderY NewCylinderY(float radius, float height)
            => new BulletCylinderY(radius, height);
        public override TCollisionCylinderZ NewCylinderZ(float radius, float height)
            => new BulletCylinderZ(radius, height);
        public override TCollisionCapsuleX NewCapsuleX(float radius, float height)
            => new BulletCapsuleX(radius, height);
        public override TCollisionCapsuleY NewCapsuleY(float radius, float height)
            => new BulletCapsuleY(radius, height);
        public override TCollisionCapsuleZ NewCapsuleZ(float radius, float height)
            => new BulletCapsuleZ(radius, height);
        public override TCollisionHeightField NewHeightField(int heightStickWidth, int heightStickLength, Stream heightfieldData, float heightScale, float minHeight, float maxHeight, int upAxis, TCollisionHeightField.EHeightValueType heightDataType, bool flipQuadEdges)
            => new BulletHeightField(heightStickLength, heightStickLength, heightfieldData, heightScale, minHeight, maxHeight, upAxis, (PhyScalarType)(int)heightDataType, flipQuadEdges);
        #endregion
    }
}
