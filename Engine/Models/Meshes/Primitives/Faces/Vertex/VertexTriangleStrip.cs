namespace TheraEngine.Rendering.Models
{
    public class VertexTriangleStrip : TVertexPolygon
    {
        public VertexTriangleStrip(params TVertex[] vertices) : base(vertices) { }

        public int FaceCount => _vertices.Count - 2;
        public override FaceType Type => FaceType.TriangleStrip;

        public override TVertexTriangle[] ToTriangles()
        {
            TVertexTriangle[] triangles = new TVertexTriangle[FaceCount];
            for (int i = 2, count = _vertices.Count, bit = 0; i < count; bit = ++i & 1)
                triangles[i - 2] = new TVertexTriangle(
                    _vertices[i - 2],
                    _vertices[i - 1 + bit],
                    _vertices[i - bit]);
            return triangles;
        }
    }
}
