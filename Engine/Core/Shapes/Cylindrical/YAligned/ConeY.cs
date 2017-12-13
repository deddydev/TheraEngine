using System.ComponentModel;
using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    [FileClass("SHAPE", "Y-Aligned Cone")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ConeY : BaseCone
    {
        public ConeY(float radius, float height) 
            : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.UnitY, radius, height) { }
        public override CollisionShape GetCollisionShape()
            => new ConeShape(Radius, Height);
    }
}
