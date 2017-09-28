using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Animation;

namespace TheraEngine.Rendering.Models
{
    public class ModelImportOptions
    {
        [Category("Primitives")]
        public Culling DefaultCulling { get => _culling; set => _culling = value; }
        [Category("Primitives")]
        public bool ReverseWinding { get => _reverseWinding; set => _reverseWinding = value; }
        [Category("Primitives")]
        public float WeightPrecision { get { return _weightPrecision; } set { _weightPrecision = value.Clamp(0.0000001f, 0.999999f); } }
        [Category("Primitives")]
        public ETexWrapMode TexCoordWrap { get { return _wrap; } set { _wrap = value; } }
        [Category("Primitives")]
        public bool GenerateBinormals { get { return _generateBinormals; } set { _generateBinormals = value; } }
        [Category("Primitives")]
        public bool GenerateTangents { get { return _generateTangents; } set { _generateTangents = value; } }
        
        [Category("Compression")]
        public bool AllowVertexCompression { get { return _allowVertexCompression; } set { _allowVertexCompression = value; } }
        [Category("Compression")]
        public bool AllowNormalCompression { get { return _allowNormalCompression; } set { _allowNormalCompression = value; } }
        [Category("Compression")]
        public bool AllowTangentCompression { get { return _allowTangentCompression; } set { _allowTangentCompression = value; } }
        [Category("Compression")]
        public bool AllowBinormalCompression { get { return _allowBinormalCompression; } set { _allowBinormalCompression = value; } }
        [Category("Compression")]
        public bool AllowTexCoordCompression { get { return _allowTexCoordCompression; } set { _allowTexCoordCompression = value; } }
        [Category("Compression")]
        public bool AllowColorCompression { get { return _allowColorCompression; } set { _allowColorCompression = value; } }

        [Category("Tristripper")]
        public bool UseTristrips { get { return _useTristrips; } set { _useTristrips = value; } }
        [Category("Tristripper")]
        public uint CacheSize { get { return _cacheSize; } set { _cacheSize = value; } }
        [Category("Tristripper")]
        public uint MinimumStripLength { get { return _minStripLen; } set { _minStripLen = value < 2 ? 2 : value; } }
        [Category("Tristripper")]
        public bool PushCacheHits { get { return _pushCacheHits; } set { _pushCacheHits = value; } }

        [Category("Import")]
        public FrameState InitialTransform { get => _initialTransform; set => _initialTransform = value; }
        [Category("Import")]
        public bool ImportModels { get => _importModels; set => _importModels = value; }
        [Category("Import")]
        public bool ImportAnimations { get => _importAnimations; set => _importAnimations = value; }
        [Category("Import")]
        public bool UseForwardShaders { get => _useForwardShaders; set => _useForwardShaders = value; }

        //[Category("Tristripper")]
        //public bool BackwardSearch { get { return _backwardSearch; } set { _backwardSearch = value; } }

        private FrameState _initialTransform = FrameState.GetIdentity(TransformOrder.TRS, RotationOrder.YPR);

        private bool _allowVertexCompression = true;
        private bool _allowNormalCompression = true;
        private bool _allowTangentCompression = true;
        private bool _allowBinormalCompression = true;
        private bool _allowTexCoordCompression = true;
        private bool _allowColorCompression = true;

        private bool _generateBinormals, _generateTangents;

        private bool _importModels = true, _importAnimations = true;
        private uint _cacheSize = 52;
        private uint _minStripLen = 2;
        private bool _pushCacheHits = true;
        private bool _useTristrips = true;
        private bool _reverseWinding = false;
        private float _weightPrecision = 0.0001f;
        private ETexWrapMode _wrap = ETexWrapMode.Repeat;
        private Culling _culling = Culling.None;
        private bool _useForwardShaders;
        //private bool _backwardSearch = false; //Doesn't work
    }
    public class ModelScene
    {
        public SkeletalMesh SkeletalModel { get; set; }
        public StaticMesh StaticModel { get; set; }
        public Skeleton Skeleton { get; set; }
    }
}
