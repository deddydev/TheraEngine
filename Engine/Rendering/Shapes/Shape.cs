using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Rendering;
using System;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Core.Shapes
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
    [FileExt("shape")]
    public abstract class Shape : TFileObject, I3DRenderable, IVolume
    {
        public Shape()
        {
            _rc = new RenderCommandDebug3D(Render);
        }

        public RenderInfo3D RenderInfo { get; }
            = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);

        public Action VisibilityChanged;

        [Browsable(false)]
        public virtual Shape CullingVolume => this;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        [TSerialize("IsVisible", XmlNodeType = EXmlNodeType.Attribute)]
        protected bool _isVisible;
        [TSerialize("RenderSolid", XmlNodeType = EXmlNodeType.Attribute)]
        protected bool _renderSolid;
        [TSerialize("RenderColor")]
        protected ColorF3 _renderColor = ColorF3.Red;
        [TSerialize("VisibleInEditorOnly", XmlNodeType = EXmlNodeType.Attribute)]
        protected bool _visibleInEditorOnly = false;
        [TSerialize("HiddenFromOwner", XmlNodeType = EXmlNodeType.Attribute)]
        protected bool _hiddenFromOwner = false;
        [TSerialize("VisibleToOwnerOnly", XmlNodeType = EXmlNodeType.Attribute)]
        protected bool _visibleToOwnerOnly = false;
        
        public bool RenderSolid
        {
            get => _renderSolid;
            set => _renderSolid = value;
        }
        public bool Visible
        {
            get => _isVisible;
            set
            {
                if (_isVisible == value)
                    return;
                _isVisible = value;
                VisibilityChanged?.Invoke();
            }
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
        
        public abstract TCollisionShape GetCollisionShape();
        public abstract BoundingBox GetAABB();

        public abstract Vec3 ClosestPoint(Vec3 point);
        public abstract bool Contains(Vec3 point);
        public abstract EContainment Contains(BoundingBox box);
        public abstract EContainment Contains(Box box);
        public abstract EContainment Contains(Sphere sphere);
        public EContainment Contains(Shape shape)
        {
            if (shape != null)
            {
                if (shape is BoundingBox bb)
                    return Contains(bb);
                else if (shape is Box b)
                    return Contains(b);
                else if (shape is Sphere s)
                    return Contains(s);
            }
            return EContainment.Disjoint;
        }
        public abstract EContainment ContainedWithin(BoundingBox box);
        public abstract EContainment ContainedWithin(Box box);
        public abstract EContainment ContainedWithin(Sphere sphere);
        public abstract EContainment ContainedWithin(Frustum frustum);
        public EContainment ContainedWithin(Shape shape)
        {
            if (shape != null)
            {
                if (shape is BoundingBox bb)
                    return ContainedWithin(bb);
                else if (shape is Box b)
                    return ContainedWithin(b);
                else if (shape is Sphere s)
                    return ContainedWithin(s);
            }
            return EContainment.Disjoint;
        }

        /// <summary>
        /// Applies the transform to this shape.
        /// </summary>
        public virtual void SetRenderTransform(Matrix4 worldMatrix)
        {
            OctreeNode?.ItemMoved(this);
        }
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
        public abstract Matrix4 GetTransformMatrix();
        public abstract void Render();

        private RenderCommandDebug3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
    }
}
