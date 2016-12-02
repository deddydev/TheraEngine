using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexLine : VertexPrimitive
    {
        public RawVertex Vertex0 { get { return _vertices[0]; } }
        public RawVertex Vertex1 { get { return _vertices[1]; } }

        public override FaceType Type { get { return FaceType.Triangles; } }

        /// <summary>
        ///    2
        ///   / \
        ///  /   \
        /// 0-----1
        /// </summary>
        public VertexLine(RawVertex v0, RawVertex v1) : base(v0, v1)
        {
            Vertex0.AddLine(this);
            Vertex1.AddLine(this);
        }

        internal void AddFace(VertexTriangle vertexTriangle)
        {
            throw new NotImplementedException();
        }
    }
}
