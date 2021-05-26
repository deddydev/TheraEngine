namespace TheraEngine.Rendering.Models
{
    public class TVertexLine : TVertexPrimitive
    {
        public TVertex Vertex0 => _vertices[0];
        public TVertex Vertex1 => _vertices[1];

        public override FaceType Type => FaceType.Triangles;

        //private List<VertexTriangle> _triangles = new List<VertexTriangle>();

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public TVertexLine(TVertex v0, TVertex v1) : base(v0, v1)
        {
            //Vertex0.AddLine(this);
            //Vertex1.AddLine(this);
        }

        internal void AddFace(TVertexTriangle face)
        {
            //_triangles.Add(face);
        }

        //internal void Unlink()
        //{
        //    Vertex0.RemoveLine(this);
        //    Vertex1.RemoveLine(this);
        //}
    }
}
