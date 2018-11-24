using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    [TFileDef("Rigid Body Construction Info")]
    public class TRigidBodyConstructionInfo : TSettings
    {
        public TRigidBodyConstructionInfo() { }
        public TRigidBodyConstructionInfo(
            TCollisionShape shape,
            float mass,
            Vec3? localIntertia,
            bool useMotionState,
            ushort collisionGroup,
            ushort collidesWith,
            bool collisionEnabled,
            bool simulatePhysics,
            bool sleepingEnabled,
            float deactivationTime)
        {
            CollisionShape = shape;
            Mass = mass;
            LocalInertia = localIntertia ?? shape?.CalculateLocalInertia(mass) ?? Vec3.Zero;
            UseMotionState = useMotionState;
            CollisionGroup = collisionGroup;
            CollidesWith = collidesWith;
            CollisionEnabled = collisionEnabled;
            SimulatePhysics = simulatePhysics;
            SleepingEnabled = sleepingEnabled;
            DeactivationTime = deactivationTime;
        }

        [TSerialize]
        public bool SleepingEnabled { get; set; } = true;
        [TSerialize]
        public bool CollisionEnabled { get; set; } = true;
        [TSerialize]
        public bool SimulatePhysics { get; set; } = true;

        /// <summary>
        /// Use <see cref="TCollisionGroup"/> or your own enum if you want. Note that the enum must be flags.
        /// </summary>
        [TSerialize]
        public ushort CollisionGroup { get; set; } = 1;
        /// <summary>
        /// Use <see cref="TCollisionGroup"/> or your own enum if you want. Note that the enum must be flags.
        /// </summary>
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
        [TSerialize]
        public bool IsKinematic { get; set; } = false;
        [TSerialize]
        public bool CustomMaterialCallback { get; set; } = true;
        [TSerialize]
        public float CcdMotionThreshold { get; set; } = 0.0f;
        [TSerialize]
        public float DeactivationTime { get; set; } = 0.0f;
        [TSerialize]
        public float CcdSweptSphereRadius { get; set; } = 0.0f;
        [TSerialize]
        public float ContactProcessingThreshold { get; set; } = 0.0f;

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
