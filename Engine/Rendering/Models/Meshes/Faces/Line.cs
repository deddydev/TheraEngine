using System;

namespace CustomEngine.Rendering.Models
{
    public class Line : ObjectBase
    {
        public Line() { }
        public Line(Point point1, Point point2)
        {
            _point0 = point1;
            _point1 = point2;

            _point0.LinkTo(_point1);
        }

        Point _point0, _point1;

        public Point Point0 { get { return _point0; } }
        public Point Point1 { get { return _point1; } }
    }
}
