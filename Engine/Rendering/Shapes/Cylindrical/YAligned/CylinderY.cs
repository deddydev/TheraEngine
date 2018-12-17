using static System.Math;
using System.ComponentModel;
using System;
using TheraEngine.Physics;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Cylinder")]
    public class CylinderY : Cylinder
    {
        public CylinderY() 
            : this(1.0f, 1.0f) { }

        public CylinderY(float radius, float halfHeight) 
            : this(BasicTransform.GetIdentity(), radius, halfHeight) { }

        public CylinderY(BasicTransform transform, float radius, float halfHeight)
            : base(transform, Vec3.UnitY, radius, halfHeight) { }

        public override TCollisionShape GetCollisionShape()
            => TCollisionCylinderY.New(Radius, HalfHeight * 2.0f);

        public override Shape HardCopy()
            => new CylinderY(Transform, Radius, HalfHeight);
    }
}
