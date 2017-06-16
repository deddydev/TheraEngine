using static System.Math;
using TheraEngine;
using TheraEngine.Rendering.Models;
using BulletSharp;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;

namespace System
{
    public class Sphere : Shape
    {
        //public const string XMLTag = "sphere";

        public static List<Sphere> Active = new List<Sphere>();

        [DefaultValue(1.0f)]
        [Serialize("Radius")]
        private float _radius = 1.0f;
        [DefaultValue("0 0 0")]
        [Serialize("Center")]
        private Vec3 _center = Vec3.Zero;

        public float Radius
        {
            get => _radius;
            set => _radius = Abs(value);
        }
        public Vec3 Center
        {
            get => _center;
            set => _center = value;
        }

        public Sphere(float radius)
            : this(radius, Vec3.Zero) { }
        public Sphere(float radius, Vec3 center) 
            : this()
        {
            _radius = Abs(radius);
            _center = center;
        }
        public Sphere()
        {
            ShapeIndex = Active.Count;
            Active.Add(this);
        }
        ~Sphere()
        {
            Active.Remove(this);
        }

        public override CollisionShape GetCollisionShape() => new SphereShape(Radius);
        public override void Render()
            => Engine.Renderer.RenderSphere(ShapeName, Center, Radius, _renderSolid, Color.Red);
        public static PrimitiveData SolidMesh(Vec3 center, float radius, float precision)
        {
            float halfPI = CustomMath.PIf * 0.5f;
            float invPrecision = 1.0f / precision;
            float twoPIThroughPrecision = CustomMath.PIf * 2.0f * invPrecision;

            float theta1, theta2, theta3;
            Vec3 norm, pos;
            Vec2 uv;

            List<VertexTriangleStrip> strips = new List<VertexTriangleStrip>();
            for (uint j = 0; j < precision * 0.5f; j++)
            {
                theta1 = (j * twoPIThroughPrecision) - halfPI;
                theta2 = ((j + 1) * twoPIThroughPrecision) - halfPI;

                Vertex[] stripVertices = new Vertex[((int)precision + 1) * 2];
                int x = 0;
                for (uint i = 0; i <= precision; i++)
                {
                    theta3 = i * twoPIThroughPrecision;

                    norm.X = -(float)(Cos(theta2) * Cos(theta3));
                    norm.Y = -(float)Sin(theta2);
                    norm.Z = -(float)(Cos(theta2) * Sin(theta3));
                    pos = center + radius * norm;
                    uv.X = i * invPrecision;
                    uv.Y = 2.0f * (j + 1) * invPrecision;

                    stripVertices[x++] = new Vertex(pos, norm, uv);

                    norm.X = -(float)(Cos(theta1) * Cos(theta3));
                    norm.Y = -(float)Sin(theta1);
                    norm.Z = -(float)(Cos(theta1) * Sin(theta3));
                    pos = center + radius * norm;
                    uv.X = i * invPrecision;
                    uv.Y = 2.0f * j * invPrecision;

                    stripVertices[x++] = new Vertex(pos, norm, uv);
                }
                strips.Add(new VertexTriangleStrip(stripVertices));
            }

            return PrimitiveData.FromTriangleList(Culling.Back, new PrimitiveBufferInfo(), strips.SelectMany(x => x.ToTriangles()));
        }

        public static PrimitiveData WireframeMesh(Vec3 center, float radius, int pointCount)
        {
            VertexLineStrip d1 = Circle3D.LineStrip(radius, Vec3.Forward, center, pointCount);
            VertexLineStrip d2 = Circle3D.LineStrip(radius, Vec3.Up, center, pointCount);
            VertexLineStrip d3 = Circle3D.LineStrip(radius, Vec3.Right, center, pointCount);
            return PrimitiveData.FromLineStrips(new PrimitiveBufferInfo() { _texcoordCount = 0, _hasNormals = false }, d1, d2, d3);
        }

