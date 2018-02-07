using System;
using System.ComponentModel;

namespace TheraEngine.Physics
{
    [FileDef("Rigid Body Construction Info")]
    public class TRigidBodyConstructionInfo : TSettings
    {
        [TSerialize]
        public bool SleepingEnabled { get; set; } = true;
        [TSerialize]
        public bool CollisionEnabled { get; set; } = true;
        [TSerialize]
        public bool SimulatePhysics { get; set; } = true;
        [TSerialize]
        public ushort CollisionGroup { get; set; } = 1;
        [TSerialize]
        public ushort CollidesWith { get; set; } = 0xFFFF;

        [TSerialize]
        public bool UseMotionState { get; set; } = true;
        [TSerialize]
        public Matrix4 CenterOfMassOffset { get; set; } = Matrix4.Identity;
        [TSerialize]
        public Matrix4 InitialWorldTransform { get; set; } = Matrix4.Identity;

        [TSerialize]
        public bool AdditionalDamping { get; set; } = false;
        [TSerialize]
        public float AdditionalDampingFactor { get; set; } = 0.005f;
        [TSerialize]
        public float AdditionalLinearDampingThresholdSqr { get; set; } = 0.01f;
        [TSerialize]
        public float AngularDamping { get; set; } = 0.0f;
        [TSerialize]
        public float AngularSleepingThreshold { get; set; } = 1.0f;
        [TSerialize]
        public float Friction { get; set; } = 0.5f;
        [TSerialize]
        public float LinearDamping { get; set; } = 0.0f;
        [TSerialize]
        public float AdditionalAngularDampingThresholdSqr { get; set; } = 0.01f;
        [TSerialize]
        public float Restitution { get; set; } = 0.0f;
        [TSerialize]
        public float RollingFriction { get; set; } = 0.0f;
        [TSerialize]
        public float LinearSleepingThreshold { get; set; } = 0.8f;
        [TSerialize]
        public float AdditionalAngularDampingFactor { get; set; } = 0.01f;

        /// <summary>
        /// Inertia vector relative to the rigid body's local frame space. 
        /// Auto-calculated when you set CollisionShape or Mass (using both), 
        /// so set after setting those if you want to override.
        /// </summary>
        [Description("Inertia vector relative to the rigid body's local frame space. " +
            "Auto-calculated when you set CollisionShape or Mass (requires both to calculate), " +
            "so set after setting those if you want to override.")]
        [TSerialize]
        public Vec3 LocalInertia { get; set; } = Vec3.Zero;

        /// <summary>
        /// The shape this rigid body will use to collide.
        /// Auto-calculates LocalIntertia for you using Mass and the given shape.
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
        /// <summary>
        /// The mass of this rigid body.
        /// Auto-calculates LocalIntertia for you using CollisionShape and the given mass.
        /// </summary>
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

        [TSerialize(nameof(Mass))]
        private float _mass = 1.0f;
        [TSerialize(nameof(CollisionShape))]
        private TCollisionShape _collisionShape = null;
    }
}
