using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Worlds.Actors.Components;

namespace System
{
    public class CylinderY : BoundingShape
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
        
        public CylinderY(Vec3 center, float radius, float height) : base(center)
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

        public override EContainment Contains(IBoundingBox box)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(IBox box)
        {
            throw new NotImplementedException();
        }

        public override EContainment Contains(ISphere sphere)
        {
            throw new NotImplementedException();
        }

        public override EContainment ContainedWithin(IBoundingBox box)
        {
            throw new NotImplementedException();
        }

        public override EContainment ContainedWithin(IBox box)
        {
            throw new NotImplementedException();
        }

        public override EContainment ContainedWithin(ISphere sphere)
        {
            throw new NotImplementedException();
        }

        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
    }
}
