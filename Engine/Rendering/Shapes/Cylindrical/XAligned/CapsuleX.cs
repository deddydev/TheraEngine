using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("X-Aligned Capsule")]
    public class CapsuleX : Capsule
    {
        public CapsuleX()
            : this(1.0f, 1.0f) { }
        public CapsuleX(float radius, float halfHeight)
            : this(Vec3.Zero, radius, halfHeight) { }
        public CapsuleX(EventVec3 center, float radius, float halfHeight) 
            : base(center, Vec3.UnitX, radius, halfHeight) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCapsuleX.New(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleX(Center, Radius, HalfHeight);
    }
}
