using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Vertex : ObjectBase
    {
        public int _index;
        public Vec3 _position;
        public Vec3? _normal;
        public List<Vec2> _texCoords;
        public List<ColorF4> _colors;
        public List<IBufferable> _specialData;

        public Vertex(Vec3 position) { _position = position; }
        public Vertex(FacePoint facepoint, List<VertexBuffer> buffers)
        {
            _normal = null;
            _texCoords = new List<Vec2>();
            _colors = new List<ColorF4>();
            if (facepoint.Indices == null)
                return;
            for (int i = 0; i < facepoint.Indices.Count; ++i)
            {
                VertexBuffer b = buffers[i];
                int index = facepoint.Indices[i];
                if (b.IsPositionsBuffer())
                {
                    _position = b.Get<Vec3>(index * 12);
                }
                else if (b.IsNormalsBuffer())
                {
                    _normal = b.Get<Vec3>(index * 12);
                }
                else if (b.IsColorBuffer())
                {
                    _colors.Add(b.Get<ColorF4>(index * 16));
                }
                else if (b.IsTexCoordBuffer())
                {
                    _texCoords.Add(b.Get<Vec2>(index * 8));
                }
                else
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
}
