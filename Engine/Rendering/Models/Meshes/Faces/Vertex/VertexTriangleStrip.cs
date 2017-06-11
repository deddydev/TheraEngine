using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models
{
    public class VertexTriangleStrip : VertexPolygon
    {
        public VertexTriangleStrip(params Vertex[] vertices) : base(vertices)
        {

        }

        public int FaceCount { get { return _vertices.Count - 2; } }
        public override FaceType Type { get { return FaceType.TriangleStrip; } }

        public override List<VertexTriangle> ToTriangles()
        {
            List<VertexTriangle> triangles = new List<VertexTriangle>();
            for (int i = 2, count = _vertices.Count, bit = 0; i < count; bit = ++i & 1)
                triangles.Add(new VertexTriangle(
                    _vertices[i - 2],
                    _vertices[i - 1 + bit],
                    _vertices[i - bit]));
            return triangles;
        }
    }
}
