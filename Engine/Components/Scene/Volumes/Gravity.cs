using System;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Physics;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Volumes
{
    public class GravityVolumeComponent : BoxComponent
    {
        [TSerialize]
        public Vec3 Gravity { get; set; } = new Vec3(0.0f, -9.81f, 0.0f);

        public GravityVolumeComponent() : this(Vec3.Half) { }
        public GravityVolumeComponent(Vec3 halfExtents) : base(halfExtents, null) { }

        public void OnOverlapEntered(TRigidBody obj)
        {
            obj.Gravity = Gravity;
        }
        public void OnOverlapLeft(TRigidBody obj)
        {
            obj.Gravity = Engine.World.Settings.Gravity;
        }
    }
}