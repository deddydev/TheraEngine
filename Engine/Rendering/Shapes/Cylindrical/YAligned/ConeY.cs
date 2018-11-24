using System.ComponentModel;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Cone")]
    public class ConeY : BaseCone
    {
        public ConeY(float radius, float height) 
            : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.UnitY, radius, height) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeY.New(Radius, Height);
    }
}