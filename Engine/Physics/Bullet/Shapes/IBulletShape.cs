using BulletSharp;

namespace TheraEngine.Physics.Bullet.Shapes
{
    internal interface IBulletShape
    {
        CollisionShape Shape { get; }
    }
}
