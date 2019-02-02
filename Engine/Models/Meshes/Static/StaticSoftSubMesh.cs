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
            TShape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, cullingVolume, primitives, material) { }
        public StaticSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            TShape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
        public StaticSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            TShape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
    }
}
