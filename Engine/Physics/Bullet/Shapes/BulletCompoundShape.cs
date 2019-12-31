using System;
using BulletSharp;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Physics.Bullet.Shapes
{
    internal class BulletCompoundShape : TCollisionCompoundShape, IBulletShape
    {
        public CompoundShape Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;
        
        public override float Margin
        {
            get => Shape.Margin;
            set => Shape.Margin = value;
        }
        public override Vec3 LocalScaling
        {
            get => Shape.LocalScaling;
            set => Shape.LocalScaling = value;
        }
        
        public BulletCompoundShape() : this(null) { }
        public BulletCompoundShape((Matrix4 localTransform, TCollisionShape shape)[] shapes)
        {
            Shape = new CompoundShape();
            if (shapes != null)
                foreach (var shape in shapes)
                    Shape.AddChildShape(shape.localTransform, ((IBulletShape)shape.shape).Shape);
        }

        public override void Dispose()
        {
            Shape.Dispose();
        }

        #region Collision Shape Methods
        public override void GetBoundingSphere(out Vec3 center, out float radius)
        {
            Shape.GetBoundingSphere(out Vector3 c, out float r);
            center = c;
            radius = r;
        }
        public override void GetAabb(Matrix4 transform, out Vec3 aabbMin, out Vec3 aabbMax)
        {
            Shape.GetAabb(transform, out Vector3 min, out Vector3 max);
            aabbMin = min;
            aabbMax = max;
        }
        public override Vec3 CalculateLocalInertia(float mass)
            => Shape.CalculateLocalInertia(mass);
        #endregion

        public override void Render(Matrix4 worldTransform, ColorF4 color, bool solid)
        {
            
        }
    }
}
