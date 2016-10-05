using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class Ray
    {
        public Ray() { }
        public Ray(Vec3 startPoint, Vec3 endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }
        
        public Vec3 StartPoint { get { return _startPoint; }set { _startPoint = value; } }
        public Vec3 EndPoint { get { return _endPoint; } set { _endPoint = value; } }
        public Vec3 Direction
        {
            get { return _endPoint - _startPoint; }
            set { _endPoint = _startPoint + value; }
        }

        private Vec3 _startPoint;
        private Vec3 _endPoint;

        public bool LineSphereIntersect(Vec3 center, float radius, out Vec3 result)
        {
            Vec3 diff = Direction;
            float a = diff.LengthSquared;
            if (a > 0.0f)
            {
                float b = 2.0f * diff.Dot(StartPoint - center);
                float c = center.LengthSquared + StartPoint.LengthSquared - 
                    2.0f * center.Dot(StartPoint) - 
                    radius * radius;

                float val1, val2;
                if (CustomMath.Quadratic(a, b, c, out val1, out val2))
                {
                    if (val2 < val1)
                        val1 = val2;

                    result = StartPoint + (diff * val1);
                    return true;
                }
            }

            result = new Vec3();
            return false;
        }
        public bool LinePlaneIntersect(Vec3 planePoint, Vec3 planeNormal, out Vec3 result)
        {
            Vec3 diff = Direction;
            float scale = -planeNormal.Dot(StartPoint - planePoint) / planeNormal.Dot(diff);

            if (float.IsNaN(scale) || scale < 0.0f || scale > 1.0f)
            {
                result = new Vec3();
                return false;
            }

            result = StartPoint + (diff * scale);
            return true;
        }
        public Vec3 PointAtLineDistance(float distance)
        {
            Vec3 diff = Direction;
            return StartPoint + (diff * (distance / diff.LengthFast));
        }
        public static Vec3 PointAtLineDistance(Vec3 startPoint, Vec3 endPoint, float distance)
        {
            Vec3 diff = endPoint - startPoint;
            return startPoint + (diff * (distance / diff.LengthFast));
        }
        public Vec3 PointLineIntersect(Vec3 point)
        {
            Vec3 diff = Direction;
            return StartPoint + (diff * (diff.Dot(point - StartPoint) / diff.LengthSquared));
        }
    }
}
