using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    public class TSoftBodyConstructionInfo
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
    }
}
