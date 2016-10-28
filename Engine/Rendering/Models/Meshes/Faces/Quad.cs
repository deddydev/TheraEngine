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
        public Quad(Point point0, Point point1, Point point2, Point point3, bool forwardSlash = false)
        {
            _points.Add(point0);
            _points.Add(point1);
            _points.Add(point2);
            _points.Add(point3);

            Line e01 = point0.LinkTo(point1);
            Line e13 = point1.LinkTo(point3);
            Line e32 = point3.LinkTo(point2);
            Line e20 = point2.LinkTo(point0);

            e01.AddFace(this);
            e13.AddFace(this);
            e32.AddFace(this);
            e20.AddFace(this);

            _forwardSlash = forwardSlash;

            //if (_forwardSlash)
            //{
            //    Line e03 = point0.LinkTo(point3);
            //}
            //else
            //{
            //    Line e12 = point1.LinkTo(point2);
            //}
        }
        public override List<IndexTriangle> ToTriangles()
        {
            List<IndexTriangle> triangles = new List<IndexTriangle>();
            if (_forwardSlash)
            {
                triangles.Add(new IndexTriangle(_points[0], _points[1], _points[3]));
                triangles.Add(new IndexTriangle(_points[0], _points[3], _points[2]));
            }
            else
            {
                triangles.Add(new IndexTriangle(_points[0], _points[1], _points[2]));
                triangles.Add(new IndexTriangle(_points[2], _points[1], _points[3]));
            }
            return triangles;
        }
    }
}
