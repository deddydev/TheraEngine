using System;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Shapes;

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
    public abstract class TShape : TFileObject, I3DRenderable, IVolume
    {
        public TShape()
        {
            _rc = new RenderCommandMethod3D(ERenderPass.OpaqueForward, Render);
        }
        
        public event Action VolumePropertyChanged;

        protected void OnVolumePropertyChanged()
            => VolumePropertyChanged?.Invoke();

        //[TSerialize]
        //[Category("Shape")]
        //public Transform Transform
        //{
        //    get => _transform;
        //    set
        //    {
        //        if (_transform != null)
        //            _transform.MatrixChanged -= TransformChanged;
        //        _transform = value ?? Transform.GetIdentity();
        //        _transform.MatrixChanged += TransformChanged;
        //    }
        //}

        protected bool _renderSolid = false;
        [TSerialize("RenderColor")]
        protected ColorF3 _renderColor = ColorF3.Red;

        //protected Transform _transform = Transform.GetIdentity();

        [TSerialize]
        [Category("Rendering")]
        public IRenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(false, true);
        [TSerialize(NodeType = ENodeType.Attribute)]
        [Category("Rendering")]
        public bool RenderSolid { get; set; }

        private void TransformChanged(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix)
            => RenderInfo.OctreeNode?.ItemMoved(this);

        public abstract TCollisionShape GetCollisionShape();
        public abstract BoundingBox GetAABB();

        public abstract Vec3 ClosestPoint(Vec3 point);

        #region Containment
        public abstract bool Contains(Vec3 point);
        public abstract EContainment Contains(BoundingBoxStruct box);
        public abstract EContainment Contains(BoundingBox box);
        public abstract EContainment Contains(Box box);
        public abstract EContainment Contains(Sphere sphere);
        public abstract EContainment Contains(Cone cone);
        public abstract EContainment Contains(Cylinder cylinder);
        public abstract EContainment Contains(Capsule capsule);
        public EContainment Contains(TShape shape)
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
        public EContainment ContainedWithin(BoundingBoxStruct box) => box.Contains(this);
        public EContainment ContainedWithin(BoundingBox box) => box?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Box box) => box?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Sphere sphere) => sphere?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(Frustum frustum) => frustum?.Contains(this) ?? EContainment.Contains;
        public EContainment ContainedWithin(TShape shape) => shape?.Contains(this) ?? EContainment.Contains;
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="matrix"></param>
        public abstract void SetTransformMatrix(Matrix4 matrix);
        /// <summary>
        /// Returns a completely unique copy of this shape (nothing shares the same memory location).
        /// </summary>
        public abstract Matrix4 GetTransformMatrix();
        public abstract TShape HardCopy();
        public abstract void Render();

        private readonly RenderCommandMethod3D _rc;
        public void AddRenderables(RenderPasses passes, ICamera camera) 
            => passes.Add(_rc);
    }
}
