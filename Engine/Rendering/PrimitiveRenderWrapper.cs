using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class PrimitiveRenderWrapper : I3DRenderable
    {
        public PrimitiveRenderWrapper() { }
        public PrimitiveRenderWrapper(PrimitiveManager m) => Primitives = m;

        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueDeferredLit);
        public Matrix4 Transform
        {
            get => _rc.WorldMatrix;
            set
            {
                _rc.WorldMatrix = value;
                _rc.NormalMatrix = Transform.Transposed().Inverted().GetRotationMatrix3();
            }
        }
        public PrimitiveManager Primitives
        {
            get => _rc.Primitives;
            set => _rc.Primitives = value;
        }
        public TMaterial Material
        {
            get => Primitives?.Material;
            set
            {
                if (Primitives != null)
                    Primitives.Material = value;
            }
        }

        IOctreeNode I3DBoundable.OctreeNode { get; set; }
        Shape I3DBoundable.CullingVolume => null;

        private RenderCommandMesh3D _rc = new RenderCommandMesh3D();
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
    }
}
