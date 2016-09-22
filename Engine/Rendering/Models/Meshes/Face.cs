namespace CustomEngine.Rendering.Models.Meshes
{
    public class Face
    {
        private Mesh _owner;
        public Mesh Owner { get { return _owner; } set { _owner = value; } }

        public Vertex _point1;
        public Vertex _point2;
        public Vertex _point3;
    }
}
