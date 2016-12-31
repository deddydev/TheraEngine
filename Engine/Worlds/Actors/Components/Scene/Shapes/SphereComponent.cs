using System;
using CustomEngine.Rendering;
using BulletSharp;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SphereComponent : ShapeComponent, ISphere
    {
        public SphereComponent(float radius, PhysicsDriverInfo info) : base(info)
        {
            _sphere = new Sphere(radius, Vec3.Zero);
        }

        private Sphere _sphere;

        public float Radius
        {
            get { return _sphere.Radius; }
            set { _sphere.Radius = value; }
        }
        public Vec3 Center
        {
            get { return _sphere.Center; }
        }
        public override IShape CullingVolume { get { return _sphere; } }

        public override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _sphere.Center = GetWorldPoint();
        }

        public override void Render() { _sphere.Render(); }
        protected override CollisionShape GetCollisionShape() { return _sphere.GetCollisionShape(); }
        public bool Contains(Vec3 point) { return _sphere.Contains(point); }
        public EContainment Contains(IBoundingBox box) { return _sphere.Contains(box); }
        public EContainment Contains(IBox box) { return _sphere.Contains(box); }
        public EContainment Contains(ISphere sphere) { return _sphere.Contains(sphere); }
        public EContainment ContainedWithin(IBoundingBox box) { return _sphere.ContainedWithin(box); }
        public EContainment ContainedWithin(IBox box) { return _sphere.ContainedWithin(box); }
        public EContainment ContainedWithin(ISphere sphere) { return _sphere.ContainedWithin(sphere); }
        public EContainment ContainedWithin(Frustum frustum) { return _sphere.ContainedWithin(frustum); }
    }
}
