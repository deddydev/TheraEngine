using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexQuad : VertexPrimitive
    {
        public Vertex Vertex0 { get { return _vertices[0]; } }
        public Vertex Vertex1 { get { return _vertices[1]; } }
        public Vertex Vertex2 { get { return _vertices[2]; } }
        public Vertex Vertex3 { get { return _vertices[3]; } }

        public bool _forwardSlash = true;

        public override FaceType Type { get { return FaceType.Quads; } }

        /// <summary>
        /// 3--2
        /// |\/|
        /// |/\|
        /// 0--1
        /// </summary>
        public VertexQuad(Vertex v0, Vertex v1, Vertex v2, Vertex v3) : base(v0, v1, v2, v3) { }

        public override List<VertexTriangle> ToTriangles()
        {
            return new List<VertexTriangle>()
            {
                new VertexTriangle(Vertex0, Vertex1, Vertex3),
                new VertexTriangle(Vertex3, Vertex1, Vertex2),
            };
        }

        public static VertexQuad MakeQuad(Vec3 bottomLeft, Vec3 bottomRight, Vec3 topRight, Vec3 topLeft, Vec3 normal)
        {
            return new VertexQuad(
                new Vertex(bottomLeft) { _normal = normal, _texCoords = new List<Vec2>() { new Vec2(0.0f, 0.0f) } },
                new Vertex(bottomRight) { _normal = normal, _texCoords = new List<Vec2>() { new Vec2(1.0f, 0.0f) } },
                new Vertex(topRight) { _normal = normal, _texCoords = new List<Vec2>() { new Vec2(1.0f, 1.0f) } },
                new Vertex(topLeft) { _normal = normal, _texCoords = new List<Vec2>() { new Vec2(0.0f, 1.0f) } });
        }
    }
}
