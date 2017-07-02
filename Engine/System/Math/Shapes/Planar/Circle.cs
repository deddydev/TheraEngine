using TheraEngine.Rendering.Models;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TheraEngine.Files;
using System.Xml;
using System.IO;
using System.ComponentModel;

namespace System
{
    /// <summary>
    /// Represents a circle in 3D space.
    /// </summary>
    public class Circle3D : Plane
    {
        public Circle3D()
            : base() { _radius = 1.0f; }
        public Circle3D(float radius)
            : base() { _radius = radius; }
        public Circle3D(float radius, Vec3 normal, float distance) 
            : base(normal, distance)  { _radius = radius; }
        public Circle3D(float radius, Vec3 point)
            : base(point) { _radius = radius; }
        public Circle3D(float radius, Vec3 point, Vec3 normal) 
            : base(point, normal) { _radius = radius; }
        public Circle3D(float radius, Vec3 point0, Vec3 point1, Vec3 point2)
            : base(point0, point1, point2) { _radius = radius; }

        [DefaultValue(1.0f)]
        [Serialize("Radius")]
        private float _radius;
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public PrimitiveData GetSolidMesh(int sides)
            => SolidMesh(Radius, Normal, IntersectionPoint, sides);
        public PrimitiveData GetWireframeMesh(int sides)
            => WireframeMesh(Radius, Normal, IntersectionPoint, sides);
        public VertexLineStrip GetLineStrip(int sides)
            => LineStrip(Radius, Normal, IntersectionPoint, sides);
        
        public static PrimitiveData SolidMesh(float radius, Vec3 normal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            normal.Normalize();
            Quat offset = Quat.BetweenVectors(Vec3.Up, normal);
            Vertex[] points = new Vertex[sides + 1];
            points[0] = new Vertex(center, normal);
            float angleInc = CustomMath.PIf * 2.0f / sides, angle = 0.0f;
            for (int i = 1; i < sides; ++i, angle += angleInc)
            {
                Vec3 v = new Vec3((float)Math.Cos(angle), 0.0f, (float)Math.Sin(angle));
                points[i] = new Vertex(center + offset * (radius * v), normal);
            }
            Vec3[] positions = Points(radius, normal, center, sides);
            VertexTriangleFan fan = new VertexTriangleFan();
            return PrimitiveData.FromTriangleFans(Culling.None, new PrimitiveBufferInfo(), fan);
        }
        public static PrimitiveData WireframeMesh(float radius, Vec3 normal, Vec3 center, int sides)
        {
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo(), LineStrip(radius, normal, center, sides));
        }
        public static VertexLineStrip LineStrip(float radius, Vec3 normal, Vec3 center, int sides)
        {
            Vec3[] points = Points(radius, normal, center, sides);
            return new VertexLineStrip(true, points.Select(x => new Vertex(x)).ToArray());
        }
        public static Vec3[] Points(float radius, Vec3 normal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            normal.NormalizeFast();
            Quat offset = Quat.BetweenVectors(Vec3.Up, normal);
            Vec3[] points = new Vec3[sides];
            float angleInc = CustomMath.PIf * 2.0f / sides;
            float angle = 0.0f;
            for (int i = 0; i < sides; ++i, angle += angleInc)
            {
                Vec3 v = new Vec3((float)Math.Cos(angle), 0.0f, (float)Math.Sin(angle));
                points[i] = center + offset * (radius * v);
            }
            return points;
        }
    }
}
