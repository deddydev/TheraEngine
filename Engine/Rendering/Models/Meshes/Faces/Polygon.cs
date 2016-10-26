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

        public abstract FaceType Type { get; }
        public ReadOnlyCollection<Point> Points { get { return _points.AsReadOnly(); } }
        public abstract List<IndexTriangle> ToTriangles();
    }
}
