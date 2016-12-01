using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class IndexTriangleFan : IndexPrimitive
    {
        public IndexTriangleFan(params Point[] points) : base(points)
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
