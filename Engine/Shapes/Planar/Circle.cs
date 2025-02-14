﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Core.Shapes
{
    /// <summary>
    /// Represents a circle in 3D space.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Circle3D
    {
        public Circle3D(float radius)
        {
            _plane = new Plane(0.0f, Vec3.Up);
            _radius = radius;
        }
        public Circle3D(float radius, Vec3 normal, float distance)
        {
            _plane = new Plane(distance, normal);
            _radius = radius;
        }
        public Circle3D(float radius, Vec3 point)
        {
            _plane = new Plane(point);
            _radius = radius;
        }
        public Circle3D(float radius, Vec3 point, Vec3 normal)
        {
            _plane = new Plane(point, normal);
            _radius = radius;
        }
        public Circle3D(float radius, Vec3 point0, Vec3 point1, Vec3 point2)
        {
            _plane = new Plane(point0, point1, point2);
            _radius = radius;
        }
        public Circle3D(float radius, Plane plane)
        {
            _plane = plane;
            _radius = radius;
        }

        private Plane _plane;
        private float _radius;

        [TSerialize]
        public Plane Plane
        {
            get => _plane;
            set => _plane = value;
        }
        [TSerialize]
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }

        public TMesh GetSolidMesh(int sides)
            => SolidMesh(Radius, Plane.Normal, Plane.IntersectionPoint, sides);
        public TMesh GetWireframeMesh(int sides)
            => WireframeMesh(Radius, Plane.Normal, Plane.IntersectionPoint, sides);
        public VertexLineStrip GetLineStrip(int sides)
            => LineStrip(Radius, Plane.Normal, Plane.IntersectionPoint, sides);
        
        public static TMesh SolidMesh(float radius, Vec3 normal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            normal.Normalize();
            List<TVertex> points = new List<TVertex>(Points(radius, normal, center, sides));
            points.Insert(0, new TVertex(center, normal, Vec2.Half));
            VertexTriangleFan fan = new VertexTriangleFan(points.ToArray());
            return TMesh.Create(VertexShaderDesc.PosNormTex(), fan);
        }
        public static TMesh WireframeMesh(float radius, Vec3 normal, Vec3 center, int sides)
        {
            return TMesh.Create(VertexShaderDesc.JustPositions(), LineStrip(radius, normal, center, sides));
        }
        public static VertexLineStrip LineStrip(float radius, Vec3 normal, Vec3 center, int sides)
        {
            TVertex[] points = Points(radius, normal, center, sides);
            return new VertexLineStrip(true, points);
        }
        public static TVertex[] Points(float radius, Vec3 normal, Vec3 center, int sides)
        {
            if (sides < 3)
                throw new Exception("A (very low res) circle needs at least 3 sides.");

            normal.Normalize();
            Quat offset = Quat.BetweenVectors(Vec3.Up, normal);
            TVertex[] points = new TVertex[sides];
            float angleInc = TMath.PIf * 2.0f / sides;
            float angle = 0.0f;
            for (int i = 0; i < sides; ++i, angle += angleInc)
            {
                Vec2 coord = new Vec2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vec3 v = new Vec3(coord.X, 0.0f, coord.Y);
                points[i] = new TVertex(center + offset * (radius * v), normal, coord * 0.5f + 0.5f);
            }
            return points;
        }
    }
}
