using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class Quad : Polygon
    {
        public override FaceType Type { get { return FaceType.Quads; } }

        bool _forwardSlash = false;

        public Quad() { }
        /// <summary>
        /// Counter-Clockwise winding
        ///2--3        3       2--3      2          2--3
        ///|  |  =    /|  and  | /   or  |\    and   \ |
        ///|  |      / |       |/        | \          \|
        ///0--1     0--1       0         0--1          1
        /// </summary>
        public Quad(Point point1, Point point2, Point point3, Point point4, bool forwardSlash = false)
        {
            _points.Add(point1);
            _points.Add(point2);
            _points.Add(point3);
            _points.Add(point4);

            point1.LinkTo(point2);
            point2.LinkTo(point4);
            point4.LinkTo(point3);
            point3.LinkTo(point1);
            if (_forwardSlash = forwardSlash)
                point1.LinkTo(point4);
            else
                point2.LinkTo(point3);
        }
        public override List<Triangle> ToTriangles()
        {
            List<Triangle> triangles = new List<Triangle>();
            if (_forwardSlash)
            {
                triangles.Add(new Triangle(_points[0], _points[1], _points[3]));
                triangles.Add(new Triangle(_points[0], _points[3], _points[2]));
            }
            else
            {
                triangles.Add(new Triangle(_points[0], _points[1], _points[2]));
                triangles.Add(new Triangle(_points[2], _points[1], _points[3]));
            }
            return triangles;
        }
    }
}
