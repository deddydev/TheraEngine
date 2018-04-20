using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [FileExt("strmesh")]
    [FileDef("Static Rigid Sub Mesh")]
    public class StaticRigidSubMesh : BaseSubMesh, IStaticSubMesh
    {
        public StaticRigidSubMesh() : base() { _name = "StaticRigidSubMesh"; }
        public StaticRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, visibleByDefault, cullingVolume, primitives, material) { }
        public StaticRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            List<LOD> lods) : base(name, visibleByDefault, cullingVolume, lods) { }
        public StaticRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            params LOD[] lods) : base(name, visibleByDefault, cullingVolume, lods) { }
    }
}
