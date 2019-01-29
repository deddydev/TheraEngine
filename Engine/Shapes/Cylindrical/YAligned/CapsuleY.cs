using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Capsule")]
    public class CapsuleY : Capsule
    {
        public CapsuleY()
            : this(1.0f, 1.0f) { }
        public CapsuleY(float radius, float halfHeight)
            : this(Vec3.Zero, radius, halfHeight) { }
        public CapsuleY(EventVec3 center, float radius, float halfHeight)
            : base(center, Vec3.UnitY, radius, halfHeight) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCapsuleY.New(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleY(Center, Radius, HalfHeight);
    }
}
