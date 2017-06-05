using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public enum Culling
    {
        None,
        Back,
        Front,
        Both
    }
    public enum EPrimitiveType
    {
        Points                  = 0,
        Lines                   = 1,
        LineLoop                = 2,
        LineStrip               = 3,
        Triangles               = 4,
        TriangleStrip           = 5,
        TriangleFan             = 6,
        Quads                   = 7,
        QuadStrip               = 8,
        Polygon                 = 9,
        LinesAdjacency          = 10,
        LineStripAdjacency      = 11,
        TrianglesAdjacency      = 12,
        TriangleStripAdjacency  = 13,
        Patches                 = 14,
    }
    public class PrimitiveBufferInfo
    {
        public int _morphCount = 0;
        public int _texcoordCount = 1;
        public int _colorCount = 0;
        public int _boneCount = 0;
        public bool _hasBarycentricCoord = false;
        public bool _hasNormals = true, _hasBinormals = false, _hasTangents = false;

        public bool IsWeighted => _boneCount > 1;
        public bool HasNormals => _hasNormals;
        public bool HasBinormals => _hasBinormals;
        public bool HasTangents => _hasTangents;
        public bool HasTexCoords => _texcoordCount > 0;
        public bool HasColors => _colorCount > 0;

        public static PrimitiveBufferInfo PosNormTex1()
        {
            return new PrimitiveBufferInfo() { _texcoordCount = 1, _hasNormals = true };
        }
        public static PrimitiveBufferInfo JustPositions()
        {
            return new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false };
        }
    }
    [FileClass("PRIM", "Mesh Data")]
    public class PrimitiveData : FileObject, IDisposable
    {
        public bool HasSkinning => _utilizedBones == null ? false : _utilizedBones.Length > 0;
        public Culling Culling
        {
            get => _culling;
            set => _culling = value;
        }

        //Faces have indices that refer to face points.
        //These may contain repeat vertex indices but each triangle is unique.
        [Serialize("Triangles", Order = 7)]
        public List<IndexTriangle> _triangles = null;
        [Serialize("Lines", Order = 6)]
        public List<IndexLine> _lines = null;
        [Serialize("Points", Order = 5)]
        public List<IndexPoint> _points = null;
        public EPrimitiveType _type;
        
        //Influence per raw vertex.
        //Count is same as _facePoints.Count
        [Serialize("Influences", Order = 2)]
        public Influence[] _influences;
        [Serialize("UtilizedBones", Order = 1)]
        public string[] _utilizedBones;
        [Serialize("SingleBindBone", Order = 0)]
        public string _singleBindBone;

        //Face points have indices that refer to each buffer.
        //These may contain repeat buffer indices but each point is unique.
        [Serialize("FacePoints", Order = 4)]
        public List<FacePoint> _facePoints = null;

        //This is the array data that will be passed through the shader.
        //Each buffer may have repeated values, as there must be a value for each remapped face point.
        [Serialize("VertexBuffers", Order = 3)]
        private List<VertexBuffer> _buffers = null;

        [Serialize("Culling", IsXmlAttribute = true)]
        private Culling _culling = Culling.Back;

        public VertexBuffer this[BufferType type]
        {
            get => _buffers.FirstOrDefault(x => x.BufferType == type);
            set
            {
                //value.Name = _type.ToString();
                value.BufferType = type;
                var buf = _buffers.FirstOrDefault(x => x.BufferType == type);
                if (buf != null)
                {
                    value.Index = buf.Index;
                    _buffers[buf.Index] = value;
                }
                else
                {
                    value.Index = _buffers.Count;
                    _buffers.Add(value);
                }
            }
        }
        public VertexBuffer this[string name]
        {
            get => _buffers.FirstOrDefault(x => x.Name == name);
            set
            {
                var buf = _buffers.FirstOrDefault(x => x.Name == name);
                if (buf != null)
                {
                    value.Index = buf.Index;
                    _buffers[buf.Index] = value;
                }
                else
                {
                    value.Index = _buffers.Count;
                    _buffers.Add(value);
                }
            }
        }
        public VertexBuffer this[int index]
        {
            get
            {
                if (index < 0 || index >= _buffers.Count)
                    throw new IndexOutOfRangeException();
                return _buffers[index];
            }
            set
            {
                if (index >= 0 && index < _buffers.Count)
                    _buffers[index] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }
        public int[] GenerateBuffers(int vaoId)
        {
            List<int> bindingIds = new List<int>();
            foreach (VertexBuffer b in _buffers)
            {
                if (b == null)
                {
                    //bindingIds.Add(0);
                    continue;
                }
                b._vaoId = vaoId;
                bindingIds.Add(b.Generate());
            }
            return bindingIds.ToArray();
        }
        public int[] GetIndices()
        {
            switch (_type)
            {
                case EPrimitiveType.Triangles:
                    return _triangles?.SelectMany(x => new int[] { x.Point0, x.Point1, x.Point2 }).ToArray();
                case EPrimitiveType.Lines:
                    return _lines?.SelectMany(x => new int[] { x.Point0, x.Point1 }).ToArray();
                case EPrimitiveType.Points:
                    return _points?.Select(x => (int)x).ToArray();
            }
            return null;
        }
        public VertexTriangle GetFace(int index)
        {
            IndexTriangle t = _triangles[index];
            FacePoint fp0 = _facePoints[t.Point0];
            FacePoint fp1 = _facePoints[t.Point1];
            FacePoint fp2 = _facePoints[t.Point2];
            Vertex v0 = new Vertex(fp0, _buffers);
            Vertex v1 = new Vertex(fp1, _buffers);
            Vertex v2 = new Vertex(fp2, _buffers);
            return new VertexTriangle(v0, v1, v2);
        }
        private void SetInfluences(params Influence[] influences)
        {
            Remapper remap = new Remapper();
            remap.Remap(influences);
            for (int i = 0; i < remap.RemapTable.Length; ++i)
                _facePoints[i]._influenceIndex = remap.RemapTable[i];
            _influences = new Influence[remap.ImplementationLength];
            for (int i = 0; i < remap.ImplementationLength; ++i)
                _influences[i] = influences[remap.ImplementationTable[i]];

            HashSet<string> utilized = new HashSet<string>();
            foreach (Influence inf in _influences)
                if (inf != null)
                    for (int i = 0; i < inf.WeightCount; ++i)
                        utilized.Add(inf.Weights[i].Bone);
            _utilizedBones = utilized.ToArray();
            if (_utilizedBones.Length == 1)
            {
                _singleBindBone = _utilizedBones[0];
                _utilizedBones = null;
            }
        }

        private void CreateFacePoints(int pointCount)
        {
            _facePoints = new List<FacePoint>();
            for (int i = 0; i < pointCount; ++i)
                _facePoints.Add(new FacePoint(i, this));
        }
        public VertexBuffer FindBuffer(string name)
        {
            if (_buffers != null)
                foreach (VertexBuffer b in _buffers)
                    if (b.Name == name)
                        return b;
            return null;
        }
        public void AddBufferNumeric<T>(
            IList<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : struct
        {
            if (_buffers == null)
                _buffers = new List<VertexBuffer>();

            int bufferIndex = _buffers.Count;
            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetDataNumeric(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetDataNumeric(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(i);
            }
            _buffers.Add(buffer);
        }
        public void ReplaceBufferNumeric<T>(
            IList<T> bufferData,
            int bufferIndex,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : struct
        {
            if (_buffers == null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetDataNumeric(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetDataNumeric(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
        }
        public void AddBuffer<T>(
            IList<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : IBufferable
        {
            if (_buffers == null)
                _buffers = new List<VertexBuffer>();

            int bufferIndex = _buffers.Count;
            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetData(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices.Add(i);
            }
            _buffers.Add(buffer);
        }
        public void ReplaceBuffer<T>(
            IList<T> bufferData,
            int bufferIndex,
            VertexAttribInfo info,
            bool remap = false,
            bool integral = false,
            EBufferTarget target = EBufferTarget.DataArray) where T : IBufferable
        {
            if (_buffers == null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target, integral);
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetData(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].BufferIndices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
        }

        public Remapper GetBuffer<T>(int bufferIndex, out T[] array, bool remap = false) where T : IBufferable
        {
            if (_buffers == null || bufferIndex < 0 || bufferIndex >= _buffers.Count)
            {
                array = null;
                return null;
            }
            return _buffers[bufferIndex].GetData(out array, remap);
        }

        private Remapper SetTriangleIndices(List<Vertex> vertices, bool remap = true)
        {
            if (vertices.Count % 3 != 0)
                throw new Exception("Vertex list needs to be a multiple of 3.");

            _triangles = new List<IndexTriangle>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _triangles.Add(new IndexTriangle(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _triangles.Add(new IndexTriangle(i++, i++, i++));
                return null;
            }
        }
        private Remapper SetLineIndices(List<Vertex> vertices, bool remap = true)
        {
            if (vertices.Count % 2 != 0)
                throw new Exception("Vertex list needs to be a multiple of 2.");

            _lines = new List<IndexLine>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _lines.Add(new IndexLine(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _lines.Add(new IndexLine(i++, i++));
                return null;
            }
        }
        private Remapper SetPointIndices(List<Vertex> vertices, bool remap = true)
        {
            _points = new List<IndexPoint>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                    _points.Add(new IndexPoint(remapper.RemapTable[i++]));
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count;)
                    _points.Add(new IndexPoint(i++));
                return null;
            }
        }

        public static PrimitiveData FromQuads(Culling culling, PrimitiveBufferInfo info, params VertexQuad[] quads)
        {
            return FromQuadList(culling, info, quads);
        }
        public static PrimitiveData FromQuadList(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexQuad> quads)
        {
            return FromTriangleList(culling, info, quads.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangleStrips(Culling culling, PrimitiveBufferInfo info, params VertexTriangleStrip[] strips)
        {
            return FromTriangleStripList(culling, info, strips);
        }
        public static PrimitiveData FromTriangleStripList(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexTriangleStrip> strips)
        {
            return FromTriangleList(culling, info, strips.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangleFans(Culling culling, PrimitiveBufferInfo info, params VertexTriangleFan[] fans)
        {
            return FromTriangleFanList(culling, info, fans);
        }
        public static PrimitiveData FromTriangleFanList(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexTriangleFan> fans)
        {
            return FromTriangleList(culling, info, fans.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangles(Culling culling, PrimitiveBufferInfo info, params VertexTriangle[] triangles)
        {
            return FromTriangleList(culling, info, triangles);
        }
        public static PrimitiveData FromTriangleList(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexTriangle> triangles)
        {
            //TODO: convert triangles to tristrips and use primitive restart to render them all in one call
            return new PrimitiveData(culling, info, triangles.SelectMany(x => x.Vertices), EPrimitiveType.Triangles);
        }
        public static PrimitiveData FromLineStrips(PrimitiveBufferInfo info, params VertexLineStrip[] lines)
        {
            return FromLineStripList(info, lines);
        }
        public static PrimitiveData FromLineStripList(PrimitiveBufferInfo info, IEnumerable<VertexLineStrip> lines)
        {
            return FromLineList(info, lines.SelectMany(x => x.ToLines()));
        }
        public static PrimitiveData FromLines(PrimitiveBufferInfo info, params VertexLine[] lines)
        {
            return FromLineList(info, lines);
        }
        public static PrimitiveData FromLineList(PrimitiveBufferInfo info, IEnumerable<VertexLine> lines)
        {
            return new PrimitiveData(Culling.None, info, lines.SelectMany(x => x.Vertices), EPrimitiveType.Lines);
        }
        public static PrimitiveData FromPoints(params Vec3[] points)
        {
            return FromPointList(points);
        }
        public static PrimitiveData FromPointList(IEnumerable<Vec3> points)
        {
            return new PrimitiveData(Culling.None, PrimitiveBufferInfo.JustPositions(), points.Select(x => new Vertex(x)), EPrimitiveType.Points);
        }

        public PrimitiveData(Culling culling, PrimitiveBufferInfo info, IEnumerable<Vertex> points, EPrimitiveType type)
        {
            _type = type;
            _influences = null;
            _culling = culling;

            List<Vertex> vertices = points.ToList();
            Remapper remapper = null;
            switch (_type)
            {
                case EPrimitiveType.Triangles:
                    remapper = SetTriangleIndices(vertices);
                    break;
                case EPrimitiveType.Lines:
                    remapper = SetLineIndices(vertices);
                    break;
                case EPrimitiveType.Points:
                    remapper = SetPointIndices(vertices);
                    break;
            }
            
            CreateFacePoints(remapper.ImplementationLength);

            if (info.IsWeighted)
                SetInfluences(remapper.ImplementationTable.Select(x => vertices[x]._influence).ToArray());

            for (int i = 0; i < info._morphCount + 1; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._position).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Position, i));
            }
            if (info.HasNormals)
                for (int i = 0; i < info._morphCount + 1; ++i)
                {
                    var data = remapper.ImplementationTable.Select(x => vertices[x]._normal).ToList();
                    AddBuffer(data, new VertexAttribInfo(BufferType.Normal, i));
                }
            if (info.HasBinormals)
                for (int i = 0; i < info._morphCount + 1; ++i)
                {
                    var data = remapper.ImplementationTable.Select(x => vertices[x]._binormal).ToList();
                    AddBuffer(data, new VertexAttribInfo(BufferType.Binormal, i));
                }
            if (info.HasTangents)
                for (int i = 0; i < info._morphCount + 1; ++i)
                {
                    var data = remapper.ImplementationTable.Select(x => vertices[x]._tangent).ToList();
                    AddBuffer(data, new VertexAttribInfo(BufferType.Tangent, i));
                }
            for (int i = 0; i < info._colorCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._color).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Color, i));
            }
            for (int i = 0; i < info._texcoordCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._texCoord).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.TexCoord, i));
            }
            if (info._hasBarycentricCoord)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._barycentric).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Barycentric, 0));
            }
        }

        public void Dispose()
        {
            _buffers?.ForEach(x => x.Dispose());
        }
        [CustomXMLDeserializeMethod("Triangles")]
        private bool CustomTrianglesSerialize(XmlWriter writer)
        {
            if (_triangles != null)
                writer.WriteElementString("Triangles", string.Join(" ", _triangles.SelectMany(x => x.Points.Select(y => y.VertexIndex))));
            return true;
        }
        [CustomXMLDeserializeMethod("Triangles")]
        private bool CustomTrianglesDeserialize(XMLReader reader)
        {
            string values = reader.ReadElementString();
            int[] points = values.Split(' ').Select(x => int.Parse(x)).ToArray();
            _triangles = new List<IndexTriangle>(points.Length / 3);
            for (int i = 0; i < points.Length; )
                _triangles.Add(new IndexTriangle(points[i++], points[i++], points[i++]));
            return true;
        }
        [CustomXMLSerializeMethod("Lines")]
        private bool CustomLinesSerialize(XmlWriter writer)
        {
            if (_lines != null)
                writer.WriteElementString("Lines", string.Join(" ", _lines.SelectMany(x => new int[] { x.Point0.VertexIndex, x.Point1.VertexIndex })));
            return true;
        }
        [CustomXMLDeserializeMethod("Lines")]
        private bool CustomLinesDeserialize(XMLReader reader)
        {
            string values = reader.ReadElementString();
            int[] points = values.Split(' ').Select(x => int.Parse(x)).ToArray();
            _triangles = new List<IndexTriangle>(points.Length / 2);
            for (int i = 0; i < points.Length;)
                _lines.Add(new IndexLine(points[i++], points[i++]));
            return true;
        }
        [CustomXMLSerializeMethod("Points")]
        private bool CustomPointsSerialize(XmlWriter writer)
        {
            if (_points != null)
                writer.WriteElementString("Points", string.Join(" ", _points.Select(x => x.VertexIndex)));
            return true;
        }
        [CustomXMLDeserializeMethod("Points")]
        private bool CustomPointsDeserialize(XMLReader reader)
        {
            string values = reader.ReadElementString();
            int[] points = values.Split(' ').Select(x => int.Parse(x)).ToArray();
            _triangles = new List<IndexTriangle>(points.Length);
            for (int i = 0; i < points.Length;)
                _points.Add(points[i++]);
            return true;
        }
        [CustomXMLSerializeMethod("FacePoints")]
        private bool CustomFacePointsSerialize(XmlWriter writer)
        {
            writer.WriteStartElement("FacePoints");
            writer.WriteAttributeString("Count", _facePoints.Count.ToString());
            {
                bool hasInfs = _influences != null && _influences.Length > 0;
                foreach (FacePoint p in _facePoints)
                {
                    if (hasInfs)
                        writer.WriteString(p._influenceIndex.ToString() + " ");
                    foreach (int i in p.BufferIndices)
                        writer.WriteString(i.ToString() + " ");
                }
            }
            writer.WriteEndElement();
            return true;
        }
        [CustomXMLDeserializeMethod("FacePoints")]
        private bool CustomFacePointsDeserialize(XMLReader reader)
        {
            int count = 0;
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("Count", true))
                    count = reader.Value;
            }
            bool hasInfs = _influences != null && _influences.Length > 0;
            int bufferCount = _buffers.Count;
            int valuesPerPoint = bufferCount + (hasInfs ? 1 : 0);
            string values = reader.ReadElementString();
            int[] points = values.Split(' ').Select(x => int.Parse(x)).ToArray();
            _facePoints = new List<FacePoint>(points.Length / valuesPerPoint);
            for (int i = 0, x = 0; x < points.Length; ++i)
            {
                FacePoint p = new FacePoint(i, this);
                if (hasInfs)
                    p._influenceIndex = points[x++];
                for (int r = 0; r < bufferCount; ++r)
                    p.BufferIndices.Add(points[x++]);
            }
            return true;
        }
        [CustomXMLSerializeMethod("Influences")]
        private bool CustomInfluencesSerialize(XmlWriter writer)
        {
            if (_influences != null)
            {
                writer.WriteStartElement("Influences");
                writer.WriteAttributeString("Count", _influences.Length.ToString());
                {
                    writer.WriteStartElement("Counts");
                    foreach (Influence inf in _influences)
                    {
                        writer.WriteString(inf.WeightCount.ToString() + " ");
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("Indices");
                    foreach (Influence inf in _influences)
                    {
                        for (int i = 0; i < inf.WeightCount; ++i)
                        {
                            writer.WriteString(_utilizedBones.IndexOf(inf.Weights[i].Bone).ToString() + " ");
                        }
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("Weights");
                    foreach (Influence inf in _influences)
                    {
                        for (int i = 0; i < inf.WeightCount; ++i)
                        {
                            writer.WriteString(inf.Weights[i].Weight.ToString() + " ");
                        }
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
            return true;
        }
        [CustomXMLDeserializeMethod("Influences")]
        private bool CustomInfluencesDeserialize(XMLReader reader)
        {
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("Count", true))
                {
                    int count = int.Parse(reader.Value);
                    _influences = new Influence[count];
                }
            }
            int[] counts = null, indices = null;
            float[] weights = null;
            string s;
            while (reader.BeginElement())
            {
                s = reader.ReadElementString();
                if (reader.Name.Equals("Counts", true))
                    counts = s.Split(' ').Select(x => int.Parse(x)).ToArray();
                else if (reader.Name.Equals("Indices", true))
                    indices = s.Split(' ').Select(x => int.Parse(x)).ToArray();
                else if (reader.Name.Equals("Weights", true))
                    weights = s.Split(' ').Select(x => float.Parse(x)).ToArray();
                reader.EndElement();
            }
            int k = 0;
            for (int i = 0; i < _influences.Length; ++i)
            {
                Influence inf = new Influence();
                for (int j = 0; j < counts[i]; ++j, ++k)
                    inf.AddWeight(new BoneWeight(_utilizedBones[indices[k]], weights[k]));
                _influences[i] = inf;
            }
            return true;
        }
    }
}
