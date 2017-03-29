using System;
using CustomEngine.Rendering;
using BulletSharp;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SphereComponent : ShapeComponent
    {
        private Sphere _sphere;

        public SphereComponent(float radius)
        {
            _sphere = new Sphere(radius, Vec3.Zero);
        }
        public SphereComponent(float radius, PhysicsDriverInfo info)
        {
            _sphere = new Sphere(radius, Vec3.Zero);
            InitPhysics(info);
        }

        public float Radius
        {
            get => _sphere.Radius;
            set => _sphere.Radius = value;
        }
        public Vec3 Center
            => _sphere.Center;

        public override Shape CullingVolume
            => _sphere;
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _sphere.Center = GetWorldPoint();
        }
        public override void Render()
            => _sphere.Render();
        protected override CollisionShape GetCollisionShape()
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
    }
}
