using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using BulletSharp;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public class Sphere : Shape
    {
        private Vec3 _center = Vec3.Zero;
        private float _radius;

        public float Radius
        {
            get { return _radius; }
            set { _radius = Abs(value); }
        }
        public Vec3 Center
        {
            get { return Vec3.TransformPosition(_center, WorldMatrix); }
            set { _center = Vec3.TransformPosition(value, InverseWorldMatrix); }
        }
        
        public Sphere(float radius, Vec3 center)
        {
            _radius = Abs(radius);
            _center = center;
        }
        public override CollisionShape GetCollisionShape()
        {
            return new SphereShape(Radius);
        }
        public override void Render() { Render(true); }
        public override void Render(bool solid)
        {
            //if (solid)
            //    Engine.Renderer.DrawSphereSolid(this);
            //else
            //    Engine.Renderer.DrawSphereWireframe(this);
        }
        public override bool Contains(Vec3 point) { return Collision.SphereContainsPoint(this, point); }
        public override EContainment Contains(Box box) { return Collision.SphereContainsBox(this, box); }
        public override EContainment Contains(Sphere sphere) { return Collision.SphereContainsSphere(this, sphere); }
        public override EContainment Contains(Capsule capsule)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }
        public override PrimitiveData GetPrimitiveData()
        {
            const float Precision = 30;

            float halfPI = CustomMath.PIf * 0.5f;
            float oneThroughPrecision = 1.0f / Precision;
            float twoPIThroughPrecision = CustomMath.PIf * 2.0f * oneThroughPrecision;

            float theta1, theta2, theta3;
            Vec3 norm, pos;
            Vec2 uv;

            List<VertexTriangleStrip> strips = new List<VertexTriangleStrip>();
            for (uint j = 0; j < Precision / 2; j++)
            {
                theta1 = (j * twoPIThroughPrecision) - halfPI;
                theta2 = ((j + 1) * twoPIThroughPrecision) - halfPI;

                Vertex[] stripVertices = new Vertex[((int)Precision + 1) * 2];
                int x = 0;
                for (uint i = 0; i <= Precision; i++)
                {
                    theta3 = i * twoPIThroughPrecision;
                    
                    norm.X = (float)(Cos(theta2) * Cos(theta3));
                    norm.Y = (float)Sin(theta2);
                    norm.Z = (float)(Cos(theta2) * Sin(theta3));
                    pos = _center + _radius * norm;
                    uv.X = i * oneThroughPrecision;
                    uv.Y = 2.0f * (j + 1) * oneThroughPrecision;

                    stripVertices[x++] = new Vertex(pos, norm, uv);

                    norm.X = (float)(Cos(theta1) * Cos(theta3));
                    norm.Y = (float)Sin(theta1);
                    norm.Z = (float)(Cos(theta1) * Sin(theta3));
                    pos = _center + _radius * norm;
                    uv.X = i * oneThroughPrecision;
                    uv.Y = 2.0f * j * oneThroughPrecision;

                    stripVertices[x++] = new Vertex(pos, norm, uv);
                }
                strips.Add(new VertexTriangleStrip(stripVertices));
            }

            return PrimitiveData.FromTriangleList(Culling.Back, new PrimitiveBufferInfo(), strips.SelectMany(x => x.ToTriangles()));
        }
    }
}
