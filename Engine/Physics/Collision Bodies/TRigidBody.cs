using System;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;

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
    public delegate void DelOnHit(TRigidBody me, TRigidBody other, TContactInfo collisionPoint);
    public abstract class TRigidBody : TCollisionObject
    {
        protected TRigidBody() : base() { }

        public new IRigidBodyCollidable Owner
        {
            get => (IRigidBodyCollidable)base.Owner;
            set
            {
                base.Owner = value;
                if (_simulatingPhysics && value != null)
                    WorldTransform = value.WorldMatrix;
            }
        }

        public static TRigidBody New(TRigidBodyConstructionInfo info)
            => Engine.Physics.NewRigidBody(info);

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 TotalForce { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 TotalTorque { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Quat Orientation { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 LocalInertia { get; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract int ConstraintCount { get; }

        protected Vec3 _previousLinearFactor = Vec3.One;
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 LinearFactor { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 LinearVelocity { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float LinearSleepingThreshold { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float LinearDamping { get; }

        protected Vec3 _previousAngularFactor = Vec3.One;
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AngularFactor { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 AngularVelocity { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float AngularSleepingThreshold { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float AngularDamping { get; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool IsInWorld { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract bool WantsSleeping { get; }

        protected ushort _previousCollidesWith = 0xFFFF;
        private bool _collisionEnabled;
        public bool CollisionEnabled
        {
            get => _collisionEnabled;
            set
            {
                if (_collisionEnabled == value)
                    return;

                _collisionEnabled = value;

                HasContactResponse = _collisionEnabled;

                if (_collisionEnabled)
                    CollidesWith = _previousCollidesWith;
                else
                {
                    _previousCollidesWith = CollidesWith;
                    CollidesWith = 0;
                }
            }
        }

        private bool _sleepingEnabled;
        public bool SleepingEnabled
        {
            get => _sleepingEnabled;
            set
            {
                _sleepingEnabled = value;
                if (_sleepingEnabled)
                {
                    if (ActivationState == EBodyActivationState.DisableSleep)
                        ActivationState = EBodyActivationState.WantsSleep;
                }
                else
                {
                    if (ActivationState != EBodyActivationState.DisableSimulation)
                        ActivationState = EBodyActivationState.DisableSleep;
                }
            }
        }

        private bool _simulatingPhysics = false;
        public bool SimulatingPhysics
        {
            get => _simulatingPhysics;
            set
            {
                _simulatingPhysics = value;
                if (!_simulatingPhysics)
                {
                    _previousLinearFactor = LinearFactor;
                    _previousAngularFactor = AngularFactor;
                    IsStatic = true;
                    LinearFactor = 0.0f;
                    AngularFactor = 0.0f;
                    ActivationState = EBodyActivationState.DisableSimulation;
                }
                else
                {
                    IsStatic = false;
                    LinearFactor = _previousLinearFactor;
                    AngularFactor = _previousAngularFactor;
                    WorldTransform = Owner?.WorldMatrix ?? Matrix4.Identity;
                    ActivationState = SleepingEnabled ? EBodyActivationState.Active : EBodyActivationState.DisableSleep;
                }
            }
        }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 Gravity { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract float Mass { get; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Matrix4 InvInertiaTensorWorld { get; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 InvInertiaDiagLocal { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract int FrictionSolverType { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract int ContactSolverType { get; set; }

        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Matrix4 CenterOfMassTransform { get; set; }
        [PhysicsSupport(EPhysicsLibrary.Bullet)]
        public abstract Vec3 CenterOfMassPosition { get; }

        public Vec3 Weight => Mass * Gravity;

        public EventList<TConstraint> Constraints { get; } = new EventList<TConstraint>();

        /// <summary>
        /// Applies a force to the center of mass of the body (the origin).
        /// </summary>
        /// <param name="force">The force to apply, in Newtons.</param>
        public abstract void ApplyCentralForce(Vec3 force);
        /// <summary>
        /// Applies an impulse to the center of mass of the body (the origin).
        /// </summary>
        /// <param name="impulse">The impulse (Force * delta sec) to apply, in Newton-seconds.</param>
        public abstract void ApplyCentralImpulse(Vec3 impulse);
        /// <summary>
        /// Applies a force to the body at a position relative to the body's center of mass (the origin).
        /// </summary>
        /// <param name="force">The force to apply, in Newtons.</param>
        /// <param name="relativePosition">An offset relative to the body's origin.</param>
        public abstract void ApplyForce(Vec3 force, Vec3 relativePosition);
        /// <summary>
        /// Applies an impulse to the body at a position relative to the body's center of mass (the origin).
        /// </summary>
        /// <param name="impulse">The impulse (Force * delta sec) to apply, in Newton-seconds.</param>
        /// <param name="relativePosition">An offset relative to the body's origin.</param>
        public abstract void ApplyImpulse(Vec3 impulse, Vec3 relativePosition);
        public abstract void ApplyTorque(Vec3 torque);
        public abstract void ApplyTorqueImpulse(Vec3 torque);
        public abstract void ClearForces();
        
        public abstract void GetAabb(out Vec3 aabbMin, out Vec3 aabbMax);
        public abstract Vec3 GetVelocityInLocalPoint(Vec3 relativePosition);
        public abstract void ProceedToTransform(Matrix4 newTrans);
        public abstract void SetDamping(float linearDamping, float angularDamping);
        public abstract void SetMassProps(float mass, Vec3 inertia);
        public abstract void SetSleepingThresholds(float linear, float angular);
        public abstract void Translate(Vec3 v);
    }
}
