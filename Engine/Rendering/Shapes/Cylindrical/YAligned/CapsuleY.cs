using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Y-Aligned Capsule")]
    public class CapsuleY : Capsule
    {
        public CapsuleY()
            : this(1.0f, 1.0f) { }

        public CapsuleY(float radius, float halfHeight)
            : base(Transform.GetIdentity(), Vec3.UnitY, radius, halfHeight) { }

        public CapsuleY(Transform transform, float radius, float halfHeight) 
            : base(transform, Vec3.UnitY, radius, halfHeight) { }

        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _transform.Matrix = worldMatrix;
            base.SetRenderTransform(worldMatrix);
        }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCapsuleY.New(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleY(_transform, Radius, HalfHeight);
        public override Matrix4 GetTransformMatrix()
            => _transform.Matrix;
    }
}
