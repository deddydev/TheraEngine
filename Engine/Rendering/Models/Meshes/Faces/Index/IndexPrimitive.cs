using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public enum FaceType
    {
        Points,
        Lines,
        LineStrip,
        Triangles,
        TriangleStrip,
        Quads,
        QuadStrip,
        Ngon
    }
    public abstract class IndexPrimitive : ObjectBase
    {
        protected List<Point> _points = new List<Point>();

        public IndexPrimitive(params Point[] points)
        {
            _points = points.ToList();
        }

        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Point> Points { get { return _points.AsReadOnly(); } }
    }
    public abstract class IndexPolygon : IndexPrimitive
    {
        public abstract List<IndexTriangle> ToTriangles();
        public bool ContainsEdge(IndexLine edge, out bool polygonIsCCW)
        {
            for (int i = 0; i < _points.Count; ++i)
            {
                if (_points[i] == edge.Point0)
                {
                    if (i + 1 < _points.Count && _points[i + 1] == edge.Point1)
                    {
                        polygonIsCCW = true;
                        return true;
                    }
                    else if (i - 1 >= 0 && _points[i - 1] == edge.Point1)
                    {
                        polygonIsCCW = false;
                        return true;
                    }
                }
            }
            polygonIsCCW = true;
            return false;
        }
    }
}
