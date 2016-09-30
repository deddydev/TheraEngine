using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Plane
    {
        public Plane() { }
        public Plane(Vector3 point, Vector3 normal)
        {
            _normal = normal;
            _distance = -point.Dot(normal);
        }
        public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            float x1 = point2.X - point1.X;
            float y1 = point2.Y - point1.Y;
            float z1 = point2.Z - point1.Z;
            float x2 = point3.X - point1.X;
            float y2 = point3.Y - point1.Y;
            float z2 = point3.Z - point1.Z;
            float yz = (y1 * z2) - (z1 * y2);
            float xz = (z1 * x2) - (x1 * z2);
            float xy = (x1 * y2) - (y1 * x2);
            float invPyth = 1.0f / (float)Math.Sqrt(yz * yz + xz * xz + xy * xy);
            Normal = new Vector3(yz * invPyth, xz * invPyth, xy * invPyth);
            _distance = -((Normal.X * point1.X) + (Normal.Y * point1.Y) + (Normal.Z * point1.Z));
        }

        public Vector3 Point { get { return _normal.Normalized() * -_distance; } set { _distance = -value.Dot(_normal); } }
        public Vector3 Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }
        
        private Vector3 _normal;
        private float _distance;

        public void Normalize()
        {
            float magnitude = 1.0f / Normal.LengthFast;
            _normal *= magnitude;
            _distance *= magnitude;
        }
        public PlaneIntersection IntersectsBox(Box box)
        {
            Vector3 min;
            Vector3 max;

            max.X = (Normal.X >= 0.0f) ? box.Minimum.X : box.Maximum.X;
            max.Y = (Normal.Y >= 0.0f) ? box.Minimum.Y : box.Maximum.Y;
            max.Z = (Normal.Z >= 0.0f) ? box.Minimum.Z : box.Maximum.Z;
            min.X = (Normal.X >= 0.0f) ? box.Maximum.X : box.Minimum.X;
            min.Y = (Normal.Y >= 0.0f) ? box.Maximum.Y : box.Minimum.Y;
            min.Z = (Normal.Z >= 0.0f) ? box.Maximum.Z : box.Minimum.Z;

            if (Normal.Dot(max) + _distance > 0.0f)
                return PlaneIntersection.Front;

            if (Normal.Dot(min) + _distance < 0.0f)
                return PlaneIntersection.Back;

            return PlaneIntersection.Intersecting;
        }

        public PlaneIntersection IntersectsSphere(float radius, Vector3 center)
        {
            float dot = center.Dot(Normal) + _distance;

            if (dot > radius)
                return PlaneIntersection.Front;

            if (dot < -radius)
                return PlaneIntersection.Back;

            return PlaneIntersection.Intersecting;
        }
    }
}
