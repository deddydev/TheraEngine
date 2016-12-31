using System;
using BulletSharp;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoxComponent : ShapeComponent, IBox
    {
        public BoxComponent() : base(null)
        {
            _box = new BoundingBox(Vec3.Zero, -Vec3.One, Vec3.One);
        }
        public BoxComponent(PhysicsDriverInfo info) : base(info)
        {
            _box = new BoundingBox(Vec3.Zero, -Vec3.One, Vec3.One);
        }
        public BoxComponent(Vec3 min, Vec3 max, PhysicsDriverInfo info) : base(info)
        {
            _box = new BoundingBox(Vec3.Zero, min, max);
        }

        private BoundingBox _box;

        public BoundingBox Box
        {
            get { return _box; }
            set { _box = value; }
        }

        public override IShape CullingVolume
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vec3 Minimum
        {
            get { return _box.Minimum; }
            set { _box.Minimum = value; }
        }
        public Vec3 Maximum
        {
            get { return _box.Maximum; }
            set { _box.Maximum = value; }
        }
        public Vec3 Center
        {
            get
            {
                return _box.ExtentsCenter;
            }
        }

        public Vec3[] GetTransformedCorners() { return _box.GetCorners(WorldMatrix); }
        public Vec3[] GetUntransformedCorners() { return _box.GetCorners(); }

        public override void Render()
        {
            throw new NotImplementedException();
        }

        protected override CollisionShape GetCollisionShape()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }

        public EContainment Contains(IBoundingBox box)
        {
            throw new NotImplementedException();
        }

        public EContainment Contains(IBox box)
        {
            throw new NotImplementedException();
        }

        public EContainment Contains(ISphere sphere)
        {
            throw new NotImplementedException();
        }

        public EContainment ContainedWithin(IBoundingBox box)
        {
            throw new NotImplementedException();
        }

        public EContainment ContainedWithin(IBox box)
        {
            throw new NotImplementedException();
        }

        public EContainment ContainedWithin(ISphere sphere)
        {
            throw new NotImplementedException();
        }

        public EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
    }
}
