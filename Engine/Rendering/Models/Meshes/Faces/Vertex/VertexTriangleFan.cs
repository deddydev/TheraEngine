using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexTriangleFan : VertexPolygon
    {
        public VertexTriangleFan(params Vertex[] vertices) : base(vertices)
        {

        }

        public override FaceType Type
        {
            get
            {
                return FaceType.TriangleFan;
            }
        }

        public override List<VertexTriangle> ToTriangles()
        {
            throw new NotImplementedException();
        }
    }
}