        public static PrimitiveData SolidMesh(Vec3 center, float radius, int slices, int stacks)
        {
            List<Vertex> v = new List<Vertex>();
            float twoPi = CustomMath.PIf * 2.0f;
            for (int i = 0; i <= stacks; ++i)
            {
                // V texture coordinate.
                float V = i / (float)stacks;
                float phi = V * CustomMath.PIf;

                for (int j = 0; j <= slices; ++j)
                {
                    // U texture coordinate.
                    float U = j / (float)slices;
                    float theta = U * twoPi;

                    float X = (float)Cos(theta) * (float)Sin(phi);
                    float Y = (float)Cos(phi);
                    float Z = (float)Sin(theta) * (float)Sin(phi);

                    Vec3 normal = new Vec3(X, Y, Z);
                    v.Add(new Vertex(center + normal * radius, normal, new Vec2(U, V)));
                }
            }
            List<VertexTriangle> triangles = new List<VertexTriangle>();
            for (int i = 0; i < slices * stacks + slices; ++i)
            {
                triangles.Add(new VertexTriangle(v[i], v[i + slices + 1], v[i + slices]));
                triangles.Add(new VertexTriangle(v[i + slices + 1], v[i], v[i + 1]));
            }
            return PrimitiveData.FromTriangleList(Culling.Back, new PrimitiveBufferInfo(), triangles);
        }

        public PrimitiveData GetMesh(int slices, int stacks, bool includeCenter)
            => SolidMesh(includeCenter ? Center : Vec3.Zero, _radius, slices, stacks);
        public PrimitiveData GetMesh(float precision, bool includeCenter)
            => SolidMesh(includeCenter ? Center : Vec3.Zero, _radius, precision);
        
        public override bool Contains(Vec3 point)
            => Collision.SphereContainsPoint(Center, Radius, point);
        public override EContainment Contains(BoundingBox box)
            => Collision.SphereContainsAABB(Center, Radius, box.Minimum, box.Maximum);
        public override EContainment Contains(Box box)
            => Collision.SphereContainsBox(Center, Radius, box.HalfExtents, box.WorldMatrix);
        public override EContainment Contains(Sphere sphere)
            => Collision.SphereContainsSphere(Center, Radius, sphere.Center, sphere.Radius); 
        public override EContainment ContainedWithin(BoundingBox box)
            => box.Contains(this);
        public override EContainment ContainedWithin(Box box)
            => box.Contains(this);
        public override EContainment ContainedWithin(Sphere sphere)
            => sphere.Contains(this);
        public override EContainment ContainedWithin(Frustum frustum)
            => frustum.Contains(this);

        public override void SetTransform(Matrix4 worldMatrix)
        {
            _center = worldMatrix.GetPoint();
            base.SetTransform(worldMatrix);
        }
        
        public override Shape HardCopy()
            => new Sphere(Radius, Center);
        
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            Sphere s = new Sphere(Radius, Center);
            s.SetTransform(worldMatrix);
            return s;
        }

        //protected override int OnCalculateSize(StringTable table)
        //    => Header.Size;
        //public unsafe override void Write(VoidPtr address, StringTable table)
        //    => *(Header*)address = this;
        //public unsafe override void Read(VoidPtr address, VoidPtr strings)
        //{
        //    Header h = *(Header*)address;
        //    _radius = h._radius;
        //    _center = h._center;
        //}
        //public override void Write(XmlWriter writer)
        //{
        //    writer.WriteStartElement(XMLTag);
        //    writer.WriteAttributeString("radius", _radius.ToString());
        //    if (_center != Vec3.Zero)
        //        writer.WriteElementString("center", _center.ToString(false, false));
        //    writer.WriteEndElement();
        //}
        //public override void Read(XMLReader reader)
        //{
        //    if (!reader.Name.Equals(XMLTag, true))
        //        throw new Exception();
        //    _radius = 0;
        //    _center = Vec3.Zero;
        //    while (reader.ReadAttribute())
        //    {
        //        if (reader.Name.Equals("radius", true))
        //            _radius = float.Parse((string)reader.Value);
        //    }
        //    while (reader.BeginElement())
        //    {
        //        if (reader.Name.Equals("center", true))
        //            _center = Vec3.Parse(reader.ReadElementString());
        //        reader.EndElement();
        //    }
        //}

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct Header
        //{
        //    public const int Size = 0x10;

        //    public float _radius;
        //    public BVec3 _center;

        //    public static implicit operator Header(Sphere s)
        //    {
        //        return new Header()
        //        {
        //            _radius = s._radius,
        //            _center = s._center,
        //        };
        //    }
        //    public static implicit operator Sphere(Header h)
        //    {
        //        return new Sphere(h._radius, h._center);
        //    }
        //}
    }
}