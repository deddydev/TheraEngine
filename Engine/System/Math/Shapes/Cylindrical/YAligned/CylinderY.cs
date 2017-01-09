using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Worlds.Actors.Components;

namespace System
{
    public class CylinderY : Shape
    {
        public float _radius, _height;
        
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }
        
        public CylinderY(Vec3 center, float radius, float height)
        {
            _radius = Abs(radius);
            _height = Abs(height);
        }
        public override CollisionShape GetCollisionShape()
        {
            return new ConeShape(Radius, Height);
        }
        public override void Render()
        {
            //Engine.Renderer.RenderCylinder(this, _renderSolid);
        }
        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment Contains(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
        public override void SetTransform(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }

        public override Shape HardCopy()
        {
            throw new NotImplementedException();
        }
    }
}
