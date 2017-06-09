using static System.Math;
using BulletSharp;
using System.Collections.Generic;

namespace System
{
    public class CapsuleY : BaseCapsule
    {
        public static List<CapsuleY> Active = new List<CapsuleY>();
        public CapsuleY() : base(Vec3.Zero, Rotator.GetZero(), Vec3.One, Vec3.Up, 1.0f, 1.0f)
        {
            ShapeIndex = Active.Count;
            Active.Add(this);
        }
        ~CapsuleY()
        {
            Active.Remove(this);
        }
        public CapsuleY(Vec3 center, Rotator rotation, Vec3 scale, float radius, float halfHeight) 
            : base(center, rotation, scale, Vec3.UnitY, radius, halfHeight)
        {
            ShapeIndex = Active.Count;
            Active.Add(this);
        }
        public override void SetTransform(Matrix4 worldMatrix)
        {
            _state.Matrix = worldMatrix;
            base.SetTransform(worldMatrix);
        }
        public override CollisionShape GetCollisionShape()
            => new CapsuleShape(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleY(_state.Translation, _state.Rotation, _state.Scale, Radius, HalfHeight);
        public override Shape TransformedBy(Matrix4 worldMatrix)
            => new CapsuleY(worldMatrix.GetPoint(), Rotator.GetZero(), Vec3.One, Radius, HalfHeight);
    }
}
