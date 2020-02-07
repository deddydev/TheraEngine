using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheraEngine.ComponentModel;

namespace TheraEngine.Rendering.Models
{
    public enum FaceType
    {
        Points,
        Lines,
        LineStrip,
        LineLoop,
        Triangles,
        TriangleStrip,
        TriangleFan,
        Quads,
        QuadStrip,
        Ngon
    }
    public abstract class IndexPrimitive : TObject
    {
        [TSerialize("Points")]
        protected List<IndexPoint> _points = new List<IndexPoint>();

        public IndexPrimitive(params IndexPoint[] points)
        {
            _points = points.ToList();
        }

        public abstract FaceType Type { get; }
        public ReadOnlyCollection<IndexPoint> Points { get { return _points.AsReadOnly(); } }
    }
    public abstract class IndexPolygon : IndexPrimitive
    {
        public IndexPolygon(params IndexPoint[] points) : base(points) { }
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
