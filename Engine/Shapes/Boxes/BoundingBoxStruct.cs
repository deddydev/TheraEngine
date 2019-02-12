using System;
using System.Globalization;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Shapes
{
    public struct BoundingBoxStruct
    {
        public Vec3 HalfExtents;
        public Vec3 Translation;
        public Vec3 Minimum
        {
            get => Translation - HalfExtents;
            set
            {
                Vec3 max = Maximum;
                Translation = (max + value) / 2.0f;
                HalfExtents = (max - value) / 2.0f;
            }
        }
        public Vec3 Maximum
        {
            get => Translation + HalfExtents;
            set
            {
                Vec3 min = Minimum;
                Translation = (value + min) / 2.0f;
                HalfExtents = (value - min) / 2.0f;
            }
        }

        #region Constructors
        public BoundingBoxStruct(float uniformHalfExtents)
            : this(new Vec3(uniformHalfExtents)) { }
        public BoundingBoxStruct(float uniformHalfExtents, Vec3 translation)
            : this(new Vec3(uniformHalfExtents), translation) { }
        public BoundingBoxStruct(float halfExtentX, float halfExtentY, float halfExtentZ)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ)) { }
        public BoundingBoxStruct(float halfExtentX, float halfExtentY, float halfExtentZ, Vec3 translation)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ), translation) { }
        public BoundingBoxStruct(float halfExtentX, float halfExtentY, float halfExtentZ, float x, float y, float z)
            : this(new Vec3(halfExtentX, halfExtentY, halfExtentZ), new Vec3(x, y, z)) { }
        public BoundingBoxStruct(Vec3 halfExtents)
            : this(halfExtents, Vec3.Zero) { }
        public BoundingBoxStruct(EventVec3 halfExtents)
            : this(halfExtents, new EventVec3(Vec3.Zero)) { }
        public BoundingBoxStruct(Vec3 halfExtents, Vec3 translation)
            : this()
        {
            HalfExtents = halfExtents;
            Translation = translation;
        }
        #endregion

        //Half extents is one of 8 octants that make up the box, so multiply half extent volume by 8
        public float GetVolume() =>
            HalfExtents.X * HalfExtents.Y * HalfExtents.Z * 8.0f;
        //Each half extent side is one of 4 quadrants on both sides of the box, so multiply each side area by 8
        public float GetSurfaceArea() =>
            HalfExtents.X * HalfExtents.Y * 8.0f +
            HalfExtents.Y * HalfExtents.Z * 8.0f +
            HalfExtents.Z * HalfExtents.X * 8.0f;

        /// <summary>
        /// Expands this bounding box to include the given point.
        /// </summary>
        public void Expand(Vec3 point)
        {
            Vec3 min = Vec3.ComponentMin(point, Minimum);
            Vec3 max = Vec3.ComponentMax(point, Maximum);
            Translation = (max + min) / 2.0f;
            HalfExtents = (max - min) / 2.0f;
        }
        public void Expand(BoundingBoxStruct box)
        {
            Vec3 min = Vec3.ComponentMin(box.Minimum, box.Maximum, Minimum);
            Vec3 max = Vec3.ComponentMax(box.Minimum, box.Maximum, Maximum);
            Translation = (max + min) / 2.0f;
            HalfExtents = (max - min) / 2.0f;
        }

        #region Collision
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBoxStruct"/>.
        /// </summary>
        public bool Intersects(Ray ray)
            => Collision.RayIntersectsAABBDistance(ray.StartPoint, ray.Direction, Minimum, Maximum, out float distance);
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBoxStruct"/>.
        /// Returns the distance of the closest intersection.
        /// </summary>
        public bool Intersects(Ray ray, out float distance)
            => Collision.RayIntersectsAABBDistance(ray, Minimum, Maximum, out distance);
        public bool Intersects(Vec3 start, Vec3 direction, out float distance)
            => Collision.RayIntersectsAABBDistance(start, direction, Minimum, Maximum, out distance);
        /// <summary>
        /// Returns true if the given <see cref="Ray"/> intersects this <see cref="BoundingBoxStruct"/>.
        /// Returns the position of the closest intersection.
        /// </summary>
        public bool Intersects(Ray ray, out Vec3 point)
            => Collision.RayIntersectsAABB(ray, Minimum, Maximum, out point);
        //public bool Intersects(Vec3 start, Vec3 direction, out Vec3 point)
        //    => Collision.RayIntersectsAABB(start, direction, Minimum, Maximum, out point);

        public bool Contains(Vec3 point)
            => Collision.AABBContainsPoint(Minimum, Maximum, point);
        public EContainment Contains(BoundingBox box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public EContainment Contains(BoundingBoxStruct box)
            => Collision.AABBContainsAABB(Minimum, Maximum, box.Minimum, box.Maximum);
        public EContainment Contains(Box box)
            => Collision.AABBContainsBox(Minimum, Maximum, box.HalfExtents, box.Transform.Matrix);
        public EContainment Contains(Sphere sphere)
            => Collision.AABBContainsSphere(Minimum, Maximum, sphere.Center, sphere.Radius);
        public EContainment Contains(Cone cone)
        {
            bool top = Contains(cone.GetTopPoint());
            bool bot = Contains(cone.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(Cylinder cylinder)
        {
            bool top = Contains(cylinder.GetTopCenterPoint());
            bool bot = Contains(cylinder.GetBottomCenterPoint());
            if (top && bot)
                return EContainment.Contains;
            else if (!top && !bot)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(Capsule capsule)
        {
            Vec3 top = capsule.GetTopCenterPoint();
            Vec3 bot = capsule.GetBottomCenterPoint();
            Vec3 radiusVec = new Vec3(capsule.Radius);
            Vec3 capsuleMin = Vec3.ComponentMin(top, bot) - radiusVec;
            Vec3 capsuleMax = Vec3.ComponentMax(top, bot) + radiusVec;
            Vec3 min = Minimum;
            Vec3 max = Maximum;

            bool containsX = false, containsY = false, containsZ = false;
            bool disjointX = false, disjointY = false, disjointZ = false;

            containsX = capsuleMin.X >= min.X && capsuleMax.X <= max.X;
            containsY = capsuleMin.Y >= min.Y && capsuleMax.Y <= max.Y;
            containsZ = capsuleMin.Z >= min.Z && capsuleMax.Z <= max.Z;

            if (!containsX) disjointX = capsuleMax.X < min.X || capsuleMin.X > max.X;
            if (!containsY) disjointY = capsuleMax.Y < min.Y || capsuleMin.Y > max.Y;
            if (!containsZ) disjointZ = capsuleMax.Z < min.Z || capsuleMin.Z > max.Z;

            if (containsX && containsY && containsZ)
                return EContainment.Contains;
            if (disjointX && disjointY && disjointZ)
                return EContainment.Disjoint;
            return EContainment.Intersects;
        }
        public EContainment Contains(TShape shape)
        {
            if (shape != null)
            {
                if (shape is BoundingBox bb)
                    return Contains(bb);
                else if (shape is Box box)
                    return Contains(box);
                else if (shape is Sphere sphere)
                    return Contains(sphere);
                else if (shape is Cone cone)
                    return Contains(cone);
                else if (shape is Cylinder cylinder)
                    return Contains(cylinder);
                else if (shape is Capsule capsule)
                    return Contains(capsule);
            }
            return EContainment.Contains;
        }
        #endregion

        #region Static Constructors
        /// <summary>
        /// Creates a new bounding box from minimum and maximum coordinates.
        /// </summary>
        public static BoundingBoxStruct FromMinMax(Vec3 min, Vec3 max)
            => new BoundingBoxStruct((max - min) * 0.5f, (max + min) * 0.5f);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBoxStruct FromHalfExtentsTranslation(Vec3 halfExtents, Vec3 translation)
            => new BoundingBoxStruct(halfExtents, translation);
        /// <summary>
        /// Creates a new bounding box from half extents and a translation.
        /// </summary>
        public static BoundingBoxStruct FromHalfExtentsTranslation(EventVec3 halfExtents, EventVec3 translation)
            => new BoundingBoxStruct(halfExtents, translation);
        /// <summary>
        /// Creates a bounding box that encloses the given sphere.
        /// </summary>
        public static BoundingBoxStruct EnclosingSphere(Sphere sphere)
            => FromMinMax(sphere.Center - sphere.Radius, sphere.Center + sphere.Radius);
        /// <summary>
        /// Creates a bounding box that includes both given bounding boxes.
        /// </summary>
        public static BoundingBoxStruct Merge(BoundingBoxStruct box1, BoundingBoxStruct box2)
            => FromMinMax(Vec3.ComponentMin(box1.Minimum, box2.Maximum), Vec3.ComponentMax(box1.Maximum, box2.Maximum));
        #endregion

        public static bool operator ==(BoundingBoxStruct left, BoundingBoxStruct right) => left.Equals(ref right);
        public static bool operator !=(BoundingBoxStruct left, BoundingBoxStruct right) => !left.Equals(ref right);

        public bool Equals(ref BoundingBoxStruct value)
            => Minimum == value.Minimum && Maximum == value.Maximum;

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "Minimum:{0} Maximum:{1}", Minimum.ToString(), Maximum.ToString());
        }
        public override bool Equals(object value)
        {
            if (!(value is BoundingBoxStruct))
                return false;

            var strongValue = (BoundingBoxStruct)value;
            return Equals(ref strongValue);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (Minimum.GetHashCode() * 397) ^ Maximum.GetHashCode();
            }
        }
        public Vec3 ClosestPoint(Vec3 point)
            => Collision.ClosestPointAABBPoint(Minimum, Maximum, point);
    }
}
