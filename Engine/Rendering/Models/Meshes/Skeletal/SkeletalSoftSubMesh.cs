using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public class SkeletalSoftSubMesh : FileObject, ISkeletalSubMesh
    {
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(ERenderPassType3D.OpaqueDeferredLit, null);

        public SkeletalSoftSubMesh() { }
        public SkeletalSoftSubMesh(PrimitiveData data, string name)
        {
            _data = data;
            _name = name;
        }
        
        protected PrimitiveData _data;
        protected Material _material;
        protected bool _visible;

        public bool VisibleByDefault => _visible;
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        public SingleFileRef<PrimitiveData> Data => _data;
    }
}
