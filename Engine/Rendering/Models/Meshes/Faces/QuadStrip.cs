using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class QuadStrip : Polygon
    {
        public override FaceType Type { get { return FaceType.QuadStrip; } }

        public QuadStrip() { }
        public QuadStrip(params Point[] points)
        {

        }

        public override List<Triangle> ToTriangles()
        {
            throw new NotImplementedException();
        }
    }
}
