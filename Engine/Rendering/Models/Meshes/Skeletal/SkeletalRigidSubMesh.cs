using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [TFileExt("skrmesh")]
    [TFileDef("Skeletal Rigid Sub Mesh")]
    public class SkeletalRigidSubMesh : BaseSubMesh, ISkeletalSubMesh
    {
        public SkeletalRigidSubMesh() : base() { _name = "SkeletalRigidSubMesh"; }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, cullingVolume, primitives, material) { }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            Shape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            Shape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
    }
}
