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
            _material = material ?? new SingleFileRef<TMaterial>();
            _primitives = primitives ?? new SingleFileRef<PrimitiveData>();
            VisibleDistance = visibleDistance;
        }

        [Category("LOD")]
        public SingleFileRef<TMaterial> Material => _material;
        [Category("LOD")]
        public SingleFileRef<PrimitiveData> Primitives => _primitives;
        [Category("LOD")]
        [TSerialize]
        public float VisibleDistance { get; set; } = 0.0f;
        
        [TSerialize("Primitives")]
        protected SingleFileRef<PrimitiveData> _primitives = new SingleFileRef<PrimitiveData>();
        [TSerialize("Material")]
        protected SingleFileRef<TMaterial> _material = new SingleFileRef<TMaterial>();

        public PrimitiveManager CreatePrimitiveManager()
            => new PrimitiveManager(_primitives.File, _material.File);
    }
}
