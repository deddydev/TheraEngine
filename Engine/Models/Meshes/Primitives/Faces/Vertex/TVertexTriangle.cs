namespace TheraEngine.Rendering.Models
{
    public class TVertexTriangle : TVertexPolygon
    {
        public TVertex Vertex0 => _vertices[0];
        public TVertex Vertex1 => _vertices[1];
        public TVertex Vertex2 => _vertices[2];

        //private VertexLine _e01, _e12, _e20;

        public override FaceType Type => FaceType.Triangles;

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public TVertexTriangle(TVertex v0, TVertex v1, TVertex v2) : base(v0, v1, v2)
        {
            //_e01 = v0.LinkTo(v1);
            //_e12 = v1.LinkTo(v2);
            //_e20 = v2.LinkTo(v0);

            //_e01.AddFace(this);
            //_e12.AddFace(this);
            //_e20.AddFace(this);
        }

        public override TVertexTriangle[] ToTriangles()
            => new TVertexTriangle[] { this };
        public override TVertexLine[] ToLines() 
            => new TVertexLine[] 
            {
                new TVertexLine(Vertex0, Vertex1),
                new TVertexLine(Vertex1, Vertex2),
                new TVertexLine(Vertex2, Vertex0),
            };
    }
}
