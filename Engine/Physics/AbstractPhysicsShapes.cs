using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Physics.Bullet;

namespace TheraEngine.Physics
{
    public abstract class TCollisionShape
    {

    }
    public abstract class TCollisionSphere : TCollisionShape
    {
        public abstract float Radius { get; }

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
