using static System.Math;
using TheraEngine;
using TheraEngine.Rendering.Models;
using System.Collections.Generic;
using BulletSharp;
using TheraEngine.Worlds.Actors;
using TheraEngine.Files;
using System.IO;
using System.Xml;

namespace System
{
    public class ConeY : Shape
    {
        public float _radius, _height;
        
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }
        public float Height
        {
            get => _height;
            set => _height = value;
        }
        
        public ConeY(Vec3 center, float radius, float height)
        {
            _radius = Abs(radius);
            _height = Abs(height);
        }

        public override CollisionShape GetCollisionShape()
            => new ConeShape(Radius, Height);
        
        public override void Render()
        {
            //Engine.Renderer.RenderCone(this, _renderSolid);
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

        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }
    }
}
