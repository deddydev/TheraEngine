using System;
using BulletSharp;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoxComponent : ShapeComponent
    {
        public BoxComponent() : base(null)
        {
            _box = new Box(1.0f);
        }
        public BoxComponent(PhysicsDriverInfo info) : base(info)
        {
            _box = new Box(1.0f);
        }
        public BoxComponent(Vec3 extents, PhysicsDriverInfo info) : base(info)
        {
            _box = new Box(extents);
        }
        public BoxComponent(float extentsX, float extentsY, float extentsZ, PhysicsDriverInfo info) : base(info)
        {
            _box = new Box(extentsX, extentsY, extentsZ);
        }
        public BoxComponent(float uniformExtents, PhysicsDriverInfo info) : base(info)
        {
            _box = new Box(uniformExtents);
        }

        private Box _box;
        public Box Box
        {
            get { return _box; }
            set { _box = value; }
        }
        public override Shape CullingVolume { get { return _box; } }
        public Vec3 HalfExtents
        {
            get { return _box.HalfExtents; }
            set { _box.HalfExtents = value; }
        }
        public override void Render()
        {
            Engine.Renderer.RenderBox(HalfExtents, WorldMatrix, false);
        }
        protected override CollisionShape GetCollisionShape() { return _box.GetCollisionShape(); }
        public bool Contains(Vec3 point) { return _box.Contains(point); }
        public EContainment Contains(BoundingBox box) { return _box.Contains(box); }
        public EContainment Contains(Box box) { return _box.Contains(box); }
        public EContainment Contains(Sphere sphere) { return _box.Contains(sphere); }
        public EContainment ContainedWithin(BoundingBox box) { return box.Contains(_box); }
        public EContainment ContainedWithin(Box box) { return box.Contains(_box); }
        public EContainment ContainedWithin(Sphere sphere) { return sphere.Contains(_box); }
        public EContainment ContainedWithin(Frustum frustum) { return frustum.Contains(_box); }
        public Vec3[] GetTransformedCorners() { return _box.GetCorners(); }
        public Vec3[] GetUntransformedCorners() { return BoundingBox.GetCorners(_box.HalfExtents, Matrix4.Identity); }
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _box.WorldMatrix = WorldMatrix;
        }
    }
}
