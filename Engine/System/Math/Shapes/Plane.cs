namespace System
{
    public class Plane
    {
        /*      
         * Represents a plane a certain distance from the origin.
         * Ax + By + Cz + D = 0
         * D is distance from the origin
         * 
         *      ________
         *     /       /
         *    /   \   /
         *   /_____\_/
         *          \
         *          origin
         */

        private Vec3 _normal;
        private float _distance;

        public Plane() { }
        /// <summary>
        /// Constructs a plane given a point.
        /// The normal points in the direction of the origin.
        /// </summary>
        public Plane(Vec3 point)
        {
            _normal = (-point).NormalizedFast();
            _distance = point.LengthFast;
        }
        /// <summary>
        /// Constructs a plane given a normal and distance from the origin.
        /// </summary>
        public Plane(Vec3 normal, float distance)
        {
            normal.NormalizeFast();
            _normal = normal;
            _distance = distance;
        }
        /// <summary>
        /// Constructs a plane given a point and normal.
        /// </summary>
        public Plane(Vec3 point, Vec3 normal)
        {
            normal.NormalizeFast();
            _normal = normal;
            _distance = -point.Dot(normal);
        }
        /// <summary>
        /// Constructs a plane given three points.
        /// Points must be specified in this order 
        /// to ensure the normal points in the right direction.
        ///   ^
        ///   |   p2
        /// n |  /
        ///   | / u
        ///   |/_______ p1
        ///  p0    v
        /// </summary>
        public Plane(Vec3 point0, Vec3 point1, Vec3 point2)
        {
            Vec3 v = point1 - point0;
            Vec3 u = point2 - point0;
            _normal = v.Cross(u).NormalizedFast();
            _distance = -point0.Dot(_normal);
        }
        public float Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        public Vec3 Point
        {
            get { return _normal.Normalized() * -_distance; }
            set { _distance = -value.Dot(_normal); }
        }
        public Vec3 Normal
        {
            get { return _normal; }
            set { _normal = value; }
        }
        public void Normalize()
        {
            float magnitude = 1.0f / Normal.LengthFast;
            _normal *= magnitude;
            _distance *= magnitude;
        }
        public EPlaneIntersection IntersectsBox(Box box)
        {
            Vec3 min;
            Vec3 max;

            max.X = (Normal.X >= 0.0f) ? box.Minimum.X : box.Maximum.X;
            max.Y = (Normal.Y >= 0.0f) ? box.Minimum.Y : box.Maximum.Y;
            max.Z = (Normal.Z >= 0.0f) ? box.Minimum.Z : box.Maximum.Z;
            min.X = (Normal.X >= 0.0f) ? box.Maximum.X : box.Minimum.X;
            min.Y = (Normal.Y >= 0.0f) ? box.Maximum.Y : box.Minimum.Y;
            min.Z = (Normal.Z >= 0.0f) ? box.Maximum.Z : box.Minimum.Z;

            if (Normal.Dot(max) + _distance > 0.0f)
                return EPlaneIntersection.Front;

            if (Normal.Dot(min) + _distance < 0.0f)
                return EPlaneIntersection.Back;

            return EPlaneIntersection.Intersecting;
        }
        public EPlaneIntersection IntersectsSphere(float radius, Vec3 center)
        {
            float dot = center.Dot(Normal) + _distance;

            if (dot > radius)
                return EPlaneIntersection.Front;

            if (dot < -radius)
                return EPlaneIntersection.Back;

            return EPlaneIntersection.Intersecting;
        }

        public Plane TransformedBy(Matrix4 transform)
        {
            Vec3 point = Point;
            Vec3 normal = Normal;
            return new Plane();
        }
    }
}
