using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Cylinder")]
    public class CylinderY : Cylinder
    {
        public CylinderY()
            : this(1.0f, 1.0f) { }
        public CylinderY(float radius, float halfHeight) 
            : this(Vec3.Zero, radius, halfHeight) { }
        public CylinderY(EventVec3 center, float radius, float halfHeight)
            : base(center, Vec3.UnitY, radius, halfHeight) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCylinderY.New(Radius, HalfHeight * 2.0f);
        public override TShape HardCopy()
            => new CylinderY(Center, Radius, HalfHeight);
    }
}
