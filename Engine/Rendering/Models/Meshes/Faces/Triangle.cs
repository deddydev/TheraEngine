using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Triangle : Polygon
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

        public Triangle() { }
        /// <summary>
        /// Counter-Clockwise winding
        ///     2
        ///    / \
        ///   /   \
        ///  0-----1
        /// </summary>
        public Triangle(Point point1, Point point2, Point point3)
        {
            _points.Add(point1);
            _points.Add(point2);
            _points.Add(point3);

            point1.LinkTo(point2);
            point2.LinkTo(point3);
            point3.LinkTo(point1);
        }

        public override List<Triangle> ToTriangles()
        {
            return new List<Triangle>() { this };
        }
    }
}
