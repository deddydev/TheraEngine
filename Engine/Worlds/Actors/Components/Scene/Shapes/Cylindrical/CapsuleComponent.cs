using BulletSharp;
using TheraEngine.Rendering;
using System;
using System.Drawing;

namespace TheraEngine.Worlds.Actors
{
    public class CapsuleComponent : ShapeComponent
    {
        BaseCapsule _capsule;

        public CapsuleComponent(float radius, float halfHeight, PhysicsConstructionInfo info) : base()
        {
            _capsule = new CapsuleY(Vec3.Zero, Rotator.GetZero(), Vec3.One, radius, halfHeight);
            InitPhysics(info);
        }
        public override Shape CullingVolume => _capsule;
        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _capsule.SetTransform(WorldMatrix);
            RenderNode?.ItemMoved(this);
        }

        public override void Render()
            => Engine.Renderer.RenderCapsule(_capsule.ShapeName, WorldMatrix, _capsule.LocalUpAxis, _capsule.Radius, _capsule.HalfHeight, _capsule.RenderSolid, Color.Red);

        protected override CollisionShape GetCollisionShape()
            => _capsule.GetCollisionShape();
        public override void OnSpawned()
        {
            Engine.Renderer.Scene.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            Engine.Renderer.Scene.Remove(this);
            base.OnDespawned();
        }
    }
}