using System.ComponentModel;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [FileDef("X-Aligned Cone")]
    public class ConeX : BaseCone
    {
        public ConeX(float radius, float height) 
            : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.UnitX, radius, height) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeX.New(Radius, Height);
    }
}
