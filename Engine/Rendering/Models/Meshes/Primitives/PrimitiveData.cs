using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Rendering.Models
{
    public enum Culling
    {
        None,
        Back,
        Front,
        Both
    }
    public class PrimitiveBufferInfo
    {
        public int _positionCount = 1;
        public int _normalCount = 1;
        public int _binormalCount = 0;
        public int _tangentCount = 0;
        public int _texcoordCount = 1;
        public int _colorCount = 0;
        public int _boneCount = 0;
    }
    public class PrimitiveData : IDisposable
    {
        public bool HasSkinning { get { return _utilizedBones.Length > 0; } }
        public Culling Culling { get { return _culling; } set { _culling = value; } }

        //Faces have indices that refer to face points.
        //These may contain repeat vertex indices but each triangle is unique.
        public List<IndexTriangle> _faces = null;
        //Contains the human-understandable mesh information.
        MonitoredList<VertexTriangle> _triangles;

        //Influence per raw vertex.
        //Count is same as _facePoints.Count
        public Influence[] _influences;
        public string[] _utilizedBones;

        //Face points have indices that refer to each buffer.
        //These may contain repeat buffer indices but each point is unique.
        public List<FacePoint> _facePoints = null;

        //This is the array data that will be passed through the shader.
        //Each buffer may have repeated values, as there must be a value for each remapped face point.
        public List<VertexBuffer> _buffers = null;
        
        private Culling _culling = Culling.Back;

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
        public int[] GenerateBuffers()
        {
            List<int> bindingIds = new List<int>();
            foreach (VertexBuffer b in _buffers)
                bindingIds.Add(b != null ? b.Generate() : 0);
            return bindingIds.ToArray();
        }
        public int[] GetFaceIndices()
        {
            return _faces != null ? _faces.SelectMany(x => new int[] { x.Point0, x.Point1, x.Point2 }).ToArray() : null;
        }
        public VertexTriangle GetFace(int index)
        {
            IndexTriangle t = _faces[index];
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
            _influences = influences;
            HashSet<string> utilized = new HashSet<string>();
            foreach (Influence inf in _influences)
                if (inf != null)
                    for (int i = 0; i < inf.WeightCount; ++i)
                        utilized.Add(inf.Weights[i].Bone);
            _utilizedBones = utilized.ToArray();
        }

        private void CreateFacePoints(int pointCount)
        {
            _facePoints = new List<FacePoint>();
            for (int i = 0; i < pointCount; ++i)
                _facePoints.Add(new FacePoint(i));
        }
        public VertexBuffer FindBuffer(string name)
        {
            if (_buffers != null)
                foreach (VertexBuffer b in _buffers)
                    if (b.Name == name)
                        return b;
            return null;
        }
        public void AddBuffer<T>(
            List<T> bufferData,
            VertexAttribInfo info,
            bool remap = false,
            BufferTarget target = BufferTarget.ArrayBuffer) where T : IBufferable
        {
            if (_buffers == null)
                _buffers = new List<VertexBuffer>();

            int bufferIndex = _buffers.Count;
            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target);
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].Indices.Add(remapper.RemapTable[i]);
            }
            else
            {
                buffer.SetData(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].Indices.Add(i);
            }
            _buffers.Add(buffer);
        }
        public void ReplaceBuffer<T>(
            List<T> bufferData,
            int bufferIndex,
            VertexAttribInfo info,
            bool remap = false,
            BufferTarget target = BufferTarget.ArrayBuffer) where T : IBufferable
        {
            if (_buffers == null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            VertexBuffer buffer = new VertexBuffer(bufferIndex, info, target);
            if (remap)
            {
                Remapper remapper = buffer.SetData(bufferData, true);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].Indices[bufferIndex] = remapper.ImplementationTable[remapper.RemapTable[i]];
            }
            else
            {
                buffer.SetData(bufferData);
                for (int i = 0; i < bufferData.Count; ++i)
                    _facePoints[i].Indices[bufferIndex] = i;
            }
            _buffers[bufferIndex] = buffer;
        }

        private Remapper SetFaceIndices(List<Vertex> vertices, bool remap = true)
        {
            if (vertices.Count % 3 != 0)
                throw new Exception("Vertex list needs to be a multiple of 3.");

            _faces = new List<IndexTriangle>();
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(vertices, null);
                for (int i = 0; i < remapper.RemapTable.Length;)
                {
                    _faces.Add(new IndexTriangle(
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++],
                        remapper.RemapTable[i++]));
                }
                return remapper;
            }
            else
            {
                for (int i = 0; i < vertices.Count; )
                    _faces.Add(new IndexTriangle(i++, i++, i++));
                return null;
            }
        }

        public static PrimitiveData FromQuads(Culling culling, PrimitiveBufferInfo info, params VertexQuad[] quads)
        {
            return FromQuadList(culling, info, quads);
        }
        public static PrimitiveData FromTriangles(Culling culling, PrimitiveBufferInfo info, params VertexTriangle[] triangles)
        {
            return FromTriangleList(culling, info, triangles);
        }
        public static PrimitiveData FromQuadList(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexQuad> quads)
        {
            return FromTriangleList(culling, info, quads.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData FromTriangleList(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexTriangle> triangles)
        {
            return new PrimitiveData(culling, info, triangles);
        }
        
        public PrimitiveData(Culling culling, PrimitiveBufferInfo info, IEnumerable<VertexTriangle> triangles)
        {
            _culling = culling;
            _triangles = new MonitoredList<VertexTriangle>(triangles);
            _triangles.Added += _triangles_Added;
            _triangles.Removed += _triangles_Removed;

            List<Vertex> vertices = triangles.SelectMany(x => x.Vertices).ToList();
            Influence[] influences = vertices.Select(y => y._influence).ToArray();

            Remapper remapper = SetFaceIndices(vertices);
            CreateFacePoints(remapper.ImplementationLength);
            SetInfluences(remapper.ImplementationTable.Select(x => influences[x]).ToArray());

            for (int i = 0; i < info._positionCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._position).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Position, i));
            }
            for (int i = 0; i < info._normalCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._normal).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Normal, i));
            }
            for (int i = 0; i < info._binormalCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._binormal).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Binormal, i));
            }
            for (int i = 0; i < info._tangentCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._tangent).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Tangent, i));
            }
            for (int i = 0; i < info._texcoordCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._texCoord).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.TexCoord, i));
            }
            for (int i = 0; i < info._colorCount; ++i)
            {
                var data = remapper.ImplementationTable.Select(x => vertices[x]._color).ToList();
                AddBuffer(data, new VertexAttribInfo(BufferType.Color, i));
            }
        }
        private void _triangles_Removed(VertexTriangle item)
        {
            throw new NotImplementedException();
        }
        private void _triangles_Added(VertexTriangle item)
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool _isDisposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _faces = null;
                    _facePoints = null;
                }

                if (_buffers != null)
                    foreach (VertexBuffer b in _buffers)
                        b.Dispose();
                _buffers = null;

                _isDisposed = true;
            }
        }
        ~PrimitiveData() { Dispose(false); }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
