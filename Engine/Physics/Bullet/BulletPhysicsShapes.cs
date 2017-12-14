using System;
using BulletSharp;

namespace TheraEngine.Physics.Bullet
{
    public interface IBulletShape
    {
        CollisionShape Shape { get; }
    }
    internal class BulletSphere : TCollisionSphere, IBulletShape
    {
        public SphereShape Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius
        {
            get => Shape.Radius;
            set => Shape.SetUnscaledRadius(value);
        }
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

        public BulletSphere()
        {
            Shape = new SphereShape(0.5f);
        }
        public BulletSphere(float radius)
        {
            Shape = new SphereShape(radius);
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
    }
    internal class BulletBox : TCollisionBox, IBulletShape
    {
        public BoxShape Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override Vec3 HalfExtents => Shape.HalfExtentsWithoutMargin;
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

        public BulletBox()
        {
            Shape = new BoxShape(0.5f);
        }
        public BulletBox(Vec3 halfExtents)
        {
            Shape = new BoxShape(halfExtents);
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
    }
}
