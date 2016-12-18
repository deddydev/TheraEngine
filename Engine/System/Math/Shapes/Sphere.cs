﻿using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using BulletSharp;

namespace System
{
    public class Sphere : Shape
    {
        private Vec3 _center = Vec3.Zero;
        private float _radius;

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public Vec3 Center
        {
            get { return WorldMatrix.TransformPosition(_center); }
            set { _center = InverseWorldMatrix.TransformPosition(value); }
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
            throw new NotImplementedException();
        }
    }
}
