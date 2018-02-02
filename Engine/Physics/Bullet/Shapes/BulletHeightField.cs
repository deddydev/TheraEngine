using System;
using System.IO;
using BulletSharp;
using TheraEngine.Physics.Bullet.Shapes;

namespace TheraEngine.Physics.Bullet
{
    internal class BulletHeightField : TCollisionHeightField, IBulletShape
    {
        public HeightfieldTerrainShape Shape { get; }
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
        
        public BulletHeightField(
            int heightStickWidth,
            int heightStickLength,
            Stream heightfieldData,
            float heightScale,
            float minHeight,
            float maxHeight, 
            int upAxis,
            PhyScalarType heightDataType,
            bool flipQuadEdges)
            => Shape = new HeightfieldTerrainShape(
                heightStickWidth,
                heightStickLength,
                heightfieldData,
                heightScale,
                minHeight,
                maxHeight,
                upAxis,
                heightDataType,
                flipQuadEdges);
        
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
