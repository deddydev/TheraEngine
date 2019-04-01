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
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, primitives, material) { }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            EventList<LOD> lods) : base(name, renderInfo, renderPass, lods) { }
        public SkeletalRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            params LOD[] lods) : base(name, renderInfo, renderPass, lods) { }
    }
}
