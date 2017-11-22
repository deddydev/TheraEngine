using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("SKSMESH", "Skeletal Rigid Sub Mesh")]
    public class SkeletalSoftSubMesh : FileObject, ISkeletalSubMesh
    {
        public SkeletalSoftSubMesh()
        {
            _name = "SkeletalSoftSubMesh";
        }
        public SkeletalSoftSubMesh(
            string name,
            PrimitiveData primitives,
            Material material,
            bool visibleByDefault = true)
        {
            _primitives.File = primitives;
            _name = name;
            _material.File = material;
            _visibleByDefault = visibleByDefault;
        }
        
        protected SingleFileRef<PrimitiveData> _primitives = new SingleFileRef<PrimitiveData>();
        protected SingleFileRef<Material> _material = new SingleFileRef<Material>();
        protected bool _visibleByDefault = true;
        
        [TSerialize(Order = 0)]
        public SingleFileRef<Material> Material  => _material;
        [TSerialize(Order = 1)]
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null);
        [TSerialize(Order = 2)]
        public SingleFileRef<PrimitiveData> Primitives => _primitives;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
    }
}
