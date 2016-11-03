using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models
{
    public class IndexLineStrip : ObjectBase
    {
        public IndexLineStrip() { }
        public IndexLineStrip(bool closed, params Point[] points)
        {
            _closed = closed;
            _points = points.ToList();
        }

        bool _closed;
        List<Point> _points;
    }
}
