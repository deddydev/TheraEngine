using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Maths;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models;
using TheraEngine.Shapes;
using static System.Math;

namespace TheraEngine.Core.Shapes
{
    [TFileDef("Sphere")]
    public class Sphere : TShape
    {
        private float _radius = 1.0f;
        private EventVec3 _center = Vec3.Zero;

        [TSerialize]
        [Category("Sphere")]
        public float Radius
        {
            get => _radius;
            set => _radius = Abs(value);
        }
        [TSerialize]
        [Category("Sphere")]
        public EventVec3 Center
        {
            get => _center;
            set => _center = value ?? Vec3.Zero;
        }

        public Sphere() 
            : this(0.0f, Vec3.Zero) { }
        public Sphere(Vec3 center)
            : this(0.0f, center) { }
        public Sphere(float radius)
            : this(radius, Vec3.Zero) { }
        public Sphere(float radius, Vec3 center) 
        {
            _radius = Abs(radius);
            Center = center;
        }
        public Sphere(Vec3 point1, Vec3 point2)
        {
            Center = (point1 + point2) / 2.0f;
            _radius = Center.DistanceToFast(point2);
        }
        public Sphere(params Vec3[] points)
        {
            Miniball ball = new Miniball(PointSetArray.FromVectors(points));
            Center = new Vec3(ball.Center[0], ball.Center[1], ball.Center[2]);
            _radius = ball.Radius;
        }

        public override TCollisionShape GetCollisionShape()
            => TCollisionSphere.New(Radius);

        #region Meshes
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
            => SolidMesh(includeCenter ? Center.Raw : Vec3.Zero, _radius, slices, stacks);
        public PrimitiveData GetMesh(uint precision, bool includeCenter)
            => SolidMesh(includeCenter ? Center.Raw : Vec3.Zero, _radius, precision);
        #endregion

        #region Containment
        public override bool Contains(Vec3 point)
            => Collision.SphereContainsPoint(Center, Radius, point);
        public override EContainment Contains(BoundingBox box)
            => Collision.SphereContainsAABB(Center, Radius, box.Minimum, box.Maximum);
        public override EContainment Contains(BoundingBoxStruct box)
            => Collision.SphereContainsAABB(Center, Radius, box.Minimum, box.Maximum);
        public override EContainment Contains(Box box)
            => Collision.SphereContainsBox(Center, Radius, box.HalfExtents, box.Transform.InverseMatrix);
        public override EContainment Contains(Sphere sphere)
            => Collision.SphereContainsSphere(Center, Radius, sphere.Center, sphere.Radius);
        public override EContainment Contains(Cone cone)
        {
            //TODO
            return EContainment.Contains;
        }
        public override EContainment Contains(Cylinder cylinder)
        {
            //TODO
            return EContainment.Contains;
        }
        public override EContainment Contains(Capsule capsule)
        {
            Vec3 top = capsule.GetTopCenterPoint();
            Vec3 bot = capsule.GetBottomCenterPoint();

            float topDist = top.DistanceToFast(Center);
            float botDist = bot.DistanceToFast(Center);
            float distToCenter = Segment.ShortestDistanceToPoint(bot, top, Center);

            bool containsTop = topDist + _radius < Radius;
            bool containsBot = botDist + _radius < Radius;
            bool containsSides = distToCenter + _radius < Radius;

            if (containsTop != containsBot)
                return EContainment.Intersects;
            if (containsBot && containsTop)
                return containsSides ? EContainment.Contains : EContainment.Intersects;
            else
                return containsSides ? EContainment.Intersects : EContainment.Disjoint;
        }
        #endregion

        public override TShape HardCopy()
            => new Sphere(Radius, Center);
        
        public override Vec3 ClosestPoint(Vec3 point)
        {
            Vec3 dir = point - Center;
            float lenSq = dir.LengthSquared;
            if (lenSq > _radius * _radius)
                return dir * TMath.InverseSqrtFast(lenSq) * _radius;
            return point;
        }

        public override BoundingBox GetAABB()
            => new BoundingBox(Radius, Center);
        
        public override void Render(bool shadowPass)
            => Engine.Renderer.RenderSphere(Center, Radius, RenderSolid, Color.Red);

        public override void SetTransformMatrix(Matrix4 matrix) 
            => Center.Raw = matrix.Translation;
        public override Matrix4 GetTransformMatrix() 
            => Center.AsTranslationMatrix();
    }
}