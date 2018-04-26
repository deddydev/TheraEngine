using System;
using System.Drawing;
using TheraEngine.Core.Shapes;
using System.ComponentModel;
using TheraEngine.Physics;

namespace TheraEngine.Components.Scene.Shapes
{
    [FileDef("Box Component")]
    public class BoxComponent : ShapeComponent
    {
        protected Box _box;

        public BoxComponent()
        {
            _box = new Box(1.0f);
        }
        public BoxComponent(TRigidBodyConstructionInfo info)
        {
            _box = new Box(1.0f);
            InitPhysicsShape(info);
        }
        public BoxComponent(Vec3 halfExtents, TRigidBodyConstructionInfo info)
        {
            _box = new Box(halfExtents);
            InitPhysicsShape(info);
        }
        public BoxComponent(float extentsX, float extentsY, float extentsZ, TRigidBodyConstructionInfo info)
        {
            _box = new Box(extentsX, extentsY, extentsZ);
            InitPhysicsShape(info);
        }
        public BoxComponent(float uniformExtents, TRigidBodyConstructionInfo info)
        {
            _box = new Box(uniformExtents);
            InitPhysicsShape(info);
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
            set
            {
                _box.HalfExtents = value;
                OctreeNode?.ItemMoved(this);
            }
        }
        
        protected override TCollisionShape GetCollisionShape() => _box.GetCollisionShape();

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

        protected override void OnWorldTransformChanged()
        {
            _box.WorldMatrix = WorldMatrix;
            base.OnWorldTransformChanged();
        }

        public override void Render()
        {
            Box.Render();
        }
    }
}
