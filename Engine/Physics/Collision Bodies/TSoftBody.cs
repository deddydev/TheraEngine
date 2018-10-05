using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    public abstract class TSoftBody : TCollisionObject
    {
        public TSoftBody() : base() { }

        public new ISoftBodyCollidable Owner
        {
            get => (ISoftBodyCollidable)base.Owner;
            set => base.Owner = value;
        }

        public static TSoftBody New(TSoftBodyConstructionInfo info)
            => Engine.Physics.NewSoftBody(info);

        public abstract Vec3 WindVelocity { get; set; }
        public abstract float Volume { get; }
        public abstract float TotalMass { get; set; }
        public abstract Matrix4 InitialWorldTransform { get; set; }
    }
}
