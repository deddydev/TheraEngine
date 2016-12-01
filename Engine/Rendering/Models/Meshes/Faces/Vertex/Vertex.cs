using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    //Contains final transformed information
    //using transform feedback from the gpu
    public class FinalVertex : ObjectBase
    {
        public Vec3 _position, _normal, _tangent, _binormal;
        public Vec2 _texCoord;
        public ColorF4 _color;
    }
    public class MorphableVertex
    {
        public MorphableVertex(Vertex baseVertex, params Vertex[] morphVertices)
        {
            _baseVertex = baseVertex;
            _morphVertices = morphVertices;
        }
        public Vertex _baseVertex;
        public Vertex[] _morphVertices = new Vertex[VertexBuffer.MaxBufferCountPerType];
    }
    public class Vertex
    {
        public int _index;
        public Influence _influence;
        public Vec3 _position, _normal, _tangent, _binormal;
        public Vec2 _texCoord;
        public ColorF4 _color;

        public Vertex(Vec3 position) { _position = position; }
        public Vertex(Vec3 position, Vec3 normal) : this(position) { _normal = normal; }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord) : this(position, normal) { _texCoord = texCoord; }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord, ColorF4 color) : this(position, normal, texCoord) { _color = color; }
        public Vertex(Vec3 position, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color) : this(position, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }
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
                        _position = b.Get<Vec3>(index * 12);
                        break;
                    case BufferType.Normal:
                        _normal = b.Get<Vec3>(index * 12);
                        break;
                    case BufferType.Binormal:
                        _binormal = b.Get<Vec3>(index * 12);
                        break;
                    case BufferType.Tangent:
                        _tangent = b.Get<Vec3>(index * 12);
                        break;
                    case BufferType.Color:
                        _texCoord = b.Get<Vec2>(index << 4);
                        break;
                    case BufferType.TexCoord:
                        _texCoord = b.Get<Vec2>(index << 3);
                        break;
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
