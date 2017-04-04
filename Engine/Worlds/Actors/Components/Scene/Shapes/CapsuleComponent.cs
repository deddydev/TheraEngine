using BulletSharp;
using CustomEngine.Rendering;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CapsuleComponent : ShapeComponent
    {
        BaseCapsule _capsule;

        public CapsuleComponent(float radius, float halfHeight, PhysicsDriverInfo info) : base()
        {
            _capsule = new CapsuleY(Vec3.Zero, radius, halfHeight);
            InitPhysics(info);
        }
        public override Shape CullingVolume => _capsule;
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _capsule.SetTransform(WorldMatrix);
        }

        public override void Render()
            => _capsule.Render();
        protected override CollisionShape GetCollisionShape()
            => _capsule.GetCollisionShape();
        public override void OnSpawned()
        {
            Engine.Renderer.Scene.AddRenderable(_capsule);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            Engine.Renderer.Scene.RemoveRenderable(_capsule);
            base.OnDespawned();
        }
    }
}