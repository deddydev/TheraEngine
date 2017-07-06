using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public class SkeletalSoftSubMesh : FileObject, ISkeletalSubMesh
    {
        public SkeletalSoftSubMesh() { }
        public SkeletalSoftSubMesh(PrimitiveData data, string name)
        {
            _data = data;
            _name = name;
        }

        protected SkeletalMesh _parent;
        protected PrimitiveData _data;
        protected Material _material;
        protected bool _visible;

        public bool Visible => _visible;
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        public SingleFileRef<PrimitiveData> Data => _data;
        public SkeletalMesh Model
        {
            get => _parent;
            internal set => _parent = value;
        }
    }
}
