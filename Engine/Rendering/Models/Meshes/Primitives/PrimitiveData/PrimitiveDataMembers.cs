using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public partial class PrimitiveData : FileObject, IDisposable
    {
        public bool HasSkinning => _utilizedBones == null ? false : _utilizedBones.Length > 0;
        public Culling Culling
        {
            get => _culling;
            set => _culling = value;
        }
        public string SingleBindBone
        {
            get => _singleBindBone;
            set => _singleBindBone = value;
        }

        [Browsable(false)]
        public VertexShaderDesc BufferInfo => _bufferInfo;

        /// <summary>
        /// TODO: move render state buffers to PrimitiveManager, but keep remapped buffer data here
        /// </summary>
        public List<VertexBuffer> Buffers { get => _buffers; set => _buffers = value; }

        public InfluenceDef[] Influences { get => _influences; set => _influences = value; }
        public string[] UtilizedBones { get => _utilizedBones; set => _utilizedBones = value; }
        public List<FacePoint> FacePoints { get => _facePoints; set => _facePoints = value; }
        public List<IndexTriangle> Triangles { get => _triangles; set => _triangles = value; }
        public List<IndexLine> Lines { get => _lines; set => _lines = value; }
        public List<IndexPoint> Points { get => _points; set => _points = value; }
        public EPrimitiveType Type { get => _type; set => _type = value; }

        //Skinning data first
        [TSerialize("SingleBindBone", Order = 0)]
        internal string _singleBindBone;
        [TSerialize("UtilizedBones", Order = 1)]
        internal string[] _utilizedBones;
        //Influence per raw vertex.
        //Count is same as _facePoints.Count
        [TSerialize("Influences", Order = 2)]
        internal InfluenceDef[] _influences;

        //Buffer data second
        [TSerialize("BufferInfo", Order = 3)]
        internal VertexShaderDesc _bufferInfo;
        //This is the array data that will be passed through the shader.
        //Each buffer may have repeated values, as there must be a value for each remapped face point.
        [TSerialize("VertexBuffers", Order = 4)]
        internal List<VertexBuffer> _buffers = null;

        //Face data last
        //Face points have indices that refer to each buffer.
        //These may contain repeat buffer indices but each point is unique.
        [TSerialize("FacePoints", Order = 5)]
        internal List<FacePoint> _facePoints = null;
        [TSerialize("Points", Order = 6)]
        internal List<IndexPoint> _points = null;
        [TSerialize("Lines", Order = 7)]
        internal List<IndexLine> _lines = null;
        //Faces have indices that refer to face points.
        //These may contain repeat vertex indices but each triangle is unique.
        [TSerialize("Triangles", Order = 8)]
        internal List<IndexTriangle> _triangles = null;

        [TSerialize("Type", XmlNodeType = EXmlNodeType.Attribute)]
        internal EPrimitiveType _type = EPrimitiveType.Triangles;
        [TSerialize("Culling", XmlNodeType = EXmlNodeType.Attribute)]
        internal Culling _culling = Culling.Back;
    }
}
