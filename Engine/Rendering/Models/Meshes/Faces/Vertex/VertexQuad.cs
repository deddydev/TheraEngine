﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class VertexQuad : VertexPolygon
    {
        public RawVertex Vertex0 { get { return _vertices[0]; } }
        public RawVertex Vertex1 { get { return _vertices[1]; } }
        public RawVertex Vertex2 { get { return _vertices[2]; } }
        public RawVertex Vertex3 { get { return _vertices[3]; } }

        public bool _forwardSlash = true;

        public override FaceType Type { get { return FaceType.Quads; } }

        /// <summary>
        /// 3--2
        /// |\/|
        /// |/\|
        /// 0--1
        /// </summary>
        public VertexQuad(RawVertex v0, RawVertex v1, RawVertex v2, RawVertex v3) : base(v0, v1, v2, v3) { }

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
                new RawVertex(bottomLeft, null, normal, new Vec2(0.0f, 0.0f)),
                new RawVertex(bottomRight, null, normal, new Vec2(1.0f, 0.0f)),
                new RawVertex(topRight, null, normal, new Vec2(1.0f, 1.0f)),
                new RawVertex(topLeft, null, normal, new Vec2(0.0f, 1.0f)));
        }
    }
}
