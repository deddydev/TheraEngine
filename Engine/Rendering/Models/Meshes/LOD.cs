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
            GlobalFileRef<TMaterial> material,
            GlobalFileRef<PrimitiveData> primitives,
            float visibleDistance)
        {
            _material = material ?? new GlobalFileRef<TMaterial>();
            _primitives = primitives ?? new GlobalFileRef<PrimitiveData>();
            VisibleDistance = visibleDistance;
        }

        [Category("LOD")]
        public GlobalFileRef<TMaterial> Material => _material;
        [Category("LOD")]
        public GlobalFileRef<PrimitiveData> Primitives => _primitives;
        [Category("LOD")]
        [TSerialize]
        public float VisibleDistance { get; set; } = 0.0f;
        
        [TSerialize("Primitives")]
        protected GlobalFileRef<PrimitiveData> _primitives = new GlobalFileRef<PrimitiveData>();
        [TSerialize("Material")]
        protected GlobalFileRef<TMaterial> _material = new GlobalFileRef<TMaterial>();

        public PrimitiveManager CreatePrimitiveManager()
            => new PrimitiveManager(_primitives.File, _material.File);
    }
}
