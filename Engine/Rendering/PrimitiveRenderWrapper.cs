using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Wrapper class to render a <see cref="PrimitiveManager"/> in a <see cref="BaseScene"/>.
    /// Not attached to the component system.
    /// </summary>
    public class MeshRenderable : I3DRenderable
    {
        public MeshRenderable() { }
        public MeshRenderable(PrimitiveManager m)
        {
            Primitives = m;
            Transform = Matrix4.Identity;
        }

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
            get => _rc.Mesh;
            set => _rc.Mesh = value;
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

        public Scene3D OwningScene3D { get; set; }

        private RenderCommandMesh3D _rc = new RenderCommandMesh3D();
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
    }
}
