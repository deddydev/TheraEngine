using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Rendering
{
    public class PrimitiveRenderWrapper : I3DRenderable
    {
        public PrimitiveRenderWrapper() { }
        public PrimitiveRenderWrapper(PrimitiveManager m) => Primitives = m;

        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null, false, false);
        public Matrix4 Transform { get; set; } = Matrix4.Identity;
        public PrimitiveManager Primitives { get; set; }

        IOctreeNode I3DBoundable.OctreeNode { get; set; }
        Shape I3DBoundable.CullingVolume => null;

        public void Render() => Primitives?.Render(Transform, Transform.Transposed().Inverted().GetRotationMatrix3());
    }
}
