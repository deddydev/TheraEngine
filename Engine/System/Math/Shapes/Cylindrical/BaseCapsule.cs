using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace System
{
    public abstract class BaseCapsule : BaseCylinder
    {
        public BaseCapsule(Vec3 center, Vec3 upAxis, float radius, float halfHeight) 
            : base(center, upAxis, radius, halfHeight) { }
        public Sphere GetTopSphere()
            => new Sphere(Radius, _upAxis * HalfHeight);
        public Sphere GetBottomSphere()
            => new Sphere(Radius, -_upAxis * HalfHeight);
    }
}
