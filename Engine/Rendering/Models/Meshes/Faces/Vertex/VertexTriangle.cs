using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexTriangle : VertexPolygon
    {
        public RawVertex Vertex0 { get { return _vertices[0]; } }
        public RawVertex Vertex1 { get { return _vertices[1]; } }
        public RawVertex Vertex2 { get { return _vertices[2]; } }

        public override FaceType Type { get { return FaceType.Triangles; } }

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public VertexTriangle(RawVertex v0, RawVertex v1, RawVertex v2) : base(v0, v1, v2)
        {
            VertexLine e01 = v0.LinkTo(v1);
            VertexLine e12 = v1.LinkTo(v2);
            VertexLine e20 = v2.LinkTo(v0);
            
            e01.AddFace(this);
            e12.AddFace(this);
            e20.AddFace(this);
        }

        public override List<VertexTriangle> ToTriangles()
        {
            return new List<VertexTriangle>() { this };
        }
    }
}
