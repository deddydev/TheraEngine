using System;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

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
    [TFileExt("shape")]
    public abstract class Shape : TFileObject, I3DRenderable, IVolume
    {
        public Shape()
        {
            _rc = new RenderCommandMethod3D(Render);
        }

        public Action VisibilityChanged;

        [TSerialize]
        [Category("Shape")]
        public Transform Transform
        {
            get => _transform;
            set
            {
                if (_transform != null)
                    _transform.MatrixChanged -= TransformChanged;
                _transform = value ?? Transform.GetIdentity();
                _transform.MatrixChanged += TransformChanged;
            }
        }

        [TSerialize("RenderSolid", NodeType = ENodeType.Attribute)]
        protected bool _renderSolid = false;
        [TSerialize("RenderColor")]
        protected ColorF3 _renderColor = ColorF3.Red;

        protected Transform _transform = Transform.GetIdentity();

        [Browsable(false)]
        public Scene3D OwningScene3D { get; set; }
        [Browsable(false)]
        public virtual Shape CullingVolume => this;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        
        [Category("Rendering")]
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, true);
        [Category("Rendering")]
        public bool RenderSolid
        {
            get => _renderSolid;
            set => _renderSolid = value;
        }

        private void TransformChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
            => OctreeNode?.ItemMoved(this);

        public abstract TCollisionShape GetCollisionShape();
        public abstract BoundingBox GetAABB();

        public abstract Vec3 ClosestPoint(Vec3 point);

        #region Containment
        public abstract bool Contains(Vec3 point);
        public abstract EContainment Contains(BoundingBox box);
        public abstract EContainment Contains(Box box);
        public abstract EContainment Contains(Sphere sphere);
        public abstract EContainment Contains(Cone cone);
        public abstract EContainment Contains(Cylinder cylinder);
        public abstract EContainment Contains(Capsule capsule);
        public EContainment Contains(Shape shape)
        {
            if (shape != null)
            {
                if (shape is BoundingBox bb)
                    return Contains(bb);
                else if (shape is Box box)
                    return Contains(box);
                else if (shape is Sphere sphere)
                    return Contains(sphere);
                else if (shape is Cone cone)
                    return Contains(cone);
                else if (shape is Cylinder cylinder)
                    return Contains(cylinder);
                else if (shape is Capsule capsule)
                    return Contains(capsule);
            }
            return EContainment.Contains;
        }
        public EContainment ContainedWithin(BoundingBox box) => box?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Box box) => box?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Sphere sphere) => sphere?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Frustum frustum) => frustum?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Shape shape) => shape?.Contains(this) ?? EContainment.Contains;
        #endregion

        /// <summary>
        /// Returns a hard copy of this shape, transformed by the given transform.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Shape PostTransformedBy(Matrix4 matrix)
        {
            var obj = HardCopy();
            obj.Transform.Matrix = Transform.Matrix * matrix;
            return obj;
        }
        /// <summary>
        /// Returns a hard copy of this shape, transformed by the given transform.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public Shape PreTransformedBy(Matrix4 matrix)
        {
            var obj = HardCopy();
            obj.Transform.Matrix = matrix * Transform.Matrix;
            return obj;
        }
        /// <summary>
        /// Returns a completely unique copy of this shape (nothing shares the same memory location).
        /// </summary>
        public abstract Shape HardCopy();
        public abstract void Render();

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera) 
            => passes.Add(_rc, RenderInfo.RenderPass);
    }
}
