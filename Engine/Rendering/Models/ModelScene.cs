using System;
using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;
using static TheraEngine.Rendering.Models.Collada;

namespace TheraEngine.Rendering.Models
{
    [FileDef("Model Importing Options")]
    public class ColladaImportOptions : TSettings
    {
        [TSerialize(UseCategory = true)]
        [Category("Primitives")]
        public RenderingParameters DefaultMaterialParameters { get => _renderParams; set => _renderParams = value ?? new RenderingParameters(); }
        [TSerialize(UseCategory = true)]
        [Category("Primitives")]
        public bool ReverseWinding { get; set; } = false;
        [TSerialize(UseCategory = true)]
        [Category("Primitives")]
        public float WeightPrecision { get => _weightPrecision; set => _weightPrecision = value.Clamp(0.0000001f, 0.999999f); }
        [TSerialize(UseCategory = true)]
        [Category("Primitives")]
        public ETexWrapMode TexCoordWrap { get; set; } = ETexWrapMode.Repeat;
        [TSerialize(UseCategory = true)]
        [Category("Primitives")]
        public bool GenerateBinormals { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Primitives")]
        public bool GenerateTangents { get; set; } = true;

        [TSerialize(UseCategory = true)]
        [Category("Compression")]
        public bool AllowVertexCompression { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Compression")]
        public bool AllowNormalCompression { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Compression")]
        public bool AllowTangentCompression { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Compression")]
        public bool AllowBinormalCompression { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Compression")]
        public bool AllowTexCoordCompression { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Compression")]
        public bool AllowColorCompression { get; set; } = true;

        [TSerialize(UseCategory = true)]
        [Category("Tristripper")]
        public bool UseTristrips { get; set; } = true;
        [TSerialize(UseCategory = true)]
        [Category("Tristripper")]
        public uint CacheSize { get; set; } = 52;
        [TSerialize(UseCategory = true)]
        [Category("Tristripper")]
        public uint MinimumStripLength { get => _minStripLen; set => _minStripLen = value < 2 ? 2 : value; }
        [TSerialize(UseCategory = true)]
        [Category("Tristripper")]
        public bool PushCacheHits { get; set; } = true;

        /// <summary>
        /// Determines how the model should be scaled, rotated and translated.
        /// </summary>
        [TSerialize(UseCategory = true)]
        [Description("Determines how the model should be scaled, rotated and translated.")]
        [Category("Import")]
        public Transform InitialTransform { get; set; } = Transform.GetIdentity();

        /// <summary>
        /// Determines if the material should be generated for a forward or deferred rendering pipeline. Deferred is default.
        /// </summary>
        [TSerialize(UseCategory = true)]
        [Description("Determines if the material should be generated for a forward or deferred rendering pipeline. Deferred is default.")]
        [Category("Import")]
        public bool UseForwardShaders { get; set; } = false;

        /// <summary>
        /// Dictates what information to ignore.
        /// Ignoring certain elements can give a decent parsing speed boost.
        /// </summary>
        [TSerialize(UseCategory = true)]
        [Description("Dictates what information to ignore. " +
            "Ignoring certain elements can give a decent parsing speed boost.")]
        [Category("Import")]
        public EIgnoreFlags IgnoreFlags { get; set; } = EIgnoreFlags.None;
        
        private uint _minStripLen = 2;
        private float _weightPrecision = 0.0001f;
        private RenderingParameters _renderParams = new RenderingParameters();
    }
    public class ModelScene
    {
        public SkeletalModel SkeletalModel { get; set; }
        public StaticModel StaticModel { get; set; }
        public Skeleton Skeleton { get; set; }
        public SkeletalAnimation Animation { get; set; }
    }
}
