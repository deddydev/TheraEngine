using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [FileExt("stsmesh")]
    [FileDef("Static Soft Sub Mesh")]
    public class StaticSoftSubMesh : BaseSubMesh, IStaticSubMesh
    {
        public StaticSoftSubMesh() : base() { _name = "StaticSoftSubMesh"; }
        public StaticSoftSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, visibleByDefault, cullingVolume, primitives, material) { }
        public StaticSoftSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            List<LOD> lods) : base(name, visibleByDefault, cullingVolume, lods) { }
        public StaticSoftSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            params LOD[] lods) : base(name, visibleByDefault, cullingVolume, lods) { }
    }
}
