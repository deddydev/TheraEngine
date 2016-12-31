using static System.Math;
using CustomEngine;
using CustomEngine.Rendering.Models;
using System.Collections.Generic;
using BulletSharp;
using CustomEngine.Worlds.Actors.Components;

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

        public override EContainment ContainedWithin(ISphere sphere)
        {
            throw new NotImplementedException();
        }

        public override EContainment ContainedWithin(IBox box)
        {
            throw new NotImplementedException();
        }

        public override EContainment ContainedWithin(IBoundingBox box)
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

        public override EContainment Contains(IBoundingBox box)
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
    }
}
