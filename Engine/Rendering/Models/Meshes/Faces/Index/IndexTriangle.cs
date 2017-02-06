using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class IndexTriangle : IndexPolygon
    {
        public override FaceType Type { get { return FaceType.Triangles; } }

        public IndexPoint Point0 { get { return _points[0]; } }
        public IndexPoint Point1 { get { return _points[1]; } }
        public IndexPoint Point2 { get { return _points[2]; } }

        public IndexTriangle() { }
        /// <summary>
        /// Counter-Clockwise winding
        ///     2
        ///    / \
        ///   /   \
        ///  0-----1
        /// </summary>
        public IndexTriangle(IndexPoint point0, IndexPoint point1, IndexPoint point2)
        {
            _points.Add(point0);
            _points.Add(point1);
            _points.Add(point2);

            IndexLine e01 = point0.LinkTo(point1);
            IndexLine e12 = point1.LinkTo(point2);
            IndexLine e20 = point2.LinkTo(point0);

            e01.AddFace(this);
            e12.AddFace(this);
            e20.AddFace(this);
        }

        public override List<IndexTriangle> ToTriangles()
        {
            return new List<IndexTriangle>() { this };
        }

        public override bool Equals(object obj)
        {
            IndexTriangle t = obj as IndexTriangle;
            return t == null ? false : t.Point0 == Point0 && t.Point1 == Point1 && t.Point2 == Point2;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
