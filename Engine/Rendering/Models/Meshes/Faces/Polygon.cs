using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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
    public abstract class Polygon : ObjectBase
    {
        protected List<Point> _points = new List<Point>();

        public bool ContainsEdge(Line edge, out bool polygonIsCCW)
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

        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Point> Points { get { return _points.AsReadOnly(); } }
        public abstract List<IndexTriangle> ToTriangles();
    }
}
