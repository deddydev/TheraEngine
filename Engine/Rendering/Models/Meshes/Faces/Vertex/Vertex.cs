using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CustomEngine.Rendering.Models
{
    public class FinalTriangle
    {
        Vertex _v0, _v1, _v2;

        public Box GetCullingVolume()
        {
            Vec3 min = Vec3.Max, max = Vec3.Min;
            max = Vec3.ComponentMax(max, _v0._position);
            min = Vec3.ComponentMin(min, _v0._position);
            max = Vec3.ComponentMax(max, _v1._position);
            min = Vec3.ComponentMin(min, _v1._position);
            max = Vec3.ComponentMax(max, _v2._position);
            min = Vec3.ComponentMin(min, _v2._position);
            return new Box(
                CustomMath.ComponentMin(_v0._position, _v1._position, _v2._position),
                CustomMath.ComponentMax(_v0._position, _v1._position, _v2._position));
        }
    }
    public class Vertex
    {
        public int Index { get { return _index; } set { _index = value; } }
        public RawVertex BaseVertex { get { return _baseVertex; } set { _baseVertex = value; } }
        
        public int _index;
        public RawVertex _baseVertex;

        public Vertex(int index, RawVertex baseVertex)
        {
            _index = index;
            _baseVertex = baseVertex;
        }
        public Vertex(RawVertex baseVertex)
        {
            _index = -1;
            _baseVertex = baseVertex;
        }

        public static implicit operator Vertex(RawVertex v) { return new Vertex(v); }
    }
    public class VertexMorphWeight
    {
        public float _weight;
        public RawVertex _vertex;

        public VertexMorphWeight(RawVertex vertex, float weight)
        {
            _vertex = vertex;
            _weight = weight.Clamp(0.0f, 1.0f);
        }
    }
    public class MorphableVertex : Vertex
    {
        public MorphableVertex(RawVertex baseVertex, params VertexMorphWeight[] morphVertices) : base(baseVertex)
        {
            if (morphVertices.Length > VertexBuffer.MaxBufferCountPerType - 1)
                morphVertices.Resize(VertexBuffer.MaxBufferCountPerType - 1);
            _morphVertices = morphVertices;
        }

        public VertexMorphWeight[] _morphVertices;

        //Contains final transformed information
        //using transform feedback from the gpu
        public RawVertex _finalVertex;
    }
    public class RawVertex
    {
        public Influence _influence;
        public Vec3? _position, _normal, _tangent, _binormal;
        public Vec2? _texCoord;
        public ColorF4? _color;

        public RawVertex(Vec3 position)
            { _position = position; }
        public RawVertex(Vec3 position, Influence inf) 
            : this(position) { _influence = inf; }
        public RawVertex(Vec3 position, Influence inf, Vec3 normal) 
            : this(position, inf) { _normal = normal; }
        public RawVertex(Vec3 position, Influence inf, Vec3 normal, Vec2 texCoord) 
            : this(position, inf, normal) { _texCoord = texCoord; }
        public RawVertex(Vec3 position, Influence inf, Vec3 normal, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord) { _color = color; }
        public RawVertex(Vec3 position, Influence inf, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }

        public RawVertex(Vec3 position, Vec3 normal) 
            : this(position, null, normal) { }
        public RawVertex(Vec3 position, Vec3 normal, Vec2 texCoord)
            : this(position, null, normal) { _texCoord = texCoord; }
        public RawVertex(Vec3 position, Vec3 normal, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord) { _color = color; }
        public RawVertex(Vec3 position, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord, color) { _binormal = binormal; _tangent = tangent; }

        public void SetData(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            if (facepoint.Indices == null) return;
            for (int i = 0; i < facepoint.Indices.Count; ++i)
            {
                VertexBuffer b = buffers[i];
                int index = facepoint.Indices[i];
                VertexAttribInfo info = b.Info;
                switch (info._type)
                {
                    case BufferType.Position:
                        b.Set(index * 12, _position.HasValue ? _position.Value : default(Vec3));
                        break;
                    case BufferType.Normal:
                        b.Set(index * 12, _normal.HasValue ? _normal.Value : default(Vec3));
                        break;
                    case BufferType.Binormal:
                        b.Set(index * 12, _binormal.HasValue ? _binormal.Value : default(Vec3));
                        break;
                    case BufferType.Tangent:
                        b.Set(index * 12, _tangent.HasValue ? _tangent.Value : default(Vec3));
                        break;
                    case BufferType.Color:
                        b.Set(index << 4, _color.HasValue ? _color.Value : default(ColorF4));
                        break;
                    case BufferType.TexCoord:
                        b.Set(index << 3, _texCoord.HasValue ? _texCoord.Value : default(Vec2));
                        break;
                }
            }
        }
        public void GetData(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            if (facepoint.Indices == null) return;
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
        public RawVertex(FacePoint facepoint, List<VertexBuffer> buffers) { GetData(facepoint, buffers); }

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
        public VertexLine LinkTo(RawVertex otherPoint)
        {
            foreach (VertexLine edge in _connectedEdges)
                if (edge.Vertex0 == otherPoint ||
                    edge.Vertex0 == otherPoint)
                    return edge;

            //Creating a new line automatically links the points.
            return new VertexLine(this, otherPoint);
        }
        public void UnlinkFrom(RawVertex otherPoint)
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
            return obj is RawVertex ? Equals(obj as RawVertex) : false;
        }
        public bool Equals(RawVertex other)
        {
            const float precision = 0.00001f;
            if (other == null)
                return false;
            if (_influence != other._influence)
                return false;
            if (_position.HasValue != other._position.HasValue || !_position.Value.Equals(other._position.Value, precision))
                return false;
            if (_normal.HasValue != other._normal.HasValue || !_normal.Value.Equals(other._normal.Value, precision))
                return false;
            if (_binormal.HasValue != other._binormal.HasValue || !_binormal.Value.Equals(other._binormal.Value, precision))
                return false;
            if (_tangent.HasValue != other._tangent.HasValue || !_tangent.Value.Equals(other._tangent.Value, precision))
                return false;
            if (_color.HasValue != other._color.HasValue || !_color.Value.Equals(other._color.Value, precision))
                return false;
            if (_texCoord.HasValue != other._texCoord.HasValue || !_texCoord.Value.Equals(other._texCoord.Value, precision))
                return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
