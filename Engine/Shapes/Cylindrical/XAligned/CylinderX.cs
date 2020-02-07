using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("X-Aligned Cylinder")]
    public class CylinderX : Cylinder
    {
        public CylinderX()
            : this(1.0f, 1.0f) { }
        public CylinderX(float radius, float halfHeight) 
            : this(Vec3.Zero, radius, halfHeight) { }
        public CylinderX(EventVec3 center, float radius, float halfHeight)
            : base(center, Vec3.UnitX, radius, halfHeight) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCylinderX.New(Radius, HalfHeight * 2.0f);
        public override TShape HardCopy()
            => new CylinderX(Center, Radius, HalfHeight);
    }
}
