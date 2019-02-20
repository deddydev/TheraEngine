using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [TFileExt("stsmesh")]
    [TFileDef("Static Soft Sub Mesh")]
    public class StaticSoftSubMesh : BaseSubMesh, IStaticSubMesh
    {
        public StaticSoftSubMesh() : base() { _name = "StaticSoftSubMesh"; }
        public StaticSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, primitives, material) { }
        public StaticSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            List<LOD> lods) : base(name, renderInfo, renderPass, lods) { }
        public StaticSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            params LOD[] lods) : base(name, renderInfo, renderPass, lods) { }
    }
}
