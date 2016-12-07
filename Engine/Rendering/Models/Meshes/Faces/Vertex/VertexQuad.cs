using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexQuad : VertexPolygon
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

        public static VertexQuad MakeQuad(
            Vec3 bottomLeft, Vec3 bottomRight, Vec3 topRight, Vec3 topLeft, Vec3 normal)
        {
            return new VertexQuad(
                new Vertex(0, bottomLeft, null, normal, new Vec2(0.0f, 0.0f)),
                new Vertex(1, bottomRight, null, normal, new Vec2(1.0f, 0.0f)),
                new Vertex(2, topRight, null, normal, new Vec2(1.0f, 1.0f)),
                new Vertex(3, topLeft, null, normal, new Vec2(0.0f, 1.0f)));
        }
        public static VertexQuad MakeQuad(
            Vec3 bottomLeft, Influence bottomLeftInf,
            Vec3 bottomRight, Influence bottomRightInf,
            Vec3 topRight, Influence topRightInf,
            Vec3 topLeft, Influence topLeftInf,
            Vec3 normal)
        {
            return new VertexQuad(
                new Vertex(0, bottomLeft, bottomLeftInf, normal, new Vec2(0.0f, 0.0f)),
                new Vertex(1, bottomRight, bottomRightInf, normal, new Vec2(1.0f, 0.0f)),
                new Vertex(2, topRight, topRightInf, normal, new Vec2(1.0f, 1.0f)),
                new Vertex(3, topLeft, topLeftInf, normal, new Vec2(0.0f, 1.0f)));
        }
        public static VertexQuad MakeQuad(
           Vec3 bottomLeft, Influence bottomLeftInf, Vec3 bottomLeftNormal,
           Vec3 bottomRight, Influence bottomRightInf, Vec3 bottomRightNormal,
           Vec3 topRight, Influence topRightInf, Vec3 topRightNormal,
           Vec3 topLeft, Influence topLeftInf, Vec3 topLeftNormal)
        {
            return new VertexQuad(
                new Vertex(0, bottomLeft, bottomLeftInf, bottomLeftNormal, new Vec2(0.0f, 0.0f)),
                new Vertex(1, bottomRight, bottomRightInf, bottomRightNormal, new Vec2(1.0f, 0.0f)),
                new Vertex(2, topRight, topRightInf, topRightNormal, new Vec2(1.0f, 1.0f)),
                new Vertex(3, topLeft, topLeftInf, topLeftNormal, new Vec2(0.0f, 1.0f)));
        }
    }
}
