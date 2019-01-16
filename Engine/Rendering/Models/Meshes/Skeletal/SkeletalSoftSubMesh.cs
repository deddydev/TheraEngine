using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [TFileExt("sksmesh")]
    [TFileDef("Skeletal Soft Sub Mesh")]
    public class SkeletalSoftSubMesh : BaseSubMesh, ISkeletalSubMesh
    {
        public SkeletalSoftSubMesh() : base() { _name = "SkeletalSoftSubMesh"; }
        public SkeletalSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, cullingVolume, primitives, material) { }
        public SkeletalSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            Shape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
        public SkeletalSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            Shape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
    }
}
