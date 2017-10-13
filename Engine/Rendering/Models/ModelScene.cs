using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Animation;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models
{
    public class ModelImportOptions
    {
        [Category("Primitives")]
        public Culling DefaultCulling { get; set; } = Culling.Back;
        [Category("Primitives")]
        public bool ReverseWinding { get; set; } = false;
        [Category("Primitives")]
        public float WeightPrecision { get => _weightPrecision; set => _weightPrecision = value.Clamp(0.0000001f, 0.999999f); }
        [Category("Primitives")]
        public ETexWrapMode TexCoordWrap { get; set; } = ETexWrapMode.Repeat;
        [Category("Primitives")]
        public bool GenerateBinormals { get; set; } = true;
        [Category("Primitives")]
        public bool GenerateTangents { get; set; } = true;

        [Category("Compression")]
        public bool AllowVertexCompression { get; set; } = true;
        [Category("Compression")]
        public bool AllowNormalCompression { get; set; } = true;
        [Category("Compression")]
        public bool AllowTangentCompression { get; set; } = true;
        [Category("Compression")]
        public bool AllowBinormalCompression { get; set; } = true;
        [Category("Compression")]
        public bool AllowTexCoordCompression { get; set; } = true;
        [Category("Compression")]
        public bool AllowColorCompression { get; set; } = true;

        [Category("Tristripper")]
        public bool UseTristrips { get; set; } = true;
        [Category("Tristripper")]
        public uint CacheSize { get; set; } = 52;
        [Category("Tristripper")]
        public uint MinimumStripLength { get => _minStripLen; set => _minStripLen = value < 2 ? 2 : value; }
        [Category("Tristripper")]
        public bool PushCacheHits { get; set; } = true;

        [Category("Import")]
        public Transform InitialTransform { get; set; } = Transform.GetIdentity(TransformOrder.TRS, RotationOrder.YPR);
        [Category("Import")]
        public bool UseForwardShaders { get; set; }
        [Category("Import")]
        public IgnoreFlags IgnoreFlags { get; set; }
        
        private uint _minStripLen = 2;
        private float _weightPrecision = 0.0001f;
    }
    public class ModelScene
    {
        public SkeletalMesh SkeletalModel { get; set; }
        public StaticMesh StaticModel { get; set; }
        public Skeleton Skeleton { get; set; }
        public List<ModelAnimation> Animations { get; set; }
    }
}
