using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Z-Aligned Cone")]
    public class ConeZ : Cone
    {
        public ConeZ()
            : this(1.0f, 1.0f) { }

        public ConeZ(float radius, float height)
            : this(Transform.GetIdentity(), radius, height) { }

        public ConeZ(Transform transform, float radius, float height)
            : base(transform, Vec3.UnitZ, radius, height) { }

        public override Shape HardCopy()
            => new ConeZ(Transform, Radius, Height);

        public override TCollisionShape GetCollisionShape()
            => TCollisionConeZ.New(Radius, Height);
    }
}
