﻿using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public class VertexTriangleFan : VertexPolygon
    {
        public VertexTriangleFan(params Vertex[] vertices) : base(vertices) { }
        public VertexTriangleFan(IEnumerable<Vertex> vertices) : base(vertices) { }
        public override FaceType Type => FaceType.TriangleFan;
        public override VertexTriangle[] ToTriangles()
        {
            int triangleCount = _vertices.Count - 2;
            VertexTriangle[] list = new VertexTriangle[triangleCount];
            for (int i = 0; i < triangleCount; ++i)
                list[i] = new VertexTriangle(_vertices[0], _vertices[i + 2], _vertices[i + 1]);
            return list;
        }
    }
}
