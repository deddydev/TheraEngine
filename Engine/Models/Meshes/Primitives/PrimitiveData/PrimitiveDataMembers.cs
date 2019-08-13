using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models
{
    public partial class PrimitiveData : TFileObject, IDisposable
    {
        public event Action BufferInfoChanged;

        [Browsable(false)]
        public bool HasSkinning => _utilizedBones == null ? false : _utilizedBones.Length > 0;
        [Browsable(false)]
        public string SingleBindBone
        {
            get => _singleBindBone;
            set => _singleBindBone = value;
        }
        /// <summary>
        /// TODO: move render state buffers to PrimitiveManager, but keep remapped buffer data here
        /// </summary>
        [Browsable(false)]
        public List<DataBuffer> Buffers { get => _buffers; set => _buffers = value; }
        [Browsable(false)]
        public InfluenceDef[] Influences { get => _influences; set => _influences = value; }
        [Browsable(false)]
        public string[] UtilizedBones { get => _utilizedBones; set => _utilizedBones = value; }
        [Browsable(false)]
        public List<FacePoint> FacePoints { get => _facePoints; set => _facePoints = value; }
        [Browsable(false)]
        public List<IndexTriangle> Triangles { get => _triangles; set => _triangles = value; }
        [Browsable(false)]
        public List<IndexLine> Lines { get => _lines; set => _lines = value; }
        [Browsable(false)]
        public List<IndexPoint> Points { get => _points; set => _points = value; }
        [Browsable(false)]
        public EPrimitiveType Type { get => _type; set => _type = value; }
        [Browsable(false)]
        public VertexShaderDesc BufferInfo => _bufferInfo;

        //Skinning data first
        [TSerialize(nameof(SingleBindBone), Order = 0)]
        public string _singleBindBone;
        [TSerialize(nameof(UtilizedBones), Order = 1)]
        public string[] _utilizedBones;
        //Influence per raw vertex.
        //Count is same as _facePoints.Count
        [TSerialize(nameof(Influences), Order = 2)]
        public InfluenceDef[] _influences;

        //Buffer data second
        [TSerialize(nameof(BufferInfo), Order = 3)]
        public VertexShaderDesc _bufferInfo;
        //This is the array data that will be passed through the shader.
        //Each buffer may have repeated values, as there must be a value for each remapped face point.
        [TSerialize(nameof(Buffers), Order = 4)]
        public List<DataBuffer> _buffers = null;

        //Face data last
        //Face points have indices that refer to each buffer.
        //These may contain repeat buffer indices but each point is unique.
        [TSerialize(nameof(FacePoints), Order = 5)]
        public List<FacePoint> _facePoints = null;
        [TSerialize(nameof(Points), Order = 6)]
        public List<IndexPoint> _points = null;
        [TSerialize(nameof(Lines), Order = 7)]
        public List<IndexLine> _lines = null;
        //Faces have indices that refer to face points.
        //These may contain repeat vertex indices but each triangle is unique.
        [TSerialize(nameof(Triangles), Order = 8)]
        public List<IndexTriangle> _triangles = null;

        [TSerialize(nameof(Type), NodeType = ENodeType.Attribute)]
        public EPrimitiveType _type = EPrimitiveType.Triangles;
    }
}
