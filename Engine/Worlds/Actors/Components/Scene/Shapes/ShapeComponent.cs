using BulletSharp;
using System;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public interface IShape
    {
        Vec3 Center { get; }

        bool Contains(Vec3 point);
        EContainment Contains(IBoundingBox box);
        EContainment Contains(IBox box);
        EContainment Contains(ISphere sphere);
        EContainment ContainedWithin(IBoundingBox box);
        EContainment ContainedWithin(IBox box);
        EContainment ContainedWithin(ISphere sphere);
        EContainment ContainedWithin(Frustum frustum);
    }
    public interface ISphere : IShape
    {
        float Radius { get; set; }
    }
    public interface IBoundingBox : IShape
    {
        Vec3 Minimum { get; set; }
        Vec3 Maximum { get; set; }
    }
    public interface IBox : IShape
    {
        Vec3 Minimum { get; set; }
        Vec3 Maximum { get; set; }
        Matrix4 WorldMatrix { get;}
        Matrix4 InverseWorldMatrix { get; }
    }
    public abstract class ShapeComponent : TRComponent, IRenderable, IPhysicsDrivable
    {
        public ShapeComponent(PhysicsDriverInfo info)
        {
            if (info != null)
            {
                info.BodyInfo.CollisionShape = GetCollisionShape();
                _physics = new PhysicsDriver(info);
            }
        }

        protected PhysicsDriver _physics;
        protected bool _isRendering, _isVisible, _visibleByDefault;

        public CustomCollisionGroup CollisionGroup
        {
            get { return _physics.CollisionGroup; }
            set { _physics.CollisionGroup = value; }
        }
        public CustomCollisionGroup CollidesWith
        {
            get { return _physics.CollidesWith; }
            set { _physics.CollidesWith = value; }
        }
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public bool Visible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        public abstract IShape CullingVolume { get; }
        public bool VisibleByDefault { get { return _visibleByDefault; } }
        public PhysicsDriver PhysicsDriver { get { return _physics; } }

        public abstract void Render();
        protected abstract CollisionShape GetCollisionShape();
    }
}
