using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering.Models
{
    [FileExt("stsmesh")]
    [FileDef("Static Soft Sub Mesh")]
    public class StaticSoftSubMesh : TFileObject, IStaticSubMesh
    {
        public StaticSoftSubMesh() { _name = "StaticSoftSubMesh"; }
        public StaticSoftSubMesh(
            string name,
            bool visibleByDefault,
            Shape cullingVolume,
            PrimitiveData primitives,
            TMaterial material)
        {
            _name = name;
            _visibleByDefault = visibleByDefault;
            _cullingVolume.File = cullingVolume;
            _lods.Add(new LOD(material, primitives, 0.0f));
        }
        public StaticSoftSubMesh(
            string name,
            bool visibleByDefault,
            List<LOD> lods)
        {
            _name = name;
            _visibleByDefault = visibleByDefault;
            _lods = lods ?? new List<LOD>();
        }
        public StaticSoftSubMesh(
            string name,
            bool visibleByDefault,
            params LOD[] lods)
        {
            _name = name;
            _visibleByDefault = visibleByDefault;
            _lods = lods.ToList();
        }

        protected List<LOD> _lods = new List<LOD>();
        [TSerialize(nameof(CullingVolumeRef), Order = 1)]
        protected GlobalFileRef<Shape> _cullingVolume = new GlobalFileRef<Shape>();
        protected bool _visibleByDefault = true;

        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }

        [TSerialize(Order = 0)]
        public RenderInfo3D RenderInfo { get; set; }
            = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null);

        //[Browsable(false)]
        public GlobalFileRef<Shape> CullingVolumeRef => _cullingVolume;

        [TSerialize(Order = 2)]
        public List<LOD> LODs
        {
            get => _lods;
            set => _lods = value;
        }
    }
}
