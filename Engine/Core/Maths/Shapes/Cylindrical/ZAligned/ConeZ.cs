﻿using static System.Math;
using BulletSharp;
using System.ComponentModel;

namespace System
{
    [FileClass("SHAPE", "Z-Aligned Cone")]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ConeZ : BaseCone
    {
        public ConeZ(float radius, float height) 
            : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.UnitZ, radius, height) { }
        public override CollisionShape GetCollisionShape()
            => new ConeShapeZ(Radius, Height);
    }
}
