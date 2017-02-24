using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace System
{
    public class Circle : Plane
    {
        public Circle() : base() { _radius = 1.0f; }
        public Circle(float radius, Vec3 normal, float distance) 
            : base(normal, distance)  { _radius = radius; }
        public Circle(float radius, Vec3 point)
            : base(point) { _radius = radius; }
        public Circle(float radius, Vec3 point, Vec3 normal) 
            : base(point, normal) { _radius = radius; }
        public Circle(float radius, Vec3 point0, Vec3 point1, Vec3 point2)
            : base(point0, point1, point2) { _radius = radius; }

        private float _radius;
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public static PrimitiveData SolidMesh(float radius, int pointCount)
        {
            if (pointCount < 3)
                throw new Exception("A (very low res) circle needs at least 3 points.");
            
            VertexTriangleFan fan = new VertexTriangleFan();
            return PrimitiveData.FromTriangleFans(Culling.None, new PrimitiveBufferInfo(), fan);
        }
        public static PrimitiveData WireframeMesh(float radius, Vec3 normal, Vec3 center, int pointCount)
        {
            if (pointCount < 3)
                throw new Exception("A (very low res) circle needs at least 3 points.");

            normal.Normalize();
            Quaternion offset = Quaternion.BetweenVectors(Vec3.Up, normal);
            Vertex[] points = new Vertex[pointCount - 1];
            float angleInc = CustomMath.PIf * 2.0f / pointCount;
            for (int i = 0; i < pointCount - 1; ++i)
            {
                float angle = angleInc * i;
                points[i] = new Vertex(center + offset * (radius * new Vec3((float)Math.Cos(angle), 0.0f, -(float)Math.Sin(angle))));
            }
            VertexLineStrip strip = new VertexLineStrip(true, points);
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo(), strip);
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public new struct Header
        {
            public float _radius;
            public Plane.Header _plane;

            public static implicit operator Header(Circle c)
            {
                return new Header()
                {
                    _radius = c._radius,
                    _plane = c,
                };
            }
            public static implicit operator Circle(Header h)
            {
                return new Circle(h._radius, h._plane._normal, h._plane._distance);
            }
        }
    }
}
