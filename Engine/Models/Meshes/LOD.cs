using System;
using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Files;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public interface ILOD : IFileObject
    {
        event Action BillboardModeChanged;
        event Action VisibleDistanceChanged;
        event Action MaterialRefChanged;
        event Action PrimitivesRefChanged;

        LocalFileRef<TMaterial> MaterialRef { get; }
        GlobalFileRef<PrimitiveData> PrimitivesRef { get; }
        float VisibleDistance { get; set; }
        ETransformFlags TransformFlags { get; set; }

        PrimitiveManager CreatePrimitiveManager();
    }
    [TFileExt("lod")]
    [TFileDef("Level Of Detail Mesh Spec")]
    public class LOD : TFileObject, ILOD
    {
        public event Action BillboardModeChanged;
        public event Action VisibleDistanceChanged;
        public event Action MaterialRefChanged;
        public event Action PrimitivesRefChanged;

        public LOD() : this(null, null, 0) { }
        public LOD(
            LocalFileRef<TMaterial> material,
            GlobalFileRef<PrimitiveData> primitives,
            float visibleDistance)
        {
            _primitivesRef = primitives ?? new GlobalFileRef<PrimitiveData>();
            _materialRef = material ?? new LocalFileRef<TMaterial>();

            VisibleDistance = visibleDistance;
        }

        [Category("LOD")]
        [DisplayName("Material")]
        public LocalFileRef<TMaterial> MaterialRef => _materialRef;
        [Category("LOD")]
        [DisplayName("Primitives")]
        public GlobalFileRef<PrimitiveData> PrimitivesRef => _primitivesRef;
        [Category("LOD")]
        [TSerialize(IsAttribute = true)]
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
        [TSerialize(IsAttribute = true)]
        public ETransformFlags TransformFlags
        {
            get => _billboardMode;
            set
            {
                _billboardMode = value;
                BillboardModeChanged?.Invoke();
            }
        }

        [TSerialize("Primitives")]
        protected GlobalFileRef<PrimitiveData> _primitivesRef;
        [TSerialize("Material")]
        protected LocalFileRef<TMaterial> _materialRef;
        protected float _visibleDistance = 0.0f;
        protected ETransformFlags _billboardMode = ETransformFlags.None;

        public PrimitiveManager CreatePrimitiveManager()
        {
            PrimitiveManager m = new PrimitiveManager(_primitivesRef.File, _materialRef.File);
            m.BufferInfo.BillboardMode = TransformFlags;
            return m;
        }
    }
}