using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    public abstract class BaseSubMesh : TFileObject, IBaseSubMesh
    {
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault { get; set; }
        [TSerialize(Order = 0)]
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null);
        [TSerialize(Order = 1)]
        public Shape CullingVolume { get; set; }
        [TSerialize(Order = 2)]
        public List<LOD> LODs { get; set; }

        public BaseSubMesh() { }
        public BaseSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material)
        {
            _name = name;
            VisibleByDefault = visibleByDefault;
            CullingVolume = cullingVolume;
            LODs = new List<LOD>() { new LOD(material, primitives, 0.0f) };
        }
        public BaseSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            List<LOD> lods)
        {
            _name = name;
            VisibleByDefault = visibleByDefault;
            CullingVolume = cullingVolume;
            LODs = lods ?? new List<LOD>();
        }
        public BaseSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            params LOD[] lods)
        {
            _name = name;
            VisibleByDefault = visibleByDefault;
            CullingVolume = cullingVolume;
            LODs = lods.ToList();
        }
    }
}
