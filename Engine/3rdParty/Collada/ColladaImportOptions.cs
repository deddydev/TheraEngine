using System;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        public class ImportOptions
        {
            [Category("Primitives")]
            public Culling DefaultCulling { get => _culling; set => _culling = value; }
            [Category("Primitives")]
            public bool ReverseWinding { get => _reverseWinding; set => _reverseWinding = value; }
            [Category("Primitives")]
            public float WeightPrecision { get { return _weightPrecision; } set { _weightPrecision = value.Clamp(0.0000001f, 0.999999f); } }
            [Category("Primitives")]
            public TexCoordWrap TexCoordWrap { get { return _wrap; } set { _wrap = value; } }

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

            public FrameState InitialTransform { get => _initialTransform; set => _initialTransform = value; }

            //[Category("Tristripper")]
            //public bool BackwardSearch { get { return _backwardSearch; } set { _backwardSearch = value; } }

            private FrameState _initialTransform = FrameState.GetIdentity(TransformOrder.TRS, RotationOrder.YPR);

            public bool _allowVertexCompression = true;
            public bool _allowNormalCompression = true;
            public bool _allowTangentCompression = true;
            public bool _allowBinormalCompression = true;
            public bool _allowTexCoordCompression = true;
            public bool _allowColorCompression = true;

            public uint _cacheSize = 52;
            public uint _minStripLen = 2;
            public bool _pushCacheHits = true;
            public bool _useTristrips = true;
            public bool _reverseWinding = false;
            public float _weightPrecision = 0.0001f;
            public TexCoordWrap _wrap = TexCoordWrap.Repeat;
            public Culling _culling = Culling.None;
            public bool _backwardSearch = false; //Doesn't work
        }
    }
}
