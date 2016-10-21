using System;

namespace CustomEngine.Rendering.Models
{
    public class Line : ObjectBase
    {
        public Line() { }
        public Line(Point point1, Point point2)
        {
            _point1 = point1;
            _point2 = point2;

            _point1.LinkTo(_point2);
        }

        Point _point1, _point2;
    }
}
