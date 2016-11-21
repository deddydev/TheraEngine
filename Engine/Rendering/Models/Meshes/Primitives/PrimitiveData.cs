using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Rendering.Models
{
    public class PrimitiveData : IDisposable
    {
        //Faces have indices that refer to face points.
        //These may contain repeat vertex indices but each triangle is unique.
        public List<IndexTriangle> _faces = null;

        //Face points have indices that refer to each buffer.
        //These may contain repeat buffer indicies but each point is unique.
        public List<FacePoint> _facePoints = null;

        //This is the array data that will be passed through the shader.
        //Each buffer only has unique values.
        public List<VertexBuffer> _buffers = null;

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
        public static PrimitiveData FromQuads(params VertexQuad[] quads)
        {
            return FromQuadList(quads);
        }
        public static PrimitiveData FromQuadList(IEnumerable<VertexQuad> quads)
        {
            return FromTriangles(quads.SelectMany(x => x.ToTriangles()).ToList());
        }
        public static PrimitiveData FromTriangles(IEnumerable<VertexTriangle> triangles)
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
            Remapper vertexRemap = data.SetFaceIndices(vertices);
            data.CreateFacePoints(vertexRemap.ImplementationLength);
            
            List<Vec3> positions = vertexRemap.ImplementationTable.Select(x => vertices[x]._position).ToList();
            data.AddBuffer(positions, VertexBuffer.PositionsName);

            if (hasNormals)
            {
                List<Vec3> normals = vertexRemap.ImplementationTable.Select(x => vertices[x]._normal.GetValueOrDefault()).ToList();
                data.AddBuffer(normals, VertexBuffer.NormalsName);
            }
            for (int i = 0; i < colorCount; ++i)
            {
                List<ColorF4> colors = vertexRemap.ImplementationTable.Select(x => i < vertices[x]._colors.Count ? vertices[x]._colors[i] : default(ColorF4)).ToList();
                data.AddBuffer(colors, VertexBuffer.ColorName + i.ToString());
            }
            for (int i = 0; i < texCoordCount; ++i)
            {
                List<Vec2> texCoords = vertexRemap.ImplementationTable.Select(x => i < vertices[x]._texCoords.Count ? vertices[x]._texCoords[i] : default(Vec2)).ToList();
                data.AddBuffer(texCoords, VertexBuffer.TexCoordName + i.ToString());
            }
            return data;
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
        private void AddBuffer<T>(List<T> bufferData, string name, BufferTarget target = BufferTarget.ArrayBuffer) where T : IBufferable
        {
            if (_buffers == null)
                _buffers = new List<VertexBuffer>();

            int bufferIndex = _buffers.Count;
            VertexBuffer buffer = new VertexBuffer(bufferIndex, name, target);
            Remapper remapper = buffer.SetData(bufferData);
            for (int i = 0; i < bufferData.Count; ++i)
                _facePoints[i].Indices.Add(remapper.ImplementationTable[remapper.RemapTable[i]]);
            _buffers.Add(buffer);
        }
        private void ReplaceBuffer<T>(List<T> bufferData, int bufferIndex, string name, BufferTarget target = BufferTarget.ArrayBuffer) where T : IBufferable
        {
            if (_buffers == null)
                throw new InvalidOperationException();
            if (bufferIndex < 0 || bufferIndex >= _buffers.Count)
                throw new IndexOutOfRangeException();

            VertexBuffer buffer = new VertexBuffer(bufferIndex, name, target);
            Remapper posRemap = buffer.SetData(bufferData);
            for (int i = 0; i < bufferData.Count; ++i)
                _facePoints[i].Indices[bufferIndex] = posRemap.ImplementationTable[posRemap.RemapTable[i]];
            _buffers[bufferIndex] = buffer;
        }

        private Remapper SetFaceIndices(List<Vertex> vertices)
        {
            if (vertices.Count % 3 != 0)
                throw new Exception("Vertex list needs to be a multiple of 3.");

            _faces = new List<IndexTriangle>();
            Remapper remapper = new Remapper();
            remapper.Remap(vertices, null);

            for (int i = 0; i < remapper.RemapTable.Length;)
            {
                _faces.Add(new IndexTriangle(
                    remapper.ImplementationTable[remapper.RemapTable[i++]],
                    remapper.ImplementationTable[remapper.RemapTable[i++]],
                    remapper.ImplementationTable[remapper.RemapTable[i++]]));
            }

            return remapper;
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
