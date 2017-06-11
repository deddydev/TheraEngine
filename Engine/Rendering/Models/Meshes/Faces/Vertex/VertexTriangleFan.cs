using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models
{
    public class VertexTriangleFan : VertexPolygon
    {
        public VertexTriangleFan(params Vertex[] vertices) : base(vertices) { }
        public override FaceType Type { get { return FaceType.TriangleFan; } }
        public override List<VertexTriangle> ToTriangles()
        {
            int triangleCount = _vertices.Count - 2;
            List<VertexTriangle> list = new List<VertexTriangle>(triangleCount);
            for (int i = 0; i < triangleCount; ++i)
                list.Add(new VertexTriangle(_vertices[0], _vertices[i + 2], _vertices[i + 1]));
            return list;
        }
    }
}
