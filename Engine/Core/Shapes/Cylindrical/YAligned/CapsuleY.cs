using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [FileClass("SHAPE", "Y-Aligned Capsule")]
    public class CapsuleY : BaseCapsule
    {
        public CapsuleY() : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.Up, 1.0f, 1.0f)
        {

        }

        public CapsuleY(Vec3 center, Rotator rotation, Vec3 scale, float radius, float halfHeight) 
            : base(center, rotation, scale, Vec3.UnitY, radius, halfHeight)
        {

        }
        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _state.Matrix = worldMatrix;
            base.SetRenderTransform(worldMatrix);
        }
        public override TCollisionShape GetCollisionShape()
            => TCollisionCapsuleY.New(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleY(_state.Translation, _state.Rotation, _state.Scale, Radius, HalfHeight);
        public override Shape TransformedBy(Matrix4 worldMatrix)
            => new CapsuleY(worldMatrix.GetPoint(), Rotator.GetZero(), Vec3.One, Radius, HalfHeight);
        public override Matrix4 GetTransformMatrix()
            => _state.Matrix;
    }
}
