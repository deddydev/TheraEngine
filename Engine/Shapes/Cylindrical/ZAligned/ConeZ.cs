using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Z-Aligned Cone")]
    public class ConeZ : Cone
    {
        [Browsable(false)]
        public override Vec3 UpAxis => base.UpAxis;
        public ConeZ()
            : this(1.0f, 1.0f) { }
        public ConeZ(float radius, float height)
            : this(Vec3.Zero, radius, height) { }
        public ConeZ(EventVec3 center, float radius, float height)
            : base(center, Vec3.UnitX, radius, height) { }
        public override TShape HardCopy()
            => new ConeZ(Center, Radius, Height);
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeZ.New(Radius, Height);
    }
}
