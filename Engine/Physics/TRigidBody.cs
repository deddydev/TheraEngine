using System;

namespace TheraEngine.Physics
{
    public enum EBodyActivationState
    {
        Active = 1,
        Sleeping = 2,
        WantsSleep = 3,
        DisableSleep = 4,
        DisableSimulation = 5
    }
    public abstract class TRigidBody : TCollisionObject
    {
        public TRigidBody(TCollisionShape shape) : base(shape)
        {
            
        }

        public static TRigidBody New(TRigidBodyConstructionInfo info)
            => Engine.Physics.NewRigidBody(info);

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 TotalTorque { get; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 TotalForce { get; }
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Quat Orientation { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract int NumConstraintRefs { get; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 LocalInertia { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 LinearVelocity { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float LinearSleepingThreshold { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 LinearFactor { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float LinearDamping { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool IsInWorld { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float Mass { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Matrix4 InvInertiaTensorWorld { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 InvInertiaDiagLocal { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 Gravity { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract int FrictionSolverType { get; set; }
        
        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract int ContactSolverType { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Matrix4 CenterOfMassTransform { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 CenterOfMassPosition { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 AngularVelocity { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float AngularSleepingThreshold { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract bool WantsSleeping { get; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract Vec3 AngularFactor { get; set; }

        [PhysicsSupport(PhysicsLibrary.Bullet)]
        public abstract float AngularDamping { get; }
    }
}
