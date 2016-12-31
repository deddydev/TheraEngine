using CustomEngine.Rendering.Models;
using System.Collections.Generic;

namespace System
{
    public class Circle : Plane
    {
        public Circle() { }

        private float _radius;
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        //public static PrimitiveData Mesh(float radius, int pointCount, bool lines)
        //{
        //    if (pointCount < 3)
        //        throw new Exception("A (very low res) circle needs at least 3 points.");

        //    if (lines)
        //    {
        //        Vertex[] points = new Vertex[pointCount];
        //        for (int i = 0; i < pointCount; ++i)
        //        {

        //        }
        //        VertexLineStrip strip = new VertexLineStrip(true, points);
        //        return PrimitiveData.FromLineList(Culling.None, new PrimitiveBufferInfo(), strip.Vertices);
        //    }
        //    else
        //    {
        //        VertexTriangleFan fan = new VertexTriangleFan();
        //        return PrimitiveData.FromTriangleFans(Culling.None, new PrimitiveBufferInfo(), fan);
        //    }
        //}
    }
}
