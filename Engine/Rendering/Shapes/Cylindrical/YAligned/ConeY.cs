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
            : this(Transform.GetIdentity(), radius, height) { }
        
        public ConeY(Transform transform, float radius, float height)
            : base(transform, Vec3.UnitY, radius, height) { }
        
        public override Shape HardCopy()
            => new ConeY(Transform, Radius, Height);
        
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeY.New(Radius, Height);
    }
}