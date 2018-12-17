using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("X-Aligned Cone")]
    public class ConeX : Cone
    {
        public ConeX()
            : this(1.0f, 1.0f) { }

        public ConeX(float radius, float height)
            : this(BasicTransform.GetIdentity(), radius, height) { }

        public ConeX(BasicTransform transform, float radius, float height)
            : base(transform, Vec3.UnitX, radius, height) { }

        public override Shape HardCopy()
            => new ConeX(Transform, Radius, Height);

        public override TCollisionShape GetCollisionShape()
            => TCollisionConeX.New(Radius, Height);
    }
}
