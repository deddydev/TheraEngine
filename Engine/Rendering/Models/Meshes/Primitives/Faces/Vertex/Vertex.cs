using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public class Vertex
    {
        public Vertex(FacePoint facepoint, List<DataBuffer> buffers)
        {
            GetData(facepoint, buffers);
        }

        public int _index = -1;
        public InfluenceDef _influence;
        public Vec3 _position, _normal, _tangent, _binormal;
        public Vec2 _texCoord;
        public ColorF4 _color;
        //public List<VertexLine> _connectedEdges = new List<VertexLine>();

        public Vertex() { }
        public Vertex(InfluenceDef inf)
            { _influence = inf; }
        public Vertex(Vec3 position)
            { _position = position; }
        public Vertex(Vec3 position, InfluenceDef inf) 
            : this(position) { _influence = inf; }

        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal) 
            : this(position, inf) { _normal = normal; }

        public Vertex HardCopy()
        {
            return new Vertex()
            {
                _index = _index,
                _influence = _influence,
                _position = _position,
                _normal = _normal,
                _tangent = _tangent,
                _binormal = _binormal,
                _texCoord = _texCoord,
                _color = _color,
                //_connectedEdges = new List<VertexLine>(_connectedEdges),
            };
        }

        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal, Vec2 texCoord) 
            : this(position, inf, normal) { _texCoord = texCoord; }
        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord) { _color = color; }
        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }

        public Vertex(Vec3 position, InfluenceDef inf, Vec2 texCoord)
            : this(position, inf) { _texCoord = texCoord; }
        public Vertex(Vec3 position, Vec2 texCoord)
            : this(position) { _texCoord = texCoord; }

        public Vertex(Vec3 position, Vec3 normal) 
            : this(position, null, normal) { }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord)
            : this(position, null, normal) { _texCoord = texCoord; }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord) { _color = color; }
        public Vertex(Vec3 position, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }

        public void SetData(FacePoint facepoint, List<DataBuffer> buffers)
        {
            if (facepoint.BufferIndices == null) return;
            for (int i = 0; i < facepoint.BufferIndices.Count; ++i)
            {
                DataBuffer b = buffers[i];
                int index = facepoint.BufferIndices[i];
                EBufferType type = b.BufferType;
                switch (type)
                {
                    case EBufferType.Position:
                        b.Set(index * 12, _position);
                        break;
                    case EBufferType.Normal:
                        b.Set(index * 12, _normal);
                        break;
                    case EBufferType.Binormal:
                        b.Set(index * 12, _binormal);
                        break;
                    case EBufferType.Tangent:
                        b.Set(index * 12, _tangent);
                        break;
                    case EBufferType.Color:
                        b.Set(index << 4, _color);
                        break;
                    case EBufferType.TexCoord:
                        b.Set(index << 3, _texCoord);
                        break;
                }
            }
        }
        public void GetData(FacePoint facepoint, List<DataBuffer> buffers)
        {
            if (facepoint.BufferIndices == null)
                return;

            for (int i = 0; i < facepoint.BufferIndices.Count; ++i)
            {
                DataBuffer b = buffers[i];
                int index = facepoint.BufferIndices[i];
                EBufferType type = b.BufferType;
                switch (type)
                {
                    case EBufferType.Position:
                        _position = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Normal:
                        _normal = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Binormal:
                        _binormal = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Tangent:
                        _tangent = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Color:
                        _texCoord = b.Get<Vec2>(index << 4);
                        break;
                    case EBufferType.TexCoord:
                        _texCoord = b.Get<Vec2>(index << 3);
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

        //internal void AddLine(VertexLine edge)
        //{
        //    if (!_connectedEdges.Contains(edge))
        //        _connectedEdges.Add(edge);
        //}
        //internal void RemoveLine(VertexLine edge)
        //{
        //    if (_connectedEdges.Contains(edge))
        //        _connectedEdges.Remove(edge);
        //}
        //public VertexLine LinkTo(Vertex otherPoint)
        //{
        //    foreach (VertexLine edge in _connectedEdges)
        //        if (edge.Vertex0 == otherPoint ||
        //            edge.Vertex0 == otherPoint)
        //            return edge;

        //    //Creating a new line automatically links the points.
        //    return new VertexLine(this, otherPoint);
        //}
        //public void UnlinkFrom(Vertex otherPoint)
        //{
        //    for (int i = 0; i < _connectedEdges.Count; ++i)
        //        if (_connectedEdges[i].Vertex0 == otherPoint ||
        //            _connectedEdges[i].Vertex1 == otherPoint)
        //        {
        //            _connectedEdges[i].Unlink();
        //            return;
        //        }
        //}

        public static implicit operator Vertex(Vec3 pos) => new Vertex(pos);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
