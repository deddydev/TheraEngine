using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [FileExt("skrmesh")]
    [FileDef("Skeletal Rigid Sub Mesh")]
    public class SkeletalRigidSubMesh : BaseSubMesh, ISkeletalSubMesh
    {
        public SkeletalRigidSubMesh() : base() { _name = "SkeletalRigidSubMesh"; }
        public SkeletalRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, visibleByDefault, cullingVolume, primitives, material) { }
        public SkeletalRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            List<LOD> lods) : base(name, visibleByDefault, cullingVolume, lods) { }
        public SkeletalRigidSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            params LOD[] lods) : base(name, visibleByDefault, cullingVolume, lods) { }
    }
}
