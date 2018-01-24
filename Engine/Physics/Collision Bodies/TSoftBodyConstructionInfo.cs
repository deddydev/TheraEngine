using System;

namespace TheraEngine.Physics
{
    public class TSoftBodyConstructionInfo
    {
        public bool CollisionEnabled = true;
        public bool SimulatePhysics = true;
        public TCollisionGroup CollisionGroup = TCollisionGroup.Default;
        public TCollisionGroup CollidesWith = TCollisionGroup.All;
        
        public float WaterOffset { get; set; }
        public Vec3 WaterNormal { get; set; }
        public float WaterDensity { get; set; }
        //public SparseSdf SparseSdf { get; }
        public float MaxDisplacement { get; set; }
        public Vec3 Gravity { get; set; }
        public float AirDensity { get; set; }
    }
}
