using System;
using TheraEngine.Rendering;
using System.Drawing;
using TheraEngine.Core.Shapes;
using System.ComponentModel;

namespace TheraEngine.Worlds.Actors.Components.Scene.Shapes
{
    public class BoxComponent : ShapeComponent
    {
        protected Box _box;

        public BoxComponent()
        {
            _box = new Box(1.0f);
        }
        public BoxComponent(PhysicsConstructionInfo info)
        {
            _box = new Box(1.0f);
            InitPhysics(info);
        }
        public BoxComponent(Vec3 halfExtents, PhysicsConstructionInfo info)
        {
            _box = new Box(halfExtents);
            InitPhysics(info);
        }
        public BoxComponent(float extentsX, float extentsY, float extentsZ, PhysicsConstructionInfo info)
        {
            _box = new Box(extentsX, extentsY, extentsZ);
            InitPhysics(info);
        }
        public BoxComponent(float uniformExtents, PhysicsConstructionInfo info)
        {
            _box = new Box(uniformExtents);
            InitPhysics(info);
        }
        
        [Browsable(false)]
        public Box Box
        {
            get => _box;
            set => _box = value;
        }
        public override Shape CullingVolume => _box;

        [Category("Box")]
        public Vec3 HalfExtents
        {
            get => _box.HalfExtents;
            set => _box.HalfExtents = value;
        }

        public override void Render()
        {
            Engine.Renderer.ApplyRenderParams(RenderParams);
            Engine.Renderer.RenderBox(HalfExtents, WorldMatrix, false, Color.Lavender);
        }
        protected override CollisionShape GetCollisionShape() => _box.GetCollisionShape();

        public bool Contains(Vec3 point) => _box.Contains(point);
        public EContainment Contains(BoundingBox box) => _box.Contains(box);
        public EContainment Contains(Box box) => _box.Contains(box);
        public EContainment Contains(Sphere sphere) => _box.Contains(sphere);
        public EContainment ContainedWithin(BoundingBox box) => box.Contains(_box);
        public EContainment ContainedWithin(Box box) => box.Contains(_box);
        public EContainment ContainedWithin(Sphere sphere) => sphere.Contains(_box);
        public EContainment ContainedWithin(Frustum frustum) => frustum.Contains(_box);

        public Vec3[] GetTransformedCorners() => _box.GetCorners();
        public Vec3[] GetUntransformedCorners() => BoundingBox.GetCorners(_box.HalfExtents, Matrix4.Identity);

        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _box.WorldMatrix = WorldMatrix;
        }
    }
}
