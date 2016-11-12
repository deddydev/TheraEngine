using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;

namespace System
{
    public class Capsule : Shape
    {
        public float _radius, _halfHeight;
        public Vec3 _position;
        
        public float Radius { get { return _radius; } set { _radius = value; } }
        public float HalfHeight { get { return _halfHeight; } set { _halfHeight = value; } }
        public Vec3 Position { get { return _position; } set { _position = value; } }
        
        public Capsule(float radius, float halfHeight)
        {
            _radius = Abs(radius);
            _halfHeight = Abs(halfHeight);
        }
        public float GetTotalHalfHeight()
        {
            return _halfHeight + _radius;
        }
        public float GetTotalHeight()
        {
            return GetTotalHalfHeight() * 2.0f;
        }
        public void Render(float delta) { Render(delta, false); }
        public void Render(float delta, bool solid)
        {
            if (solid)
                Engine.Renderer.DrawCapsuleSolid(this);
            else
                Engine.Renderer.DrawCapsuleWireframe(this);
        }
        public override List<PrimitiveData> GetPrimitives()
        {
            PrimitiveData data = new PrimitiveData();

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
            return new List<PrimitiveData>() { data };
        }
        public override bool Contains(Vec3 point)
        {
            float totalHalfHeight = GetTotalHalfHeight();
            if (point.Z < totalHalfHeight && point.Z > -totalHalfHeight)
            {
                //Adjust Z to origin
                if (point.Z > _halfHeight)
                    point.Z -= _halfHeight;
                else if (point.Z < -_halfHeight)
                    point.Z += _halfHeight;
                return Abs(point.LengthSquared) < _radius * _radius;
            }
            return false;
        }
    }
}
