using System.ComponentModel;
using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [FileClass("SHAPE", "Z-Aligned Cone")]
    public class ConeZ : BaseCone
    {
        public ConeZ(float radius, float height) 
            : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.UnitZ, radius, height) { }
        public override TCollisionShape GetCollisionShape()
            => TCollisionConeZ.New(Radius, Height);
    }
}
