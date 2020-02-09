using System;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Wrapper class to render a <see cref="PrimitiveManager"/> in a <see cref="BaseScene"/>.
    /// Not attached to the component system.
    /// </summary>
    public class DirectMesh : I3DRenderable
    {
        public DirectMesh() { }
        public DirectMesh(PrimitiveManager m)
        {
            Primitives = m;
            Transform = Matrix4.Identity;
        }

        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D();
        
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
        
        public Scene3D OwningScene3D { get; set; }

        private RenderCommandMesh3D _rc = new RenderCommandMesh3D(ERenderPass.OpaqueDeferredLit);
        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            passes.Add(_rc);
        }
    }
}
