﻿using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics.Bullet.Shapes
{
    internal class BulletConvexHull : TCollisionConvexHull, IBulletShape
    {
        public ConvexHullShape Shape { get; }
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
        
        public BulletConvexHull() : base()
        {
            Shape = new ConvexHullShape();
        }
        public BulletConvexHull(IEnumerable<Vec3> points)
        {
            Shape = new ConvexHullShape(points.Select(x => (Vector3)x));
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
    }
}
