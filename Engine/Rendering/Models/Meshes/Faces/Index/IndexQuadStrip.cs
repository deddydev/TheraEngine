using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public class IndexQuadStrip : IndexPolygon
    {
        public override FaceType Type { get { return FaceType.QuadStrip; } }

        public IndexQuadStrip() { }
        public IndexQuadStrip(params IndexPoint[] points)
        {

        }

        public override List<IndexTriangle> ToTriangles()
        {
            throw new NotImplementedException();
        }
    }
}
