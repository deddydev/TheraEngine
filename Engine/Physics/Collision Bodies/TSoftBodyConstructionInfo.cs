using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    /// <summary>
    /// Contains parameters for constructing a new soft body.
    /// </summary>
    [TFileDef("Soft Body Construction Info", "Contains parameters for constructing a new soft body.")]
    public class TSoftBodyConstructionInfo : ICollisionObjectConstructionInfo
    {
        public bool CollisionEnabled = true;
        public bool SimulatePhysics = true;

        public ETheraCollisionGroup CollisionGroup = ETheraCollisionGroup.Default;
        public ETheraCollisionGroup CollidesWith = ETheraCollisionGroup.All;
        
        public float WaterOffset { get; set; }
        public Vec3 WaterNormal { get; set; }
        public float WaterDensity { get; set; }
        //public SparseSdf SparseSdf { get; }
        public float MaxDisplacement { get; set; }
        public Vec3 Gravity { get; set; }
        public float AirDensity { get; set; }

        public TCollisionShape CollisionShape { get; set; }
        public Matrix4 InitialWorldTransform { get; set; }
    }
}
