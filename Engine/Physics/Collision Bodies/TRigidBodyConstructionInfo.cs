using System;

namespace TheraEngine.Physics
{
    public class TRigidBodyConstructionInfo
    {
        public bool SleepingEnabled = true;
        public bool CollisionEnabled = true;
        public bool SimulatePhysics = true;
        public ushort CollisionGroup = 1;
        public ushort CollidesWith = 0xFFFF;

        private float _mass = 1.0f;
        private TCollisionShape _collisionShape = null;
        public Vec3 LocalInertia = Vec3.Zero;

        public bool UseMotionState = true;
        public Matrix4 CenterOfMassOffset = Matrix4.Identity;
        public Matrix4 InitialWorldTransform = Matrix4.Identity;

        public bool AdditionalDamping = false;
        public float AdditionalDampingFactor = 0.005f;
        public float AdditionalLinearDampingThresholdSqr = 0.01f;
        public float AngularDamping = 0.0f;
        public float AngularSleepingThreshold = 1.0f;
        public float Friction = 0.5f;
        public float LinearDamping = 0.0f;
        public float AdditionalAngularDampingThresholdSqr = 0.01f;
        public float Restitution = 0.0f;
        public float RollingFriction = 0.0f;
        public float LinearSleepingThreshold = 0.8f;
        public float AdditionalAngularDampingFactor = 0.01f;

        /// <summary>
        /// The shape this rigid body will use to collide.
        /// </summary>
        public TCollisionShape CollisionShape
        {
            get => _collisionShape;
            set
            {
                _collisionShape = value;
                if (CollisionShape != null)
                    LocalInertia = CollisionShape.CalculateLocalInertia(Mass);
            }
        }
        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                if (CollisionShape != null)
                    LocalInertia = CollisionShape.CalculateLocalInertia(Mass);
            }
        }
    }
}
