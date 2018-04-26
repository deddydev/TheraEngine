﻿using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    public class SphereComponent : ShapeComponent
    {
        private Sphere _sphere;

        public SphereComponent() : this(1.0f) { }
        public SphereComponent(float radius) : this(radius, null) { }
        public SphereComponent(float radius, TRigidBodyConstructionInfo info)
        {
            _sphere = new Sphere(radius, Vec3.Zero);
            InitPhysicsShape(info);
        }

        [Category("Sphere")]
        public float Radius
        {
            get => _sphere.Radius;
            set => _sphere.Radius = value;
        }

        [Browsable(false)]
        public Vec3 Center
            => _sphere.Center;

        public override Shape CullingVolume
            => _sphere;
        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            _sphere.Center = WorldPoint;
        }
        protected override TCollisionShape GetCollisionShape()
            => _sphere.GetCollisionShape();
        public bool Contains(Vec3 point) 
            => _sphere.Contains(point);
        public EContainment Contains(BoundingBox box)
            => _sphere.Contains(box);
        public EContainment Contains(Box box)
            => _sphere.Contains(box);
        public EContainment Contains(Sphere sphere)
            => _sphere.Contains(sphere);
        public EContainment ContainedWithin(BoundingBox box)
            => _sphere.ContainedWithin(box);
        public EContainment ContainedWithin(Box box)
            => _sphere.ContainedWithin(box);
        public EContainment ContainedWithin(Sphere sphere)
            => _sphere.ContainedWithin(sphere);
        public EContainment ContainedWithin(Frustum frustum)
            => _sphere.ContainedWithin(frustum);

        public override void Render()
        {

        }
    }
}
