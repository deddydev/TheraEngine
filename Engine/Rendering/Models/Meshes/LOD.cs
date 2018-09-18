using System;
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
        public event Action BillboardModeChanged;
        public event Action VisibleDistanceChanged;
        public event Action MaterialRefChanged;
        public event Action PrimitivesRefChanged;
        
        public LOD()
        {
            _primitives = new GlobalFileRef<PrimitiveData>(Path.DirectorySeparatorChar.ToString());
            _material = new GlobalFileRef<TMaterial>(Path.DirectorySeparatorChar.ToString());
        }
        public LOD(
            GlobalFileRef<TMaterial> material,
            GlobalFileRef<PrimitiveData> primitives,
            float visibleDistance)
        {
            _material = material ?? new GlobalFileRef<TMaterial>(Path.DirectorySeparatorChar.ToString());
            _primitives = primitives ?? new GlobalFileRef<PrimitiveData>(Path.DirectorySeparatorChar.ToString());
            VisibleDistance = visibleDistance;
        }

        [Category("LOD")]
        public GlobalFileRef<TMaterial> MaterialRef => _material;
        [Category("LOD")]
        public GlobalFileRef<PrimitiveData> PrimitivesRef => _primitives;
        [Category("LOD")]
        public float VisibleDistance
        {
            get => _visibleDistance;
            set
            {
                _visibleDistance = value;
                VisibleDistanceChanged?.Invoke();
            }
        }
        [Category("LOD")]
        [TSerialize(IsXmlAttribute = true)]
        public EBillboardMode BillboardMode
        {
            get => _billboardMode;
            set
            {
                _billboardMode = value;
                BillboardModeChanged?.Invoke();
            }
        }

        [TSerialize("Primitives")]
        protected GlobalFileRef<PrimitiveData> _primitives;
        [TSerialize("Material")]
        protected GlobalFileRef<TMaterial> _material;
        [TSerialize(nameof(VisibleDistance), IsXmlAttribute = true)]
        protected float _visibleDistance = 0.0f;
        [TSerialize(nameof(BillboardMode), IsXmlAttribute = true)]
        protected EBillboardMode _billboardMode = EBillboardMode.None;

        public PrimitiveManager CreatePrimitiveManager()
            => new PrimitiveManager(_primitives.File, _material.File);
    }
}