using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class TriangleFan : ObjectBase
    {
        public TriangleFan(Vertex midPoint, params Vertex[] points)
        {
            _points.Add(midPoint);
        }

        public Vertex _midPoint;
        public List<Vertex> _points = new List<Vertex>();
    }
}
