using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class TriangleStrip : Polygon
    {
        public override FaceType Type { get { return FaceType.TriangleStrip; } }

        public TriangleStrip() { }
        public TriangleStrip(params Point[] points)
        {
            if (points.Length < 3)
                throw new Exception("A triangle strip needs 3 or more points.");
            _points = points.ToList();
        }

        public override List<Triangle> ToTriangles()
        {
            List<Triangle> triangles = new List<Triangle>();
            for (int i = 2; i < _points.Count; ++i)
            {
                bool cw = (i & 1) == 0;
                triangles.Add(new Triangle(
                    _points[i - 2], 
                    _points[cw ? i : i - 1], 
                    _points[cw ? i - 1 : i]));
            }
            return triangles;
        }
    }
}
