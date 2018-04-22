using static System.Math;
using TheraEngine.Rendering.Models;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using TheraEngine.Maths;
using System;
using TheraEngine.Physics;

namespace TheraEngine.Core.Shapes
{
    [FileDef("Sphere")]
    public class Sphere : Shape
    {
        [TSerialize("Radius")]
        private float _radius = 1.0f;
        [TSerialize("Center")]
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

        public Sphere() : this(0.0f, Vec3.Zero) { }
        public Sphere(Vec3 center)
            : this(0.0f, center) { }
        public Sphere(float radius)
            : this(radius, Vec3.Zero) { }
        public Sphere(float radius, Vec3 center) 
        {
            _radius = Abs(radius);
            _center = center;
        }
        public Sphere(Vec3 point1, Vec3 point2)
        {
            _center = (point1 + point2) / 2.0f;
            _radius = _center.DistanceToFast(point2);
        }
        public Sphere(params Vec3[] points)
        {
            Miniball ball = new Miniball(PointSetArray.FromVectors(points));
            _center = new Vec3(ball.Center[0], ball.Center[1], ball.Center[2]);
            _radius = ball.Radius;
        }

        public override TCollisionShape GetCollisionShape()
            => TCollisionSphere.New(Radius);

        public override void Render()
            => Engine.Renderer.RenderSphere(Center, Radius, _renderSolid, Color.Red);

        public static PrimitiveData SolidMesh(Vec3 center, float radius, uint precision)
        {
            float halfPI = TMath.PIf * 0.5f;
            float invPrecision = 1.0f / precision;
            float twoPIThroughPrecision = TMath.PIf * 2.0f * invPrecision;

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

            return PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), strips.SelectMany(x => x.ToTriangles()));
        }

        public static PrimitiveData WireframeMesh(Vec3 center, float radius, int pointCount)
        {
            VertexLineStrip d1 = Circle3D.LineStrip(radius, Vec3.Forward, center, pointCount);
            VertexLineStrip d2 = Circle3D.LineStrip(radius, Vec3.Up, center, pointCount);
            VertexLineStrip d3 = Circle3D.LineStrip(radius, Vec3.Right, center, pointCount);
            return PrimitiveData.FromLineStrips(VertexShaderDesc.JustPositions(), d1, d2, d3);
        }

        public static PrimitiveData SolidMesh(Vec3 center, float radius, int slices, int stacks)
        {
            List<Vertex> v = new List<Vertex>();
            float twoPi = TMath.PIf * 2.0f;
            for (int i = 0; i <= stacks; ++i)
            {
                // V texture coordinate.
                float V = i / (float)stacks;
                float phi = V * TMath.PIf;

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
            return PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), triangles);
        }

        public PrimitiveData GetMesh(int slices, int stacks, bool includeCenter)
            => SolidMesh(includeCenter ? Center : Vec3.Zero, _radius, slices, stacks);
        public PrimitiveData GetMesh(uint precision, bool includeCenter)
            => SolidMesh(includeCenter ? Center : Vec3.Zero, _radius, precision);
        
        public override bool Contains(Vec3 point)
            => Collision.SphereContainsPoint(Center, Radius, point);
        public override EContainment Contains(BoundingBox box)
            => Collision.SphereContainsAABB(Center, Radius, box.Minimum, box.Maximum);
        public override EContainment Contains(Box box)
            => Collision.SphereContainsBox(Center, Radius, box.HalfExtents, box.InverseWorldMatrix);
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

        public override void SetRenderTransform(Matrix4 worldMatrix)
        {
            _center = worldMatrix.Translation;
            base.SetRenderTransform(worldMatrix);
        }
        
        public override Shape HardCopy()
            => new Sphere(Radius, Center);
        
        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            Sphere s = new Sphere(Radius, Center);
            s.SetRenderTransform(worldMatrix);
            return s;
        }

        public override Matrix4 GetTransformMatrix()
            => _center.AsTranslationMatrix();

        public override Vec3 ClosestPoint(Vec3 point)
        {
            Vec3 dir = point - _center;
            float lenSq = dir.LengthSquared;
            if (lenSq > _radius * _radius)
                return dir * TMath.InverseSqrtFast(lenSq) * _radius;
            return point;
        }

        public override BoundingBox GetAABB()
            => new BoundingBox(_radius, _center);
    }
}