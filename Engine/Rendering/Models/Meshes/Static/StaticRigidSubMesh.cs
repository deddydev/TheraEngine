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
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material) : base(name, renderInfo, cullingVolume, primitives, material) { }
        public StaticRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            List<LOD> lods) : base(name, renderInfo, cullingVolume, lods) { }
        public StaticRigidSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            params LOD[] lods) : base(name, renderInfo, cullingVolume, lods) { }
    }
}
