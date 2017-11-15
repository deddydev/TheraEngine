using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    [FileClass("OBJ", "Static Soft Sub Mesh")]
    public class StaticSoftSubMesh : FileObject, IStaticSubMesh
    {
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null);

        public StaticSoftSubMesh()
        {
            _name = "SoftSubMesh";
            _cullingVolume = null;
            _material = null;
            _data = null;
        }
        public StaticSoftSubMesh(string name, PrimitiveData data, Material material, Shape cullingVolume)
        {
            _cullingVolume = cullingVolume;
            _material = material;
            _data = data;
            _name = name;
        }

        protected PrimitiveData _data;
        protected Material _material;
        protected Shape _cullingVolume;
        protected bool _visibleByDefault = true;

        [TSerialize]
        public PrimitiveData Data
        {
            get => _data;
            set => _data = value;
        }
        [TSerialize]
        public Shape CullingVolume
        {
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        [TSerialize]
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
    }
}
