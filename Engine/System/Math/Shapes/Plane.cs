namespace System
{
    public class Plane
    {
        /*      
         * Represents a plane a certain distance from the origin.
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
            _normal = -point;
            _distance = point.Length;
        }
        /// <summary>
        /// Constructs a plane given a normal and distance from the origin.
        /// </summary>
        public Plane(Vec3 normal, float distance)
        {
            _normal = normal;
            _distance = distance;
        }
        /// <summary>
        /// Constructs a plane given a point and normal.
        /// </summary>
        public Plane(Vec3 point, Vec3 normal)
        {
            _normal = normal;
            _distance = -point.Dot(normal);
        }
        /// <summary>
        /// Constructs a plane given three points.
        /// </summary>
        public Plane(Vec3 point1, Vec3 point2, Vec3 point3)
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
            Normal = new Vec3(yz * invPyth, xz * invPyth, xy * invPyth);
            _distance = -((Normal.X * point1.X) + (Normal.Y * point1.Y) + (Normal.Z * point1.Z));
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
        public PlaneIntersection IntersectsBox(Box box)
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
                return PlaneIntersection.Front;

            if (Normal.Dot(min) + _distance < 0.0f)
                return PlaneIntersection.Back;

            return PlaneIntersection.Intersecting;
        }
        public PlaneIntersection IntersectsSphere(float radius, Vec3 center)
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
