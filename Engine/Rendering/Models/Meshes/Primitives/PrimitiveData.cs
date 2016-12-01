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
    public class PrimitiveData : IDisposable
    {
        //Faces have indices that refer to face points.
        //These may contain repeat vertex indices but each triangle is unique.
        public List<IndexTriangle> _faces = null;

        //Influence per raw vertex.
        //Count is same as _facePoints.Count
        public Influence[] _influences;
        public string[] _utilizedBones;

        public bool HasSkinning { get { return _utilizedBones.Length > 0; } }

        //Face points have indices that refer to each buffer.
        //These may contain repeat buffer indicies but each point is unique.
        public List<FacePoint> _facePoints = null;

        //This is the array data that will be passed through the shader.
        //Each buffer only has unique values.
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
        public int[] Initialize()
        {
            List<int> bindingIds = new List<int>();
            foreach (VertexBuffer b in _buffers)
                bindingIds.Add(b != null ? b.Initialize() : 0);
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
        public static PrimitiveData FromQuads(Culling culling, params VertexQuad[] quads)
        {
            return FromQuadList(culling, quads);
        }
        public static PrimitiveData FromTriangles(Culling culling, params VertexTriangle[] triangles)
        {
            return FromTriangleList(culling, triangles);
        }
        public static PrimitiveData FromQuadList(Culling culling, IEnumerable<VertexQuad> quads)
        {
            return FromTriangleList(culling, quads.SelectMany(x => x.ToTriangles()).ToList());
        }
        public static PrimitiveData FromTriangleList(Culling culling, IEnumerable<VertexTriangle> triangles)
        {
            bool hasNormals = false;
            int texCoordCount = 0, colorCount = 0;

            List<Vertex> vertices = triangles.SelectMany(x => x.Vertices).ToList();
            if (vertices.Any(x => x._normal != null))
                hasNormals = true;
            foreach (Vertex v in vertices)
            {
                if (v._texCoords != null && v._texCoords.Count > texCoordCount)
                    texCoordCount = v._texCoords.Count;
                if (v._colors != null && v._colors.Count > colorCount)
                    colorCount = v._colors.Count;
            }
            
            PrimitiveData data = new PrimitiveData();
            data._culling = culling;
            data.SetInfluences(triangles.SelectMany(x => x.Vertices.Select(y => y._influence)).ToArray());

            Remapper remapper = data.SetFaceIndices(vertices);
            data.CreateFacePoints(remapper.ImplementationLength);

            List<Vec3> positions = remapper.ImplementationTable.Select(x => vertices[x]._position).ToList();
            data.AddBuffer(positions, new VertexAttribInfo(BufferType.Position));

            if (hasNormals)
            {
                List<Vec3> normals = remapper.ImplementationTable.Select(x => vertices[x]._normal.GetValueOrDefault()).ToList();
                data.AddBuffer(normals, VertexBuffer.GetBufferName(BufferType.Normal));
            }
            for (int i = 0; i < colorCount; ++i)
            {
                List<ColorF4> colors = remapper.ImplementationTable.Select(x => i < vertices[x]._colors.Count ? vertices[x]._colors[i] : default(ColorF4)).ToList();
                data.AddBuffer(colors, VertexBuffer.GetBufferName(BufferType.Color, i));
            }
            for (int i = 0; i < texCoordCount; ++i)
            {
                List<Vec2> texCoords = remapper.ImplementationTable.Select(x => i < vertices[x]._texCoords.Count ? vertices[x]._texCoords[i] : default(Vec2)).ToList();
                data.AddBuffer(texCoords, VertexBuffer.GetBufferName(BufferType.TexCoord, i));
            }
            
            return data;
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
        private void AddBuffer<T>(
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
        private void ReplaceBuffer<T>(
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
