using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Files;

namespace System
{
    public abstract class Shape : FileObject, IMesh
    {
        public Shape() { }

        public event Action AttributeChanged;

        protected bool _isRendering, _isVisible, _visibleByDefault, _renderSolid;

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

        public Shape CullingVolume { get { return this; } }

        public RenderOctree.OctreeNode RenderNode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public abstract void Render();
        public abstract CollisionShape GetCollisionShape();

        public abstract bool Contains(Vec3 point);
        public abstract EContainment Contains(BoundingBox box);
        public abstract EContainment Contains(Box box);
        public abstract EContainment Contains(Sphere sphere);
        public abstract EContainment ContainedWithin(BoundingBox box);
        public abstract EContainment ContainedWithin(Box box);
        public abstract EContainment ContainedWithin(Sphere sphere);
        public abstract EContainment ContainedWithin(Frustum frustum);
        public abstract void SetTransform(Matrix4 worldMatrix);
        public abstract Shape TransformedBy(Matrix4 worldMatrix);
        public abstract Shape HardCopy();
    }
}
