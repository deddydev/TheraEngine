using BulletSharp;
using CustomEngine.Rendering;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CapsuleComponent : ShapeComponent
    {
        CapsuleY _capsule;
        public CapsuleComponent(float radius, float halfHeight, PhysicsDriverInfo info) : base()
        {
            _capsule = new CapsuleY(GetWorldPoint(), radius, halfHeight);
            InitPhysics(info);
        }

        public override Shape CullingVolume => _capsule;

        public override void Render()
        {
            throw new NotImplementedException();
        }
        protected override CollisionShape GetCollisionShape()
        {
            return _capsule.GetCollisionShape();
        }
    }
}
