using System.Collections.Generic;
using System.ComponentModel;
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
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, primitives, material) { }
        public SkeletalRigidSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            IEventList<ILOD> lods) : base(name, renderInfo, renderPass, lods) { }
        public SkeletalRigidSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            params ILOD[] lods) : base(name, renderInfo, renderPass, lods) { }
    }
}
