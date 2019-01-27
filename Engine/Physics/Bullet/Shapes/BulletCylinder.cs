using System;
using BulletSharp;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics.Bullet.Shapes
{
    internal class BulletCylinderX : TCollisionCylinderX, IBulletShape
    {
        public CylinderShapeX Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;
        
        public override float Radius => Shape.Radius;
        public override float Height => Shape.HalfExtentsWithMargin.X * 2.0f;
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
        
        public BulletCylinderX() : this(0.5f, 1.0f) { }
        public BulletCylinderX(float radius, float height)
            => Shape = new CylinderShapeX(new Vector3(height, radius, 0.0f));

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
    }
    internal class BulletCylinderY : TCollisionCylinderY, IBulletShape
    {
        public CylinderShape Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.HalfExtentsWithMargin.Y * 2.0f;
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

        public BulletCylinderY() : this(0.5f, 1.0f) { }
        public BulletCylinderY(float radius, float height)
            => Shape = new CylinderShape(new Vector3(radius, height, 0.0f));

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
    }
    internal class BulletCylinderZ : TCollisionCylinderZ, IBulletShape
    {
        public CylinderShapeZ Shape { get; }
        CollisionShape IBulletShape.Shape => Shape;

        public override float Radius => Shape.Radius;
        public override float Height => Shape.HalfExtentsWithMargin.Z * 2.0f;
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

        public BulletCylinderZ() : this(0.5f, 1.0f) { }
        public BulletCylinderZ(float radius, float height)
            => Shape = new CylinderShapeZ(new Vector3(radius, 0.0f, height));

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
    }
}
