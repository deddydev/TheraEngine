using BulletSharp;

namespace TheraEngine.Physics.Bullet
{
    public interface IBulletConstraint
    {
        TypedConstraint Constraint { get; }
    }
}
