using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Vertex : ObjectBase
    {
        public Vec3 _position;
        public Vec3? _normal;
        public List<Vec2> _texCoords;
        public List<ColorF4> _colors;
        public List<List<IBufferable>> _customData;

        public Vertex(Vec3 position) { _position = position; }
        public Vertex(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            if (facepoint.Indices != null)
            {
                foreach (int i in facepoint.Indices)
                {

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

    public class VertexTriangle
    {
        public Vertex _v0, _v1, _v2;

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public VertexTriangle(Vertex v0, Vertex v1, Vertex v2)
        {
            _v0 = v0;
            _v1 = v1;
            _v2 = v2;
        }

        public List<Vertex> GetVertexList(bool swapCulling)
        {
            if (!swapCulling)
                return new List<Vertex>() { _v0, _v1, _v2 };
            else //Return opposite order
                return new List<Vertex>() { _v0, _v2, _v1 };
        }
    }
    public enum OutsideDirection
    {
        Toward,
        Away
    }
    public class VertexQuad
    {
        public Vertex _v0, _v1, _v2, _v3;
        public OutsideDirection _outside;
        public bool _forwardSlash = true;

        /// <summary>
        /// 2--3
        /// |\ |
        /// | \|
        /// 0--1
        /// </summary>
        public VertexQuad(Vertex v0, Vertex v1, Vertex v2, Vertex v3, OutsideDirection outside)
        {
            _v0 = v0;
            _v1 = v1;
            _v2 = v2;
            _v3 = v3;
            _outside = outside;
        }

        public List<VertexTriangle> GetTriangleList()
        {
            if (_outside == OutsideDirection.Toward)
                return new List<VertexTriangle>()
                {
                    new VertexTriangle(_v0, _v1, _v2),
                    new VertexTriangle(_v2, _v1, _v3)
                };
            else
                return new List<VertexTriangle>()
                {
                    new VertexTriangle(_v0, _v2, _v1),
                    new VertexTriangle(_v1, _v2, _v3)
                };
        }
    }
}
