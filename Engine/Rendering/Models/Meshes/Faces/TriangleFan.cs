using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class TriangleFan : ObjectBase
    {
        public TriangleFan(FacePoint midPoint, params FacePoint[] points)
        {
            _points.Add(midPoint);
        }

        public FacePoint _midPoint;
        public List<FacePoint> _points = new List<FacePoint>();
    }
}
