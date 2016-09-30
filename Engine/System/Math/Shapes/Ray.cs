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
        public Ray(Vector3 startPoint, Vector3 endPoint)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
        }
        
        public Vector3 StartPoint { get { return _startPoint; }set { _startPoint = value; } }
        public Vector3 EndPoint { get { return _endPoint; } set { _endPoint = value; } }
        public Vector3 Direction
        {
            get { return _endPoint - _startPoint; }
            set { _endPoint = _startPoint + value; }
        }

        private Vector3 _startPoint;
        private Vector3 _endPoint;

        public bool LineSphereIntersect(Vector3 center, float radius, out Vector3 result)
        {
            Vector3 diff = Direction;
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

            result = new Vector3();
            return false;
        }
        public bool LinePlaneIntersect(Vector3 planePoint, Vector3 planeNormal, out Vector3 result)
        {
            Vector3 diff = Direction;
            float scale = -planeNormal.Dot(StartPoint - planePoint) / planeNormal.Dot(diff);

            if (float.IsNaN(scale) || scale < 0.0f || scale > 1.0f)
            {
                result = new Vector3();
                return false;
            }

            result = StartPoint + (diff * scale);
            return true;
        }
        public Vector3 PointAtLineDistance(float distance)
        {
            Vector3 diff = Direction;
            return StartPoint + (diff * (distance / diff.LengthFast));
        }
        public static Vector3 PointAtLineDistance(Vector3 startPoint, Vector3 endPoint, float distance)
        {
            Vector3 diff = endPoint - startPoint;
            return startPoint + (diff * (distance / diff.LengthFast));
        }
        public Vector3 PointLineIntersect(Vector3 point)
        {
            Vector3 diff = Direction;
            return StartPoint + (diff * (diff.Dot(point - StartPoint) / diff.LengthSquared));
        }
    }
}
