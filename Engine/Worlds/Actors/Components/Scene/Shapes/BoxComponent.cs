using System;
using BulletSharp;
using CustomEngine.Rendering;
using System.Drawing;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoxComponent : ShapeComponent
    {
        private Box _box;

        public BoxComponent()
        {
            _box = new Box(1.0f);
        }
        public BoxComponent(PhysicsDriverInfo info)
        {
            _box = new Box(1.0f);
            InitPhysics(info);
        }
        public BoxComponent(Vec3 halfExtents, PhysicsDriverInfo info)
        {
            _box = new Box(halfExtents);
            InitPhysics(info);
        }
        public BoxComponent(float extentsX, float extentsY, float extentsZ, PhysicsDriverInfo info)
        {
            _box = new Box(extentsX, extentsY, extentsZ);
            InitPhysics(info);
        }
        public BoxComponent(float uniformExtents, PhysicsDriverInfo info)
        {
            _box = new Box(uniformExtents);
            InitPhysics(info);
        }

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
            Engine.Renderer.RenderBox(Box.ShapeName, HalfExtents, WorldMatrix, false, Color.Black);
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
