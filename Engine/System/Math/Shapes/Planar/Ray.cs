using System.ComponentModel;

namespace System
{
    public class Ray
    {
        public Ray() { }
        public Ray(Vec3 startPoint, Vec3 direction)
        {
            _startPoint = startPoint;
            _direction = direction.NormalizedFast();
        }
        public Vec3 StartPoint
        {
            get => _startPoint;
            set => _startPoint = value;
        }
        public Vec3 Direction
        {
            get => _direction;
            set => _direction = value;
        }

        [DefaultValue("0 0 0")]
        [Serialize("StartPoint")]
        private Vec3 _startPoint;
        [DefaultValue("0 1 0")]
        [Serialize("Direction")]
        private Vec3 _direction;

        public Ray TransformedBy(Matrix4 transform)
        {
            Vec3 newStart = Vec3.TransformPosition(StartPoint, transform);
            Vec3 newEnd = Vec3.TransformPosition(StartPoint + Direction, transform);
            return new Ray(newStart, newEnd - newStart);
        }

        public float DistanceToPointFast(Vec3 point)
            => (Direction ^ (point - StartPoint)).LengthFast;
        public float DistanceToPoint(Vec3 point)
            => (Direction ^ (point - StartPoint)).Length;

        //public float DistanceToRay(Ray ray)
        //{
        //    throw new NotImplementedException();
        //}
        //public float DistanceToSegment(Segment segment)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Returns a vector that starts at the given point and perpendicularly intersects with a ray formed by the given start and end points.
        /// </summary>
        public static Vec3 GetPerpendicularVectorFromPoint(Vec3 startPoint, Vec3 direction, Vec3 point)
        {
            direction.NormalizeFast();
            return (startPoint + ((point - startPoint).Dot(direction)) * direction) - point;
        }
        public Vec3 PerpendicularVectorFromPoint(Vec3 point)
            => GetPerpendicularVectorFromPoint(StartPoint, Direction, point);
        public Segment PerpendicularSegmentFromPoint(Vec3 point)
            => new Segment(point, point + PerpendicularVectorFromPoint(point));
        
        public bool LineSphereIntersect(Vec3 center, float radius, out Vec3 result)
        {
            Vec3 diff = Direction;
            float a = diff.LengthSquared;
            if (a > 0.0f)
            {
                float b = 2.0f * diff.Dot(StartPoint - center);
                float c = center.LengthSquared + StartPoint.LengthSquared - 2.0f * center.Dot(StartPoint) - radius * radius;

                if (CustomMath.Quadratic(a, b, c, out float val1, out float val2))
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
        public bool LinePlaneIntersect(Plane p, out Vec3 result)
            => LinePlaneIntersect(p.IntersectionPoint, p.Normal, out result);

        public bool LinePlaneIntersect(Vec3 point, Vec3 normal, out Vec3 result)
        {
            Vec3 diff = Direction;
            float scale = -normal.Dot(StartPoint - point) / normal.Dot(diff);

            if (float.IsNaN(scale) || scale < 0.0f || scale > 1.0f)
            {
                result = new Vec3();
                return false;
            }
            result = StartPoint + (diff * scale);
            return true;
        }
        public Vec3 PointAtNormalizedLineDistance(float distance)
            => StartPoint + Direction * distance;
        public Vec3 PointAtLineDistance(float distance)
        {
            Vec3 diff = Direction;
            return StartPoint + (diff * (distance / diff.LengthFast));
        }
        public Vec3 PointLineIntersect(Vec3 point)
        {
            Vec3 diff = Direction;
            return StartPoint + (diff * (diff.Dot(point - StartPoint) / diff.LengthSquared));
        }
        public static Vec3 PointAtLineDistance(Vec3 start, Vec3 end, float distance)
        {
            Vec3 diff = end - start;
            return start + (diff * (distance / diff.LengthFast));
        }
    }
}
