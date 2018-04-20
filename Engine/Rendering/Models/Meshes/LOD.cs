using System.ComponentModel;
using System.IO;
using TheraEngine.Files;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    [FileExt("lod")]
    [FileDef("Level Of Detail Mesh Spec")]
    public class LOD : TFileObject
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
        public GlobalFileRef<TMaterial> MaterialRef => _material;
        [Category("LOD")]
        public GlobalFileRef<PrimitiveData> PrimitivesRef => _primitives;
        [Category("LOD")]
        [TSerialize(IsXmlAttribute = true)]
        public float VisibleDistance { get; set; } = 0.0f;
        
        [TSerialize("Primitives")]
        protected GlobalFileRef<PrimitiveData> _primitives = new GlobalFileRef<PrimitiveData>(Path.DirectorySeparatorChar.ToString());
        [TSerialize("Material")]
        protected GlobalFileRef<TMaterial> _material = new GlobalFileRef<TMaterial>(Path.DirectorySeparatorChar.ToString());

        public PrimitiveManager CreatePrimitiveManager()
            => new PrimitiveManager(_primitives.File, _material.File);
    }
}