using TheraEngine.Physics;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Volumes
{
    public class GravityVolumeComponent : TriggerVolumeComponent
    {
        [TSerialize]
        public Vec3 Gravity { get; set; } = new Vec3(0.0f, -9.81f, 0.0f);

        public GravityVolumeComponent() : this(Vec3.Half) { }
        public GravityVolumeComponent(Vec3 halfExtents) : base(halfExtents) { }

        protected override void OnEntered(TCollisionObject obj)
        {
            if (obj is TRigidBody rb)
                rb.Gravity = Gravity;

            base.OnEntered(obj);
        }
        protected override void OnLeft(TCollisionObject obj)
        {
            if (obj is TRigidBody rb)
                rb.Gravity = Engine.World.Settings.Gravity;

            base.OnLeft(obj);
        }
    }
}