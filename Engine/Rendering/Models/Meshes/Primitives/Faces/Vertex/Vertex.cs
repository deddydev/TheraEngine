using System;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Models
{
    public class Vertex
    {
        public Vertex(FacePoint facepoint, List<DataBuffer> buffers)
            => GetData(facepoint, buffers);
        
        public int Index { get; set; } = -1;
        public InfluenceDef Influence { get; set; }
        public Vec3 Position { get; set; }
        public Vec3 Normal { get; set; }
        public Vec3 Tangent { get; set; }
        public Vec3 Binormal { get; set; }
        public Vec2 TexCoord { get; set; }
        public ColorF4 Color { get; set; }
        
        public Vertex() { }
        public Vertex(InfluenceDef inf)
            { Influence = inf; }
        public Vertex(Vec3 position)
            { Position = position; }
        public Vertex(Vec3 position, ColorF4 color)
            { Position = position; Color = color; }
        public Vertex(Vec3 position, InfluenceDef inf) 
            : this(position) { Influence = inf; }

        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal) 
            : this(position, inf) { Normal = normal; }

        public Vertex HardCopy()
        {
            return new Vertex()
            {
                Index = Index,
                Influence = Influence,
                Position = Position,
                Normal = Normal,
                Tangent = Tangent,
                Binormal = Binormal,
                TexCoord = TexCoord,
                Color = Color,
            };
        }

        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal, Vec2 texCoord) 
            : this(position, inf, normal) { TexCoord = texCoord; }
        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord) { Color = color; }
        public Vertex(Vec3 position, InfluenceDef inf, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color) 
            : this(position, inf, normal, texCoord, color) { Binormal = binormal; Tangent = tangent; }

        public Vertex(Vec3 position, InfluenceDef inf, Vec2 texCoord)
            : this(position, inf) { TexCoord = texCoord; }
        public Vertex(Vec3 position, Vec2 texCoord)
            : this(position) { TexCoord = texCoord; }

        public Vertex(Vec3 position, Vec3 normal) 
            : this(position, null, normal) { }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord)
            : this(position, null, normal) { TexCoord = texCoord; }
        public Vertex(Vec3 position, Vec3 normal, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord) { Color = color; }
        public Vertex(Vec3 position, Vec3 normal, Vec3 binormal, Vec3 tangent, Vec2 texCoord, ColorF4 color)
            : this(position, null, normal, texCoord, color) { Binormal = binormal; Tangent = tangent; }

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
                        b.Set(index * 12, Position);
                        break;
                    case EBufferType.Normal:
                        b.Set(index * 12, Normal);
                        break;
                    case EBufferType.Binormal:
                        b.Set(index * 12, Binormal);
                        break;
                    case EBufferType.Tangent:
                        b.Set(index * 12, Tangent);
                        break;
                    case EBufferType.Color:
                        b.Set(index << 4, Color);
                        break;
                    case EBufferType.TexCoord:
                        b.Set(index << 3, TexCoord);
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
                        Position = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Normal:
                        Normal = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Binormal:
                        Binormal = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Tangent:
                        Tangent = b.Get<Vec3>(index * 12);
                        break;
                    case EBufferType.Color:
                        TexCoord = b.Get<Vec2>(index << 4);
                        break;
                    case EBufferType.TexCoord:
                        TexCoord = b.Get<Vec2>(index << 3);
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
            if (Influence != other.Influence)
                return false;
            if (!Position.Equals(other.Position, precision))
                return false;
            if (!Normal.Equals(other.Normal, precision))
                return false;
            if (!Binormal.Equals(other.Binormal, precision))
                return false;
            if (!Tangent.Equals(other.Tangent, precision))
                return false;
            if (!Color.Equals(other.Color, precision))
                return false;
            if (!TexCoord.Equals(other.TexCoord, precision))
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
