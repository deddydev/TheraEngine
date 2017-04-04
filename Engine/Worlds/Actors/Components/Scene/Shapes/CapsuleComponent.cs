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
            _capsule = new CapsuleY(Vec3.Zero, radius, halfHeight);
            InitPhysics(info);
        }
        public override Shape CullingVolume => _capsule;
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _capsule.Center = GetWorldPoint();
        }

        public override void Render()
            => _capsule.Render();
        protected override CollisionShape GetCollisionShape()
            => _capsule.GetCollisionShape();
    }
}