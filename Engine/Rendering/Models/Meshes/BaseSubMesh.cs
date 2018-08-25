using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public abstract class BaseSubMesh : TFileObject, IBaseSubMesh
    {
        [TSerialize(Order = 0)]
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass.OpaqueDeferredLit) { CastsShadows = true, ReceivesShadows = true };

        [TSerialize(Order = 1)]
        public Shape CullingVolume { get; set; }

        [Browsable(false)]
        [TSerialize(Order = 2)]
        public List<LOD> LODs { get; set; }

        public BaseSubMesh() { }
        public BaseSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material)
        {
            _name = name;
            RenderInfo = renderInfo;
            CullingVolume = cullingVolume;
            LODs = new List<LOD>() { new LOD(material, primitives, 0.0f) };
        }
        public BaseSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            List<LOD> lods)
        {
            _name = name;
            RenderInfo = renderInfo;
            CullingVolume = cullingVolume;
            LODs = lods ?? new List<LOD>();
        }
        public BaseSubMesh(
            string name,
            RenderInfo3D renderInfo,
            Shape cullingVolume,
            params LOD[] lods)
        {
            _name = name;
            RenderInfo = renderInfo;
            CullingVolume = cullingVolume;
            LODs = lods.ToList();
        }
    }
}
