using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Shapes;
using TheraEngine.Physics.Bullet;

namespace TheraEngine.Physics
{
    public abstract class TCollisionShape
    {
        public abstract float Margin { get; set; }
        public abstract Vec3 LocalScaling { get; set; }

        public Sphere GetBoundingSphere()
        {
            GetBoundingSphere(out Vec3 center, out float radius);
            return new Sphere(radius, center);
        }
        public abstract void GetBoundingSphere(out Vec3 center, out float radius);
        public BoundingBox GetAabb(Matrix4 transform)
        {
            GetAabb(transform, out Vec3 aabbMin, out Vec3 aabbMax);
            return BoundingBox.FromMinMax(aabbMin, aabbMax);
        }
        public abstract void GetAabb(Matrix4 transform, out Vec3 aabbMin, out Vec3 aabbMax);
        public abstract Vec3 CalculateLocalInertia(float mass);
    }
    public abstract class TCollisionSphere : TCollisionShape
    {
        public abstract float Radius { get; set; }

        public static TCollisionSphere New(float radius)
            => Engine.Physics.NewSphere(radius);
    }
    public abstract class TCollisionBox : TCollisionShape
    {
        public abstract Vec3 HalfExtents { get; }

        public static TCollisionBox New(float halfExtentsX, float halfExtentsY, float halfExtentsZ)
            => New(new Vec3(halfExtentsX, halfExtentsY, halfExtentsZ));
        public static TCollisionBox New(Vec3 halfExtents)
            => Engine.Physics.NewBox(halfExtents);
    }
}
