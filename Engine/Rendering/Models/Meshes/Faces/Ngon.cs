using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class Ngon : Polygon
    {
        public Ngon() { }
        public Ngon(params Point[] points)
        {

        }

        public override FaceType Type { get { return FaceType.Ngon; } }

        public override List<IndexTriangle> ToTriangles()
        {
            throw new Exception("");
        }
    }
}
