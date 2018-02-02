using System;
using BulletSharp;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    internal class BulletCapsuleX : TCollisionCapsuleX, IBulletShape
    {
        public CapsuleShapeX Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;
        
        public override float Radius => Shape.Radius;
        public override float Height => Shape.HalfHeight * 2.0f;
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

        public BulletCapsuleX() : this(0.5f, 1.0f) { }
        public BulletCapsuleX(float radius, float height)
            => Shape = new CapsuleShapeX(radius, height);

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
    internal class BulletCapsuleY : TCollisionCapsuleY, IBulletShape
    {
        public CapsuleShape Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.HalfHeight * 2.0f;
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

        public BulletCapsuleY() : this(0.5f, 1.0f) { }
        public BulletCapsuleY(float radius, float height)
            => Shape = new CapsuleShape(radius, height);

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
    internal class BulletCapsuleZ : TCollisionCapsuleZ, IBulletShape
    {
        public CapsuleShapeZ Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.HalfHeight * 2.0f;
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

        public BulletCapsuleZ() : this(0.5f, 1.0f) { }
        public BulletCapsuleZ(float radius, float height)
            => Shape = new CapsuleShapeZ(radius, height);

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
