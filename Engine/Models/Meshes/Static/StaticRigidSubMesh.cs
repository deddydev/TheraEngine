using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [TFileExt("strmesh")]
    [TFileDef("Static Rigid Sub Mesh")]
    public class StaticRigidSubMesh : BaseSubMesh, IStaticSubMesh
    {
        public StaticRigidSubMesh() : base() { _name = "StaticRigidSubMesh"; }
        public StaticRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            TShape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, renderPass, cullingVolume, primitives, material) { }
        public StaticRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            TShape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
        public StaticRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            ERenderPass renderPass,
            TShape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, renderPass, cullingVolume, lods) { }
    }
}
