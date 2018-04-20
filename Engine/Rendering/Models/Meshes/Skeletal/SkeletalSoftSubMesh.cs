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
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, visibleByDefault, cullingVolume, primitives, material) { }
        public SkeletalSoftSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            List<LOD> lods) : base(name, visibleByDefault, cullingVolume, lods) { }
        public SkeletalSoftSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            params LOD[] lods) : base(name, visibleByDefault, cullingVolume, lods) { }
    }
}
