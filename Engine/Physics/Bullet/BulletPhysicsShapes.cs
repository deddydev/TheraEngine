using System;
using BulletSharp;

namespace TheraEngine.Physics.Bullet
{
    public interface IBulletShape
    {
        CollisionShape CollisionShape { get; }
    }
    internal class BulletSphere : TCollisionSphere, IBulletShape
    {
        public SphereShape Sphere { get; }

        public override float Radius => Sphere.Radius;

        CollisionShape IBulletShape.CollisionShape => Sphere;

        public BulletSphere()
            => Sphere = new SphereShape(0.5f);
        public BulletSphere(float radius)
            => Sphere = new SphereShape(radius);
    }
    internal class BulletBox : TCollisionBox, IBulletShape
    {
        public BoxShape Box { get; }
        
        public override Vec3 HalfExtents => Box.HalfExtentsWithoutMargin;

        CollisionShape IBulletShape.CollisionShape => Box;

        public BulletBox() 
            => Box = new BoxShape(0.5f);
        public BulletBox(Vec3 halfExtents)
            => Box = new BoxShape(halfExtents);
    }
}
