using System;
using Jitter;
using Jitter.Collision.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics.Jitter.Shapes;

namespace TheraEngine.Physics.Jitter
{
    internal class JitterBox : TCollisionBox, IJitterShape
    {
        public BoxShape Shape { get; }
        Shape IJitterShape.Shape => Shape;

        public override Vec3 HalfExtents => (Vec3)Shape.Size / 2.0f;
        public override float Margin { get => 0.0f; set { } }
        public override Vec3 LocalScaling { get => Vec3.One; set { } }
        
        public JitterBox() : this(0.5f) { }
        public JitterBox(Vec3 halfExtents)
            => Shape = new BoxShape(halfExtents * 2.0f);

        public override void Dispose()
        {
            //Shape.Dispose();
        }

        #region Collision Shape Methods
        public override void GetBoundingSphere(out Vec3 center, out float radius)
        {
            throw new NotImplementedException();
        }
        public override void GetAabb(Matrix4 transform, out Vec3 aabbMin, out Vec3 aabbMax)
        {
            throw new NotImplementedException();
        }
        public override Vec3 CalculateLocalInertia(float mass)
            => throw new NotImplementedException();
        #endregion
    }
}
