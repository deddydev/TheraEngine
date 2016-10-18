using System;

namespace CustomEngine.Rendering.Models
{
    public class Vertex : ObjectBase
    {
        int _index;
        int _matrix;
        int _position;
        int _normal;
        int[] _texCoords;
        int[] _colors;
        
        public override string ToString()
        {
            string vtx = String.Format("VTX{2}: M={0}, V={1}", _matrix, _position, _index);
            if (_normal >= 0)
                vtx += ", N=" + _normal;
            for (int i = 0; i < _texCoords.Length; ++i)
                if (_texCoords[i] >= 0)
                    vtx += String.Format(", T{0}={1}", i, _texCoords[i]);
            for (int i = 0; i < _colors.Length; ++i)
                if (_colors[i] >= 0)
                    vtx += String.Format(", C{0}={1}", i, _colors[i]);
            return vtx;
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
