using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("X-Aligned Cone")]
    public class ConeX : Cone
    {
        [Browsable(false)]
        public override Vec3 UpAxis => base.UpAxis;
        public ConeX()
            : this(1.0f, 1.0f) { }
        public ConeX(float radius, float height)
            : this(Vec3.Zero, radius, height) { }
        public ConeX(EventVec3 center, float radius, float height)
            : base(center, Vec3.UnitX, radius, height) { }
        public override TShape HardCopy()
            => new ConeX(Center, Radius, Height);
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeX.New(Radius, Height);
    }
}
