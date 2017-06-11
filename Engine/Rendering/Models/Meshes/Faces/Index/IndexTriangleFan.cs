using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public class IndexTriangleFan : IndexPolygon
    {
        public IndexTriangleFan(params IndexPoint[] points) : base(points)
        {

        }

        public override FaceType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override List<IndexTriangle> ToTriangles()
        {
            throw new NotImplementedException();
        }
    }
}
