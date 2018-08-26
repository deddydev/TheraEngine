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
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, cullingVolume, primitives, material) { }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, cullingVolume, lods) { }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, cullingVolume, lods) { }
    }
}
