using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using System.Runtime.InteropServices;
using CustomEngine;
using System.IO;
using System.Xml;

namespace System
{
    public class Plane : FileObject
    {
        public override ResourceType ResourceType => ResourceType.Plane;

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

        public Plane()
        {
            _normal = Vec3.Up;
            _distance = 0;
        }
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
            //Ax + By + Cz + D = 0
            //Ax + By + Cz = -D
            //-(Ax + By + Cz) = D
            //normal = (A, B, C)
            //point = (x, y, z)
            //Distance is negative dot product between normal and point
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
            //Get two difference vectors between points
            Vec3 v = point1 - point0;
            Vec3 u = point2 - point0;
            //Cross them to get normal vector
            _normal = v ^ u;
            _normal.NormalizeFast();
            //Solve for distance
            _distance = -point0.Dot(_normal);
        }
        public float Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        public Vec3 Point
        {
            //Ax + By + Cz + D = 0
            //Ax + By + Cz = -D
            //(x, y, z) = -D * (A, B, C)
            get { return _normal * -_distance; }
            set { _distance = -value.Dot(_normal); }
        }
        public Vec3 Normal
        {
            get { return _normal; }
            set
            {
                Vec3 point = Point;
                _normal = value;
                _normal.NormalizeFast();
                Point = point;
            }
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
        public void FlipNormal()
        {
            Normal = -Normal;
        }
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
        {
            return Collision.ClosestPointPlanePoint(this, point);
        }
        public float DistanceTo(Vec3 point)
        {
            return Collision.DistancePlanePoint(this, point);
        }
        public Plane TransformedBy(Matrix4 transform)
        {
            Vec3 point = Point;
            Vec3 normal = Normal;
            return new Plane(Vec3.TransformPosition(point, transform), Vec3.TransformNormal(normal, transform));
        }
        public PrimitiveData GetWireframeMesh(float xExtent, float yExtent)
        {
            return WireframeMesh(Point, Normal.LookatAngles(), xExtent, yExtent);
        }
        public PrimitiveData GetSolidMesh(float xExtent, float yExtent, Culling culling)
        {
            return SolidMesh(Point, Normal.LookatAngles(), xExtent, yExtent, culling);
        }
        public static PrimitiveData WireframeMesh(Vec3 position, Rotator rotation, float xExtent, float yExtent)
        {
            Vertex v0 = new Vertex();
            Vertex v1 = new Vertex();
            Vertex v2 = new Vertex();
            Vertex v3 = new Vertex();
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _hasNormals = false, _texcoordCount = 0 }, new VertexLineStrip(true, v0, v1, v2, v3));
        }
        public static PrimitiveData SolidMesh(Vec3 position, Rotator rotation, float xExtent, float yExtent, Culling culling)
        {
            float xHalf = xExtent / 2.0f;
            float yHalf = yExtent / 2.0f;
            Vec3 topFront = new Vec3(0.0f, yHalf, xHalf);
            Vec3 topBack = new Vec3(0.0f, yHalf, -xHalf);
            Vec3 bottomFront = new Vec3(0.0f, -yHalf, xHalf);
            Vec3 bottomBack = new Vec3(0.0f, -yHalf, -xHalf);
            Vec3 normal = Vec3.Up;
            Vertex v0 = new Vertex(bottomFront, normal, new Vec2(0.0f, 0.0f));
            Vertex v1 = new Vertex(bottomBack, normal, new Vec2(1.0f, 0.0f));
            Vertex v2 = new Vertex(topBack, normal, new Vec2(1.0f, 1.0f));
            Vertex v3 = new Vertex(topFront, normal, new Vec2(0.0f, 1.0f));
            return PrimitiveData.FromQuads(culling, new PrimitiveBufferInfo(), new VertexQuad(v0, v1, v2, v3));
        }

        public unsafe override void Write(VoidPtr address, StringTable table)
        {
            *(Header*)address = this;
        }

        public unsafe override void Read(VoidPtr address, VoidPtr strings)
        {
            Header h = *(Header*)address;
            _normal = h._normal;
            _distance = h._distance;
        }

        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("aabb");
            writer.WriteElementString("normal", _normal.ToString(false, false));
            writer.WriteElementString("distance", _distance.ToString());
            writer.WriteEndElement();
        }

        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("aabb", true))
                throw new Exception();
            while (reader.BeginElement())
            {
                if (reader.Name.Equals("normal", true))
                    _normal = Vec3.Parse(reader.ReadElementString());
                else if (reader.Name.Equals("distance", true))
                    _distance = float.Parse(reader.ReadElementString());
                reader.EndElement();
            }
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public float _distance;
            public BVec3 _normal;

            public static implicit operator Header(Plane p)
            {
                return new Header()
                {
                    _distance = p._distance,
                    _normal = p._normal,
                };
            }
            public static implicit operator Plane(Header h)
            {
                return new Plane(h._normal, h._distance);
            }
        }
    }
}
