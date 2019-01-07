using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Z-Aligned Capsule")]
    public class CapsuleZ : Capsule
    {
        public CapsuleZ()
            : this(1.0f, 1.0f) { }
        public CapsuleZ(float radius, float halfHeight)
            : this(Vec3.Zero, radius, halfHeight) { }
        public CapsuleZ(EventVec3 center, float radius, float halfHeight)
            : base(center, Vec3.UnitZ, radius, halfHeight) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCapsuleZ.New(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleZ(Center, Radius, HalfHeight);
    }
}
