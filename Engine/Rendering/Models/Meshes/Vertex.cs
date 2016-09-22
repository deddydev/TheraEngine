using CustomEngine.Rendering.Textures;
using OpenTK;

namespace CustomEngine.Rendering.Models.Meshes
{
    public class Vertex
    {
        private Face _owner;
        public Face Owner { get { return _owner; } set { _owner = value; } }

        Vector3 _position;
        Vector3 _normal;
        Vector2[] _uvs;
        RGBAPixel[] _colors;
    }
}
