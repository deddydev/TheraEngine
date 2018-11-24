using static System.Math;
using System.ComponentModel;
using System;
using TheraEngine.Physics;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Cylinder")]
    public class CylinderY : BaseCylinder
    {
        public CylinderY()
               : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.UnitY, 1.0f, 1.0f)
        {

        }
        public CylinderY(Vec3 center, float radius, float halfHeight) 
            : base(center, Rotator.GetZero(), Vec3.One, Vec3.UnitY, radius, halfHeight)
        {

        }
        public override TCollisionShape GetCollisionShape()
        {
            return TCollisionCylinderY.New(Radius, HalfHeight * 2.0f);
        }
        public override Shape HardCopy()
        {
            throw new NotImplementedException();
        }
    }
}
