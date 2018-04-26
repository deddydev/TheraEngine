using System;
using System.Drawing;
using TheraEngine.Core.Shapes;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene.Shapes
{
    public class CapsuleYComponent : ShapeComponent
    {
        BaseCapsule _capsule;

        public CapsuleYComponent(float radius, float halfHeight, TRigidBodyConstructionInfo info) : base()
        {
            _capsule = new CapsuleY(Vec3.Zero, Rotator.GetZero(), Vec3.One, radius, halfHeight);
            InitPhysicsShape(info);
        }

        [Browsable(false)]
        public override Shape CullingVolume => _capsule;

        [Category("Y-Aligned Capsule")]
        public float Radius
        {
            get => _capsule.Radius;
            set => _capsule.Radius = value;
        }
        [Category("Y-Aligned Capsule")]
        public float HalfHeight
        {
            get => _capsule.HalfHeight;
            set => _capsule.HalfHeight = value;
        }

        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            _capsule.SetRenderTransform(WorldMatrix);
            OctreeNode?.ItemMoved(this);
        }

        public override void Render()
            => Engine.Renderer.RenderCapsule(WorldMatrix, _capsule.LocalUpAxis, _capsule.Radius, _capsule.HalfHeight, _capsule.RenderSolid, Color.Red);

        protected override TCollisionShape GetCollisionShape()
            => _capsule.GetCollisionShape();
        public override void OnSpawned()
        {
            //Engine.Scene.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            //Engine.Scene.Remove(this);
            base.OnDespawned();
        }

        public override void AddRenderables(RenderPasses passes, Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}