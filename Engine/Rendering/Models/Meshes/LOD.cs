using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [FileClass("LOD", "Level Of Detail Mesh Spec")]
    public class LOD : FileObject
    {
        public LOD() { }
        public LOD(
            SingleFileRef<TMaterial> material,
            SingleFileRef<PrimitiveData> primitives,
            float visibleDistance)
        {
            _material = material;
            _primitives = primitives;
            _visibleDistance = visibleDistance;
        }

        public SingleFileRef<TMaterial> Material => _material;
        public SingleFileRef<PrimitiveData> Primitives => _primitives;
        public float VisibleDistance
        {
            get => _visibleDistance;
            set => _visibleDistance = value;
        }

        [Category("LOD")]
        [TSerialize("VisibleDistance")]
        private float _visibleDistance;
        [Category("LOD")]
        [TSerialize("Primitives")]
        protected SingleFileRef<PrimitiveData> _primitives = new SingleFileRef<PrimitiveData>();
        [Category("LOD")]
        [TSerialize("Material")]
        protected SingleFileRef<TMaterial> _material = new SingleFileRef<TMaterial>();

        public PrimitiveManager CreatePrimitiveManager()
            => new PrimitiveManager(_primitives.File, _material.File);
    }
}
