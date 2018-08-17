using System;
using BulletSharp;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics.Bullet.Shapes
{
    internal class BulletConeX : TCollisionConeX, IBulletShape
    {
        public ConeShapeX Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.Height;
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

        public BulletConeX() : this(0.5f, 1.0f) { }
        public BulletConeX(float radius, float height)
            => Shape = new ConeShapeX(radius, height);

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
    internal class BulletConeY : TCollisionConeY, IBulletShape
    {
        public ConeShape Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.Height;
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

        public BulletConeY() : this(0.5f, 1.0f) { }
        public BulletConeY(float radius, float height)
            => Shape = new ConeShape(radius, height);

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
    internal class BulletConeZ : TCollisionConeZ, IBulletShape
    {
        public ConeShapeZ Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.Height;
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

        public BulletConeZ() : this(0.5f, 1.0f) { }
        public BulletConeZ(float radius, float height)
            => Shape = new ConeShapeZ(radius, height);

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
