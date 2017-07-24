using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    [FileClass("OBJ", "Static Soft Sub Mesh")]
    public class StaticSoftSubMesh : FileObject, IStaticSubMesh
    {
        public RenderInfo3D RenderInfo { get; set; } = new RenderInfo3D(RenderPassType3D.OpaqueDeferredLit, null);

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

        [Serialize]
        public PrimitiveData Data
        {
            get => _data;
            set => _data = value;
        }
        [Serialize]
        public Shape CullingVolume
        {
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        [Serialize]
        public Material Material
        {
            get => _material;
            set => _material = value;
        }
        [Serialize(IsXmlAttribute = true)]
        public bool VisibleByDefault
        {
            get => _visibleByDefault;
            set => _visibleByDefault = value;
        }
    }
}
