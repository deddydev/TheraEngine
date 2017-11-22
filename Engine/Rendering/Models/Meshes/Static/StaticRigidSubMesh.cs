using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.Models
{
    [FileClass("STRMESH", "Static Rigid Sub Mesh")]
    public class StaticRigidSubMesh : FileObject, IStaticSubMesh
    {
        public StaticRigidSubMesh()
        {
            _name = "StaticRigidSubMesh";
        }
        public StaticRigidSubMesh(
            string name, 
            PrimitiveData primitives,
            Shape cullingVolume,
            Material material,
            bool visibleByDefault = true)
        {
            _cullingVolume.File = cullingVolume;
            _material.File = material;
            _primitives.File = primitives;
            _name = name;
            _visibleByDefault = visibleByDefault;
        }
        
        protected SingleFileRef<PrimitiveData> _primitives = new SingleFileRef<PrimitiveData>();
        protected SingleFileRef<Material> _material = new SingleFileRef<Material>();
        protected SingleFileRef<Shape> _cullingVolume = new SingleFileRef<Shape>();
        protected bool _visibleByDefault = true;

        [TSerialize(Order = 0)]
        public SingleFileRef<Shape> CullingVolume => _cullingVolume;
        [TSerialize(Order = 1)]
        public SingleFileRef<Material> Material => _material;
        [TSerialize(Order = 2)]
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPass3D.OpaqueDeferredLit, null);
        [TSerialize(Order = 3)]
        public SingleFileRef<PrimitiveData> Primitives  => _primitives;
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
    }
}
