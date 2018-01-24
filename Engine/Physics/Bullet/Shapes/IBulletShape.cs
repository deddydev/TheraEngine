using BulletSharp;

namespace TheraEngine.Physics.Bullet.Shapes
{
    internal interface IBulletShape
    {
        ConvexShape Shape { get; }
    }
}
