using System;

namespace TheraEngine.Physics
{
    public abstract class TSoftBody : TCollisionObject
    {
        public TSoftBody(ISoftCollidable owner, TCollisionShape shape) : base(owner, shape)
        {
            
        }

        public new ISoftCollidable Owner
        {
            get => (ISoftCollidable)base.Owner;
            set => base.Owner = value;
        }

        public static TSoftBody New(ISoftCollidable owner, TSoftBodyConstructionInfo info)
            => Engine.Physics.NewSoftBody(owner, info);

        public abstract Vec3 WindVelocity { get; set; }
        public abstract float Volume { get; }
        public abstract float TotalMass { get; set; }
        public abstract Matrix4 InitialWorldTransform { get; set; }
    }
}
