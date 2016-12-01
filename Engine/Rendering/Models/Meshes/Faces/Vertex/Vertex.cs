using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    public class Vertex : ObjectBase
    {
        public int _index;
        public Influence _influence;
        public List<Vec3>
            _positions = new List<Vec3>(VertexAttribInfo.MaxTypeBufferCount),
            _normals = new List<Vec3>(VertexAttribInfo.MaxTypeBufferCount),
            _binormals = new List<Vec3>(VertexAttribInfo.MaxTypeBufferCount),
            _tangents = new List<Vec3>(VertexAttribInfo.MaxTypeBufferCount);
        public List<Vec2> _texCoords = new List<Vec2>(VertexAttribInfo.MaxTypeBufferCount);
        public List<ColorF4> _colors = new List<ColorF4>(VertexAttribInfo.MaxTypeBufferCount);

        //Set using transform feedback from the gpu
        public Vec3 _transformedPosition;

        public Vertex(Vec3 position) { _positions.Add(position); }
        public Vertex(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            if (facepoint.Indices == null)
                return;
            for (int i = 0; i < facepoint.Indices.Count; ++i)
            {
                VertexBuffer b = buffers[i];
                int index = facepoint.Indices[i];
                VertexAttribInfo info = b.Info;
                switch (info._type)
                {
                    case BufferType.Position:
                        _positions.Add(b.Get<Vec3>(index * 12));
                        break;
                    case BufferType.Normal:
                        _normals.Add(b.Get<Vec3>(index * 12));
                        break;
                    case BufferType.Binormal:
                        _binormals.Add(b.Get<Vec3>(index * 12));
                        break;
                    case BufferType.Tangent:
                        _tangents.Add(b.Get<Vec3>(index * 12));
                        break;
                    case BufferType.Color:
                        _texCoords.Add(b.Get<Vec2>(index << 4));
                        break;
                    case BufferType.TexCoord:
                        _texCoords.Add(b.Get<Vec2>(index << 3));
                        break;
                }
                if (b.IsType(BufferType.Position))
                {
                    _position = ;
                }
                else if (b.IsType(BufferType.Normal))
                {
                    _normal = b.Get<Vec3>(index * 12);
                }
                else if (b.IsType(BufferType.Color))
                {
                    _colors.Add(b.Get<ColorF4>(index * 16));
                }
                else if (b.IsType(BufferType.TexCoord))
                {
                    _texCoords.Add();
                }
                else
                {

                }
            }
        }

        public ReadOnlyCollection<VertexLine> ConnectedEdges { get { return _connectedEdges.AsReadOnly(); } }

        List<VertexLine> _connectedEdges = new List<VertexLine>();

        internal void AddLine(VertexLine edge)
        {
            if (!_connectedEdges.Contains(edge))
                _connectedEdges.Add(edge);
        }

        internal void RemoveLine(VertexLine edge)
        {
            if (_connectedEdges.Contains(edge))
                _connectedEdges.Remove(edge);
        }

        public VertexLine LinkTo(Vertex otherPoint)
        {
            foreach (VertexLine edge in _connectedEdges)
                if (edge.Vertex0 == otherPoint ||
                    edge.Vertex0 == otherPoint)
                    return edge;

            //Creating a new line automatically links the points.
            return new VertexLine(this, otherPoint);
        }
        public void UnlinkFrom(Vertex otherPoint)
        {
            for (int i = 0; i < _connectedEdges.Count; ++i)
                if (_connectedEdges[i].Point0 == otherPoint ||
                    _connectedEdges[i].Point1 == otherPoint)
                {
                    _connectedEdges[i].Unlink();
                    return;
                }
        }
        public override bool Equals(object obj)
        {
            return obj is Vertex ? Equals(obj as Vertex) : false;
        }
        public bool Equals(Vertex other)
        {
            const float precision = 0.00001f;
            if (other == null)
                return false;
            if (_influence != other._influence)
                return false;
            if (!_position.Equals(other._position, precision))
                return false;
            if (_normal.HasValue != other._normal.HasValue || !_normal.Value.Equals(other._normal.Value, precision))
                return false;
            if (_colors.Count != other._colors.Count)
                return false;
            if (_texCoords.Count != other._texCoords.Count)
                return false;
            for (int i = 0; i < _colors.Count; ++i)
                if (!_colors[i].Equals(other._colors[i], precision))
                    return false;
            for (int i = 0; i < _texCoords.Count; ++i)
                if (!_texCoords[i].Equals(other._texCoords[i], precision))
                    return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
