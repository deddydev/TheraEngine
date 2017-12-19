using BulletSharp;
using System;

namespace TheraEngine.Physics.Bullet
{
    internal interface IBulletCollisionObject
    {
        CollisionObject CollisionObject { get; }
        void OnTransformChanged(Matrix4 worldTransform);
    }
}
