using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [FileExt("sksmesh")]
    [FileDef("Skeletal Soft Sub Mesh")]
    public class SkeletalSoftSubMesh : BaseSubMesh, ISkeletalSubMesh
    {
        public SkeletalSoftSubMesh() : base() { _name = "SkeletalSoftSubMesh"; }
        public SkeletalSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, cullingVolume, primitives, material) { }
        public SkeletalSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, cullingVolume, lods) { }
        public SkeletalSoftSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, cullingVolume, lods) { }
    }
}
