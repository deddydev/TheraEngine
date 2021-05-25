using System.Collections.Generic;
using TheraEngine.ComponentModel;
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
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            TMesh primitives,
            TMaterial material) : base(name, renderInfo, renderPass, primitives, material) { }
        public SkeletalSoftSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            IEventList<ILOD> lods) : base(name, renderInfo, renderPass, lods) { }
        public SkeletalSoftSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            params ILOD[] lods) : base(name, renderInfo, renderPass, lods) { }
    }
}
