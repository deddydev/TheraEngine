using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using BulletSharp;

namespace System
{
    public class Cone : Shape
    {
        public float _radius, _height;
        
        public float Radius { get { return _radius; } set { _radius = value; } }
        public float Height { get { return _height; } set { _height = value; } }

        public Cone(float radius, float height)
        {
            _radius = Abs(radius);
            _height = Abs(height);
        }
        public override CollisionShape GetCollisionShape()
        {
            return new ConeShape(Radius, Height);
        }
        public override void Render() { Render(false); }
        public override void Render(bool solid)
        {
            //if (solid)
            //    Engine.Renderer.DrawCapsuleSolid(this);
            //else
            //    Engine.Renderer.DrawCapsuleWireframe(this);
        }
        public override PrimitiveData GetPrimitiveData()
        {
            List<VertexQuad> quads = new List<VertexQuad>();

            //int precision = 8;

            //float halfPI = (float)(Math.PI * 0.5);
            //float oneThroughPrecision = 1.0f / precision;
            //float twoPIThroughPrecision = (float)(Math.PI * 2.0 * oneThroughPrecision);

            //float theta1, theta2, theta3;
            //Vec3 norm, pos;

            //for (uint j = 0; j < precision / 2; j++)
            //{
            //    theta1 = (j * twoPIThroughPrecision) - halfPI;
            //    theta2 = ((j + 1) * twoPIThroughPrecision) - halfPI;

            //    VertexTriangleStrip strip = new VertexTriangleStrip();
            //    strip.Points.Add();

            //    GL.Begin(BeginMode.TriangleStrip);
            //    for (uint i = 0; i <= precision; i++)
            //    {
            //        theta3 = i * twoPIThroughPrecision;

            //        norm.X = (float)(Math.Cos(theta2) * Math.Cos(theta3));
            //        norm.X = (float)Math.Sin(theta2);
            //        norm.Z = (float)(Math.Cos(theta2) * Math.Sin(theta3));
            //        pos.X = center._x + radius * norm._x;
            //        pos.Y = center._y + radius * norm._y;
            //        pos.Z = center._z + radius * norm._z;

            //        GL.Normal3(norm._x, norm._y, norm._z);
            //        GL.TexCoord2(i * oneThroughPrecision, 2.0f * (j + 1) * oneThroughPrecision);
            //        GL.Vertex3(pos._x, pos._y, pos._z);

            //        norm._x = (float)(Math.Cos(theta1) * Math.Cos(theta3));
            //        norm._y = (float)Math.Sin(theta1);
            //        norm._z = (float)(Math.Cos(theta1) * Math.Sin(theta3));
            //        pos._x = center._x + radius * norm._x;
            //        pos._y = center._y + radius * norm._y;
            //        pos._z = center._z + radius * norm._z;

            //        GL.Normal3(norm._x, norm._y, norm._z);
            //        GL.TexCoord2(i * oneThroughPrecision, 2.0f * j * oneThroughPrecision);
            //        GL.Vertex3(pos._x, pos._y, pos._z);
            //    }
            //    GL.End();
            //}

            //const int sides = 3;
            //const int heightNum = 1;
            //const int sphereHeightNum = 1;

            //float heightInc = HalfHeight / heightNum;
            //double sphereAngleInc = PI / 2.0 / sphereHeightNum;
            //double sideAngleInc = 2.0 * PI / sides;

            //for (int i = 0; i < sphereHeightNum; ++i)
            //{
            //    double zAngle = i * sphereAngleInc;
            //    double Z1 = Sin(zAngle);
            //    double Z2 = Sin(zAngle + sphereAngleInc);

            //    for (int j = 0; j < sides; ++j)
            //    {
            //        double xAngle = j * sideAngleInc;
            //        double X1 = Cos(xAngle);
            //        double X2 = Cos(xAngle + sideAngleInc);
            //    }
            //}

            //for (int i = 0; i > -sphereHeightNum; --i)
            //{
            //    double yAngle = i * sphereAngleInc;
            //    double Y1 = Sin(yAngle);
            //    double Y2 = Sin(yAngle - sphereAngleInc);

            //    for (int j = 0; j < sides; ++j)
            //    {
            //        double xAngle = j * sideAngleInc;
            //        double X1 = Cos(xAngle);
            //        double X2 = Cos(xAngle + sideAngleInc);
            //    }
            //}

            return PrimitiveData.FromQuadList(Culling.Back, new PrimitiveBufferInfo(), quads);
        }
        
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(Sphere sphere)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(Capsule capsule)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(Cone cone)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }
    }
}
