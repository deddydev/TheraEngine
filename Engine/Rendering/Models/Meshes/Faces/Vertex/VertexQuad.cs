using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.Models
{
    public class VertexQuad : VertexPolygon
    {
        public Vertex Vertex0 => _vertices[0];
        public Vertex Vertex1 => _vertices[1];
        public Vertex Vertex2 => _vertices[2];
        public Vertex Vertex3 => _vertices[3];
        
        public override FaceType Type => FaceType.Quads;

        /// <summary>
        /// 3--2
        /// |\ |
        /// | \|
        /// 0--1
        /// </summary>
        public VertexQuad(Vertex v0, Vertex v1, Vertex v2, Vertex v3) 
            : base(v0, v1, v2, v3) { }

        public override List<VertexTriangle> ToTriangles()
        {
            return new List<VertexTriangle>()
            {
                new VertexTriangle(Vertex0, Vertex1, Vertex3),
                new VertexTriangle(Vertex3, Vertex1, Vertex2),
            };
        }
        public static VertexQuad MakeQuad(
            Vec3 bottomLeft, Vec3 bottomRight, Vec3 topRight, Vec3 topLeft, bool addAutoNormal = false)
        {
            if (addAutoNormal)
            {
                Vec3 normal = Vec3.CalculateNormal(bottomLeft, bottomRight, topLeft);
                return new VertexQuad(
                    new Vertex(bottomLeft,  normal, new Vec2(0.0f, 0.0f)),
                    new Vertex(bottomRight, normal, new Vec2(1.0f, 0.0f)),
                    new Vertex(topRight,    normal, new Vec2(1.0f, 1.0f)),
                    new Vertex(topLeft,     normal, new Vec2(0.0f, 1.0f)));
            }
            else
                return new VertexQuad(
                    new Vertex(bottomLeft,  new Vec2(0.0f, 0.0f)),
                    new Vertex(bottomRight, new Vec2(1.0f, 0.0f)),
                    new Vertex(topRight,    new Vec2(1.0f, 1.0f)),
                    new Vertex(topLeft,     new Vec2(0.0f, 1.0f)));
        }
        public static VertexQuad MakeQuad(
            Vec3 bottomLeft, Vec3 bottomRight, Vec3 topRight, Vec3 topLeft, Vec3 normal)
        {
            return new VertexQuad(
                new Vertex(bottomLeft,  normal, new Vec2(0.0f, 0.0f)),
                new Vertex(bottomRight, normal, new Vec2(1.0f, 0.0f)),
                new Vertex(topRight,    normal, new Vec2(1.0f, 1.0f)),
                new Vertex(topLeft,     normal, new Vec2(0.0f, 1.0f)));
        }
        /// <summary>
        /// Makes a quad using positions, influences, and a common normal.
        /// </summary>
        public static VertexQuad MakeQuad(
            Vec3 bottomLeft,    Influence bottomLeftInf,
            Vec3 bottomRight,   Influence bottomRightInf,
            Vec3 topRight,      Influence topRightInf,
            Vec3 topLeft,       Influence topLeftInf,
            Vec3 normal)
        {
            return new VertexQuad(
                new Vertex(bottomLeft,  bottomLeftInf,  normal, new Vec2(0.0f, 0.0f)),
                new Vertex(bottomRight, bottomRightInf, normal, new Vec2(1.0f, 0.0f)),
                new Vertex(topRight,    topRightInf,    normal, new Vec2(1.0f, 1.0f)),
                new Vertex(topLeft,     topLeftInf,     normal, new Vec2(0.0f, 1.0f)));
        }
        public static VertexQuad MakeQuad(
           Vec3 bottomLeft,     Influence bottomLeftInf,    Vec3 bottomLeftNormal,
           Vec3 bottomRight,    Influence bottomRightInf,   Vec3 bottomRightNormal,
           Vec3 topRight,       Influence topRightInf,      Vec3 topRightNormal,
           Vec3 topLeft,        Influence topLeftInf,       Vec3 topLeftNormal)
        {
            return new VertexQuad(
                new Vertex(bottomLeft,  bottomLeftInf,  bottomLeftNormal,   new Vec2(0.0f, 0.0f)),
                new Vertex(bottomRight, bottomRightInf, bottomRightNormal,  new Vec2(1.0f, 0.0f)),
                new Vertex(topRight,    topRightInf,    topRightNormal,     new Vec2(1.0f, 1.0f)),
                new Vertex(topLeft,     topLeftInf,     topLeftNormal,      new Vec2(0.0f, 1.0f)));
        }
        /// <summary>
        /// Y-up is facing the sky, like a floor.
        /// </summary>
        public static VertexQuad YUpQuad(float scale = 1.0f) => YUpQuad(scale, scale);
        /// <summary>
        /// Y-up is facing the sky, like a floor.
        /// </summary>
        public static VertexQuad YUpQuad(float xScale, float zScale)
        {
            float xHalf = xScale / 2.0f;
            float zHalf = zScale / 2.0f;
            Vec3 v1 = new Vec3(-xHalf, 0.0f, zHalf);
            Vec3 v2 = new Vec3(xHalf, 0.0f, zHalf);
            Vec3 v3 = new Vec3(xHalf, 0.0f, -zHalf);
            Vec3 v4 = new Vec3(-xHalf, 0.0f, -zHalf);
            return MakeQuad(v1, v2, v3, v4, Vec3.UnitY);
        }
        public static VertexQuad YUpQuad(BoundingRectangle region)
        {
            return MakeQuad(
                ((Vec3)region.BottomLeft).Xzy,
                ((Vec3)region.BottomRight).Xzy,
                ((Vec3)region.TopRight).Xzy,
                ((Vec3)region.TopLeft).Xzy,
                Vec3.UnitZ);
        }

        /// <summary>
        /// Z-up is facing the camera, like a wall.
        /// </summary>
        public static VertexQuad ZUpQuad(float scale = 1.0f) => ZUpQuad(scale, scale);
        /// <summary>
        /// Z-up is facing the camera, like a wall.
        /// </summary>
        public static VertexQuad ZUpQuad(float xScale, float yScale)
        {
            float xHalf = xScale / 2.0f;
            float yHalf = yScale / 2.0f;
            Vec3 v1 = new Vec3(-xHalf, -yHalf, 0.0f);
            Vec3 v2 = new Vec3(xHalf, -yHalf, 0.0f);
            Vec3 v3 = new Vec3(xHalf, yHalf, 0.0f);
            Vec3 v4 = new Vec3(-xHalf, yHalf, 0.0f);
            return MakeQuad(v1, v2, v3, v4, Vec3.UnitZ);
        }
        public static VertexQuad ZUpQuad(BoundingRectangle region)
        {
            return MakeQuad(
                region.BottomLeft,
                region.BottomRight,
                region.TopRight,
                region.TopLeft,
                Vec3.UnitZ);
        }

        public static VertexQuad MakeQuad(
           Vec3 bottomLeft,     Influence bottomLeftInf,
           Vec3 bottomRight,    Influence bottomRightInf,
           Vec3 topRight,       Influence topRightInf,
           Vec3 topLeft,        Influence topLeftInf,
           bool addAutoNormal = false)
        {
            if (addAutoNormal)
            {
                Vec3 normal = Vec3.CalculateNormal(bottomLeft, bottomRight, topLeft);
                return new VertexQuad(
                    new Vertex(bottomLeft,  bottomLeftInf,  normal, new Vec2(0.0f, 0.0f)),
                    new Vertex(bottomRight, bottomRightInf, normal, new Vec2(1.0f, 0.0f)),
                    new Vertex(topRight,    topRightInf,    normal, new Vec2(1.0f, 1.0f)),
                    new Vertex(topLeft,     topLeftInf,     normal, new Vec2(0.0f, 1.0f)));
            }
            else
                return new VertexQuad(
                    new Vertex(bottomLeft,  bottomLeftInf,  new Vec2(0.0f, 0.0f)),
                    new Vertex(bottomRight, bottomRightInf, new Vec2(1.0f, 0.0f)),
                    new Vertex(topRight,    topRightInf,    new Vec2(1.0f, 1.0f)),
                    new Vertex(topLeft,     topLeftInf,     new Vec2(0.0f, 1.0f)));
        }
    }
}
