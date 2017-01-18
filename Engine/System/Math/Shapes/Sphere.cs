using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using BulletSharp;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Worlds.Actors.Components;

namespace System
{
    public class Sphere : Shape
    {
        private float _radius;
        private Vec3 _center;
        public float Radius
        {
            get { return _radius; }
            set { _radius = Abs(value); }
        }
        public Vec3 Center
        {
            get { return _center; }
            set { _center = value; }
        }
        public Sphere(float radius) : this(radius, Vec3.Zero) { }
        public Sphere(float radius, Vec3 center) : base()
        {
            _radius = Abs(radius);
            _center = center;
        }
        public override CollisionShape GetCollisionShape() { return new SphereShape(Radius); }
        public override void Render() { Engine.Renderer.RenderSphere(Center, Radius, _renderSolid); }
        public static PrimitiveData Mesh(Vec3 center, float radius, float precision)
        {
            float halfPI = CustomMath.PIf * 0.5f;
            float invPrecision = 1.0f / precision;
            float twoPIThroughPrecision = CustomMath.PIf * 2.0f * invPrecision;

            float theta1, theta2, theta3;
            Vec3 norm, pos;
            Vec2 uv;

            List<VertexTriangleStrip> strips = new List<VertexTriangleStrip>();
            for (uint j = 0; j < precision / 2; j++)
            {
                theta1 = (j * twoPIThroughPrecision) - halfPI;
                theta2 = ((j + 1) * twoPIThroughPrecision) - halfPI;

                Vertex[] stripVertices = new Vertex[((int)precision + 1) * 2];
                int x = 0;
                for (uint i = 0; i <= precision; i++)
                {
                    theta3 = i * twoPIThroughPrecision;

                    norm.X = (float)(Cos(theta2) * Cos(theta3));
                    norm.Y = (float)Sin(theta2);
                    norm.Z = (float)(Cos(theta2) * Sin(theta3));
                    pos = center + radius * norm;
                    uv.X = i * invPrecision;
                    uv.Y = 2.0f * (j + 1) * invPrecision;

                    stripVertices[x++] = new Vertex(pos, norm, uv);

                    norm.X = (float)(Cos(theta1) * Cos(theta3));
                    norm.Y = (float)Sin(theta1);
                    norm.Z = (float)(Cos(theta1) * Sin(theta3));
                    pos = center + radius * norm;
                    uv.X = i * invPrecision;
                    uv.Y = 2.0f * j * invPrecision;

                    stripVertices[x++] = new Vertex(pos, norm, uv);
                }
                strips.Add(new VertexTriangleStrip(stripVertices));
            }

            return PrimitiveData.FromTriangleList(Culling.Back, new PrimitiveBufferInfo(), strips.SelectMany(x => x.ToTriangles()));
        }
        public static PrimitiveData Mesh(Vec3 center, float radius, int slices, int stacks)
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
        {
            return Mesh(includeCenter ? Center : Vec3.Zero, _radius, slices, stacks);
        }
        public PrimitiveData GetMesh(float precision, bool includeCenter)
        {
            return Mesh(includeCenter ? Center : Vec3.Zero, _radius, precision);
        }
        public override bool Contains(Vec3 point) { return Collision.SphereContainsPoint(Center, Radius, point); }
        public override EContainment Contains(BoundingBox box) { return Collision.SphereContainsAABB(Center, Radius, box.Minimum, box.Maximum); }
        public override EContainment Contains(Box box) { return Collision.SphereContainsBox(Center, Radius, box.HalfExtents, box.WorldMatrix); }
        public override EContainment Contains(Sphere sphere) { return Collision.SphereContainsSphere(Center, Radius, sphere.Center, sphere.Radius); }
        public override EContainment ContainedWithin(BoundingBox box) { return box.Contains(this); }
        public override EContainment ContainedWithin(Box box) { return box.Contains(this); }
        public override EContainment ContainedWithin(Sphere sphere) { return sphere.Contains(this); }
        public override EContainment ContainedWithin(Frustum frustum) { return frustum.Contains(this); }

        public override void SetTransform(Matrix4 worldMatrix)
        {
            _center = Vec3.TransformPosition(_center, worldMatrix);
        }
        public override Shape HardCopy()
        {
            return new Sphere(Radius, Center);
        }
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            Sphere s = new Sphere(Radius, Center);
            s.SetTransform(worldMatrix);
            return s;
        }
    }
}