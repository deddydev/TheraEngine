using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class IndexTriangle : Polygon
    {
        public override FaceType Type { get { return FaceType.Triangles; } }

        public Point Point0
        {
            get { return _points[0]; }
        }
        public Point Point1
        {
            get { return _points[1]; }
        }
        public Point Point2
        {
            get { return _points[2]; }
        }

        public IndexTriangle() { }
        /// <summary>
        /// Counter-Clockwise winding
        ///     2
        ///    / \
        ///   /   \
        ///  0-----1
        /// </summary>
        public IndexTriangle(Point point0, Point point1, Point point2)
        {
            _points.Add(point0);
            _points.Add(point1);
            _points.Add(point2);

            Line e01 = point0.LinkTo(point1);
            Line e12 = point1.LinkTo(point2);
            Line e20 = point2.LinkTo(point0);

            e01.AddFace(this);
            e12.AddFace(this);
            e20.AddFace(this);
        }

        public override List<IndexTriangle> ToTriangles()
        {
            return new List<IndexTriangle>() { this };
        }
    }
}
