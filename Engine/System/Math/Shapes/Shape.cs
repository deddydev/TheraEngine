using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Files;

namespace System
{
    public abstract class BoundingShape : FileObject, IRenderable, IShape
    {
        public BoundingShape(Vec3 point)
        {
            _point = point;
        }

        public event Action AttributeChanged;

        protected bool _isRendering, _isVisible, _visibleByDefault, _renderSolid;
        protected Vec3 _point = Vec3.Zero;

        public IShape CullingVolume { get { return this; } }
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public bool VisibleByDefault
        {
            get { return _visibleByDefault; }
        }
        public bool Visible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }
        public Vec3 Center
        {
            get { return _point; }
            set { _point = value; }
        }

        public abstract void Render();
        public abstract CollisionShape GetCollisionShape();

        public abstract bool Contains(Vec3 point);
        public abstract EContainment Contains(IBoundingBox box);
        public abstract EContainment Contains(IBox box);
        public abstract EContainment Contains(ISphere sphere);
        public abstract EContainment ContainedWithin(IBoundingBox box);
        public abstract EContainment ContainedWithin(IBox box);
        public abstract EContainment ContainedWithin(ISphere sphere);
        public abstract EContainment ContainedWithin(Frustum frustum);
    }
}
