using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Files;

namespace System
{
    public abstract class Shape : FileObject, IRenderable
    {
        public override ResourceType ResourceType { get { return ResourceType.Shape; } }

        public Shape() { }

        public event Action AttributeChanged;

        protected bool _isRendering, _isVisible, _visibleByDefault, _renderSolid;
        protected RenderOctree.Node _renderNode;

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
        public RenderOctree.Node RenderNode
        {
            get { return _renderNode; }
            set { _renderNode = value; }
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
        /// <summary>
        /// Applies the transform to this shape.
        /// </summary>
        /// <param name="worldMatrix"></param>
        public abstract void SetTransform(Matrix4 worldMatrix);
        /// <summary>
        /// Returns a hard copy of this shape, transformed by the given transform.
        /// </summary>
        /// <param name="worldMatrix"></param>
        /// <returns></returns>
        public abstract Shape TransformedBy(Matrix4 worldMatrix);
        /// <summary>
        /// Returns a completely unique copy of this shape (nothing shares the same instance).
        /// </summary>
        public abstract Shape HardCopy();
    }
}
