using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    public class Vertex
    {
        public Vertex(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            GetData(facepoint, buffers);
        }

        public int _index = -1;
        public Influence _influence;
        public Vec3 _position, _normal, _tangent, _binormal, _barycentric;
        public Vec2 _texCoord;
        public ColorF4 _color;
        public List<VertexLine> _connectedEdges = new List<VertexLine>();

        public Vertex() { }
        public Vertex(Influence inf)
            { _influence = inf; }
        public Vertex(Vec3 position)
            { _position = position; }
        public Vertex(Vec3 position, Influence inf) 
            : this(position) { _influence = inf; }
        public Vertex(Vec3 position, Influence inf, Vec3 normal) 
            : this(position, inf) { _normal = normal; }
        public Vertex(Vec3 position, Influence inf, Vec3 normal, Vec2 texCoord) 
            : this(position, inf, normal) { _texCoord = texCoord; }
        public Vertex(Vec3 position, Influence inf, Vec3 normal, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord) { _color = color; }
        public Vertex(Vec3 position, Influence inf, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }

        public Vertex(Vec3 position, Vec3 normal) 
            : this(position, null, normal) { }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord)
            : this(position, null, normal) { _texCoord = texCoord; }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord) { _color = color; }
        public Vertex(Vec3 position, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }

        public void SetData(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            if (facepoint.Indices == null) return;
            for (int i = 0; i < facepoint.Indices.Count; ++i)
            {
                VertexBuffer b = buffers[i];
                int index = facepoint.Indices[i];
                BufferType type = b.Target;
                switch (type)
                {
                    case BufferType.Position:
                        b.Set(index * 12, _position);
                        break;
                    case BufferType.Normal:
                        b.Set(index * 12, _normal);
                        break;
                    case BufferType.Binormal:
                        b.Set(index * 12, _binormal);
                        break;
                    case BufferType.Tangent:
                        b.Set(index * 12, _tangent);
                        break;
                    case BufferType.Color:
                        b.Set(index << 4, _color);
                        break;
                    case BufferType.TexCoord:
                        b.Set(index << 3, _texCoord);
                        break;
                    case BufferType.Barycentric:
                        b.Set(index * 12, _barycentric);
                        break;
                }
            }
        }
        public void GetData(FacePoint facepoint, List<VertexBuffer> buffers)
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
                    case BufferType.Barycentric:
                        _barycentric = b.Get<Vec3>(index * 12);
                        break;
                }
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
            if (!_normal.Equals(other._normal, precision))
                return false;
            if (!_binormal.Equals(other._binormal, precision))
                return false;
            if (!_tangent.Equals(other._tangent, precision))
                return false;
            if (!_color.Equals(other._color, precision))
                return false;
            if (!_texCoord.Equals(other._texCoord, precision))
                return false;
            return true;
        }

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
                if (_connectedEdges[i].Vertex0 == otherPoint ||
                    _connectedEdges[i].Vertex1 == otherPoint)
                {
                    _connectedEdges[i].Unlink();
                    return;
                }
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
