using static System.Math;
using BulletSharp;
using CustomEngine.Files;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace System
{
    public class CapsuleY : BaseCapsule
    {
        public CapsuleY(Vec3 center, float radius, float halfHeight) 
            : base(center, Vec3.UnitY, radius, halfHeight) { }
        
        public override void SetTransform(Matrix4 worldMatrix)
            => Center = worldMatrix.GetPoint();
        public override CollisionShape GetCollisionShape()
            => new CapsuleShape(Radius, HalfHeight * 2.0f);
        public override Shape HardCopy()
            => new CapsuleY(Center, Radius, HalfHeight);
        public override Shape TransformedBy(Matrix4 worldMatrix)
            => new CapsuleY(worldMatrix.GetPoint(), Radius, HalfHeight);

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct Header
        //{
        //    public const int Size = 0x14;

        //    public BVec3 _center;
        //    public float _radius;
        //    public float _halfHeight;

        //    public static implicit operator Header(CapsuleY c)
        //    {
        //        return new Header()
        //        {
        //            _radius = c._radius,
        //            _center = c._center,
        //            _halfHeight = c._halfHeight,
        //        };
        //    }
        //    public static implicit operator CapsuleY(Header h)
        //    {
        //        return new CapsuleY(h._center, h._radius, h._halfHeight);
        //    }
        //}
    }
}
