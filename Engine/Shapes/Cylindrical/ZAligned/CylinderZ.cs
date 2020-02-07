using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Z-Aligned Cylinder")]
    public class CylinderZ : Cylinder
    {
        public CylinderZ()
            : this(1.0f, 1.0f) { }
        public CylinderZ(float radius, float halfHeight) 
            : this(Vec3.Zero, radius, halfHeight) { }
        public CylinderZ(EventVec3 center, float radius, float halfHeight)
            : base(center, Vec3.UnitZ, radius, halfHeight) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCylinderZ.New(Radius, HalfHeight * 2.0f);
        public override TShape HardCopy()
            => new CylinderZ(Center, Radius, HalfHeight);
    }
}
