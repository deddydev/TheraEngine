using System.Collections.Generic;
using TheraEngine.ComponentModel;
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
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            TMesh primitives,
            TMaterial material) : base(name, renderInfo, renderPass, primitives, material) { }
        public StaticRigidSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            IEventList<ILOD> lods) : base(name, renderInfo, renderPass, lods) { }
        public StaticRigidSubMesh(
            string name,
            IRenderInfo3D renderInfo,
            ERenderPass renderPass,
            params ILOD[] lods) : base(name, renderInfo, renderPass, lods) { }
    }
}
