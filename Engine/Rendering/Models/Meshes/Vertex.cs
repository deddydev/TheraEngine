using System;

namespace CustomEngine.Rendering.Models.Meshes
{
    public class Vertex
    {
        private Face _owner;
        public Face Owner { get { return _owner; } set { _owner = value; } }

        Vec3 _position;
        Vec3 _normal;
        Vec2[] _uvs;
        ColorF4[] _colors;
    }
}
