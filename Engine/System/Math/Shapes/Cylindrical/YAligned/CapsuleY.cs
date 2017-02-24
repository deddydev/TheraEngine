using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Files;
using System.IO;
using System.Xml;

namespace System
{
    public class CapsuleY : BaseCapsule
    {
        public CapsuleY(Vec3 center, float radius, float halfHeight) 
            : base(center, Vec3.UnitY, radius, halfHeight) { }

        public override EContainment ContainedWithin(Frustum frustum)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Sphere sphere)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(Box box)
        {
            throw new NotImplementedException();
        }
        public override EContainment ContainedWithin(BoundingBox box)
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
        public override EContainment Contains(BoundingBox box)
        {
            throw new NotImplementedException();
        }
        public override bool Contains(Vec3 point)
        {
            throw new NotImplementedException();
        }

        public override CollisionShape GetCollisionShape()
        {
            return new CapsuleShape(Radius, HalfHeight * 2.0f);
        }

        public override Shape HardCopy()
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        public override void SetTransform(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }

        public override Shape TransformedBy(Matrix4 worldMatrix)
        {
            throw new NotImplementedException();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
