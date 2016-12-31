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
            get { return _startPoint; }
            set { _startPoint = value; }
        }
        public Vec3 Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        private Vec3 _startPoint;
        private Vec3 _direction;

        public Ray TransformedBy(Matrix4 transform)
        {
            Vec3 newStart = Vec3.TransformPosition(StartPoint, transform);
            Vec3 newEnd = Vec3.TransformPosition(StartPoint + Direction, transform);
            return new Ray(newStart, newEnd - newStart);
        }
        public float DistanceToPoint(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public float DistanceToLine(Ray line)
        {
            throw new NotImplementedException();
        }
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
        public bool LinePlaneIntersect(Plane p, out Vec3 result)
        {
            Vec3 diff = Direction;
            float scale = -p.Normal.Dot(StartPoint - p.Point) / p.Normal.Dot(diff);

            if (float.IsNaN(scale) || scale < 0.0f || scale > 1.0f)
            {
                result = new Vec3();
                return false;
            }

            result = StartPoint + (diff * scale);
            return true;
        }
        public Vec3 PointAtNormalizedLineDistance(float distance)
        {
            return StartPoint + Direction * distance;
        }
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
    }
}
