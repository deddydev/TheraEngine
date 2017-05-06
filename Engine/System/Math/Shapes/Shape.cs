using BulletSharp;
using CustomEngine.Rendering;
using CustomEngine;
using CustomEngine.Rendering.Models;
using CustomEngine.Worlds.Actors;
using CustomEngine.Files;
using System.Collections.Generic;
using System.ComponentModel;

namespace System
{
    public enum ShapeType
    {
        Ray,
        Segment,
        Plane,
        BoundingRectangle,
        Sphere,
        AABB,
        Box,
        Frustum,
        CylinderX,
        CylinderY,
        CylinderZ,
        CylinderComplex,
        CapsuleX,
        CapsuleY,
        CapsuleZ,
        CapsuleComplex,
        ConeX,
        ConeY,
        ConeZ,
        ConeComplex,
    }
    public abstract class Shape : FileObject, IRenderable
    {
        public event Action AttributeChanged;

        protected bool _isRendering;
        [Serialize("IsVisible", IsXmlAttribute = true)]
        protected bool _isVisible;
        [Serialize("RenderSolid", IsXmlAttribute = true)]
        protected bool _renderSolid;
        [Serialize("VisibleInEditorOnly", IsXmlAttribute = true)]
        protected bool _visibleInEditorOnly = false;
        [Serialize("HiddenFromOwner", IsXmlAttribute = true)]
        protected bool _hiddenFromOwner = false;
        [Serialize("VisibleToOwnerOnly", IsXmlAttribute = true)]
        protected bool _visibleToOwnerOnly = false;

        protected IOctreeNode _renderNode;
        private int _shapeIndex;
        private string _shapeName;

        public bool RenderSolid
        {
            get => _renderSolid;
            set => _renderSolid = value;
        }
        public string ShapeName => _shapeName;
        public int ShapeIndex
        {
            get => _shapeIndex;
            protected set
            {
                _shapeIndex = value;
                _shapeName = GetType().Name + _shapeIndex;
            }
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
        public bool VisibleInEditorOnly
        {
            get => _visibleInEditorOnly;
            set => _visibleInEditorOnly = value;
        }
        public bool HiddenFromOwner
        {
            get => _hiddenFromOwner;
            set => _hiddenFromOwner = value;
        }
        public bool VisibleToOwnerOnly
        {
            get => _visibleToOwnerOnly;
            set => _visibleToOwnerOnly = value;
        }
        public Shape CullingVolume { get { return this; } }
        public IOctreeNode RenderNode
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
        /// Returns a completely unique copy of this shape (nothing shares the same memory location).
        /// </summary>
        public abstract Shape HardCopy();
    }
}
