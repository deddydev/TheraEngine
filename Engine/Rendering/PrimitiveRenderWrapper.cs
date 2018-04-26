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
        public Matrix4 Transform { get; set; } = Matrix4.Identity;
        public PrimitiveManager Primitives { get; set; }
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
            _rc.Primitives = Primitives;
            _rc.WorldMatrix = Transform;
            _rc.NormalMatrix = Transform.Transposed().Inverted().GetRotationMatrix3();
            passes.Add(_rc, RenderInfo.RenderPass);
        }
    }
}
