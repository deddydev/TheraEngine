using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Triangle : Polygon
    {
        public Triangle() { }
        /// <summary>
        /// Clockwise winding
        ///     2
        ///    /|
        ///   / |
        ///  0--1
        /// </summary>
        public Triangle(Point point1, Point point2, Point point3)
        {
            _point1 = point1;
            _point2 = point2;
            _point3 = point3;
        }

        public Point _point1, _point2, _point3;

        public override FaceType Type { get { return FaceType.Triangles; } }

        public override List<Triangle> ToTriangles()
        {
            return new List<Triangle>() { this };
        }
    }
}
