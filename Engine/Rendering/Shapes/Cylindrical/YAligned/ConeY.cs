using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Cone")]
    public class ConeY : Cone
    {
        public ConeY()
            : this(1.0f, 1.0f) { }
        public ConeY(float radius, float height)
            : this(Vec3.Zero, radius, height) { }
        public ConeY(EventVec3 center, float radius, float height)
            : base(center, Vec3.UnitY, radius, height) { }
        public override Shape HardCopy()
            => new ConeY(Center, Radius, Height);
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeY.New(Radius, Height);
    }
}