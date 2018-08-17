using TheraEngine.Rendering.Models;
using System.ComponentModel;
using System;
using System.Runtime.InteropServices;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Represents a plane in 3D space using a normal and distance to the origin at (0,0,0).
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Plane
    {
        /*
        * Represents a plane a certain distance from the origin.
        * Ax + By + Cz + D = 0
        * D is distance from the origin
        *      ________
        *     /       /
        *    /   \   /
        *   /_____\_/
        *          \
        *          origin
        */
        
        private Vec3 _normal;
        private float _distance;

        [TSerialize]
        [Category("Plane")]
        public float Distance
        {
            get => _distance;
            set => _distance = value;
        }
        /// <summary>
        /// The intersection point of a line, which is perpendicular to the plane and passes through the origin, and the plane.
        /// Note that while you can set this point to anything, the original world position will be lost and the distance value will be updated
        /// so that the plane is coplanar with the point, using same normal.
        /// </summary>
        [Category("Plane")]
        [Description("The intersection point of a line, which is perpendicular to the plane " +
            "and passes through the origin, and the plane. " +
            "Note that while you can set this point to anything, the original world position will be lost +" +
            "and the distance value will be updated so that the plane is coplanar with the point, using same normal.")]
        public Vec3 IntersectionPoint
        {
            //Ax + By + Cz + D = 0
            //Ax + By + Cz = -D
            //(x, y, z) = -D * (A, B, C)
            get => _normal * -_distance;
            set => _distance = -value.Dot(_normal);
        }
        [TSerialize]
        [Category("Plane")]
        public Vec3 Normal
        {
            get => _normal;
            set
            {
                Vec3 point = IntersectionPoint;
                _normal = value;
                _normal.NormalizeFast();
                IntersectionPoint = point;
            }
        }

        //public Plane()
        //{
        //    _normal = Vec3.Up;
        //    _distance = 0;
        //}

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
        public Plane(float distance, Vec3 normal)
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
            //Ax + By + Cz + D = 0
            //Ax + By + Cz = -D
            //-(Ax + By + Cz) = D
            //normal = (A, B, C)
            //point = (x, y, z)
            //Distance is negative dot product between normal and point
            _distance = -point.Dot(normal);
        }

        /// <summary>
        /// Returns distance from the plane defined by a point and normal to the origin.
        /// </summary>
        /// <param name="planePoint">Point in space the plane intersects.</param>
        /// <param name="planeNormal">The normal of the plane.</param>
        /// <returns>Shortest distance to the origin from the plane.</returns>
        public static float ComputeDistance(Vec3 planePoint, Vec3 planeNormal)
            => -planePoint.Dot(planeNormal);

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
            //Get two difference vectors between points
            Vec3 v = point1 - point0;
            Vec3 u = point2 - point0;
            //Cross them to get normal vector
            _normal = v ^ u;
            _normal.NormalizeFast();
            //Solve for distance
            _distance = -point0.Dot(_normal);
        }
        public void NormalizeFast()
        {
            float magnitude = 1.0f / Normal.LengthFast;
            _normal *= magnitude;
            _distance *= magnitude;
        }
        public void Normalize()
        {
            float magnitude = 1.0f / Normal.Length;
            _normal *= magnitude;
            _distance *= magnitude;
        }
        public void FlipNormal() => Normal = -Normal;
        public EPlaneIntersection IntersectsBox(BoundingBox box)
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
        public Vec3 ClosestPoint(Vec3 point)
            => Collision.ClosestPointPlanePoint(this, point);
        public float DistanceTo(Vec3 point)
            => Collision.DistancePlanePoint(this, point);

        public Plane TransformedBy(Matrix4 transform)
            => new Plane(Vec3.TransformPosition(IntersectionPoint, transform), _normal * transform.GetRotationMatrix4());
        
        public void TransformBy(Matrix4 transform)
        {
            _normal = _normal * transform.GetRotationMatrix4();
            IntersectionPoint = IntersectionPoint * transform;
        }

        public PrimitiveData GetWireframeMesh(float xExtent, float yExtent)
            => WireframeMesh(IntersectionPoint, Normal, xExtent, yExtent);
        public PrimitiveData GetSolidMesh(float xExtent, float yExtent)
            => SolidMesh(IntersectionPoint, Normal, xExtent, yExtent);
        public static PrimitiveData WireframeMesh(Vec3 position, Vec3 normal, float xExtent, float yExtent)
        {
            Quat r = normal.LookatAngles().ToQuaternion();
            Vec3 bottomLeft = position + new Vec3(-0.5f * xExtent, -0.5f * yExtent, 0.0f) * r;
            Vec3 bottomRight = position + new Vec3(0.5f * xExtent, -0.5f * yExtent, 0.0f) * r;
            Vec3 topLeft = position + new Vec3(-0.5f * xExtent, 0.5f * yExtent, 0.0f) * r;
            Vec3 topRight = position + new Vec3(0.5f * xExtent, 0.5f * yExtent, 0.0f) * r;
            return PrimitiveData.FromLineStrips(VertexShaderDesc.JustPositions(), new VertexLineStrip(true, bottomLeft, bottomRight, topRight, topLeft));
        }
        public static PrimitiveData SolidMesh(Vec3 position, Vec3 normal, float xExtent, float yExtent)
        {
            Quat r = normal.LookatAngles().ToQuaternion();
            Vec3 bottomLeft = position + new Vec3(-0.5f * xExtent, -0.5f * yExtent, 0.0f) * r;
            Vec3 bottomRight = position + new Vec3(0.5f * xExtent, -0.5f * yExtent, 0.0f) * r;
            Vec3 topLeft = position + new Vec3(-0.5f * xExtent, 0.5f * yExtent, 0.0f) * r;
            Vec3 topRight = position + new Vec3(0.5f * xExtent, 0.5f * yExtent, 0.0f) * r;
            VertexQuad q = VertexQuad.MakeQuad(bottomLeft, bottomRight, topRight, topLeft, normal);
            return PrimitiveData.FromQuads(VertexShaderDesc.PosNormTex(), q);
        }
    }
}
