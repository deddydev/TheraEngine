using System;
using System.ComponentModel;
using TheraEngine.ComponentModel;
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
        GlobalFileRef<TMesh> PrimitivesRef { get; }
        float VisibleDistance { get; set; }
        ECameraTransformFlags TransformFlags { get; set; }

        MeshRenderer CreatePrimitiveManager();
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
            GlobalFileRef<TMesh> primitives,
            float visibleDistance)
        {
            _primitivesRef = primitives ?? new GlobalFileRef<TMesh>();
            _materialRef = material ?? new LocalFileRef<TMaterial>();

            VisibleDistance = visibleDistance;
        }

        [Category("LOD")]
        [DisplayName("Material")]
        public LocalFileRef<TMaterial> MaterialRef => _materialRef;
        [Category("LOD")]
        [DisplayName("Primitives")]
        public GlobalFileRef<TMesh> PrimitivesRef => _primitivesRef;
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
        /// <summary>
        /// Determines how this LOD will move in relation to the camera.
        /// </summary>
        [Category("LOD")]
        [TSerialize(IsAttribute = true)]
        [Description("Determines how this LOD will move in relation to the camera.")]
        public ECameraTransformFlags TransformFlags
        {
            get => _billboardMode;
            set
            {
                _billboardMode = value;
                BillboardModeChanged?.Invoke();
            }
        }

        [TSerialize("Primitives")]
        protected GlobalFileRef<TMesh> _primitivesRef;
        [TSerialize("Material")]
        protected LocalFileRef<TMaterial> _materialRef;

        protected float _visibleDistance = 0.0f;
        protected ECameraTransformFlags _billboardMode = ECameraTransformFlags.None;

        public MeshRenderer CreatePrimitiveManager()
        {
            MeshRenderer m = new MeshRenderer(_primitivesRef.File, _materialRef.File);
            m.BufferInfo.CameraTransformFlags = TransformFlags;
            return m;
        }
    }
}