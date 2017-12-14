using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering;

namespace TheraEngine.Physics
{
    public abstract class TSoftBody : TCollisionObject
    {
        public TSoftBody(TCollisionShape shape) : base(shape)
        {
            
        }

        public static TSoftBody New(TSoftBodyConstructionInfo info)
            => Engine.Physics.NewSoftBody(info);

        public abstract Vec3 WindVelocity { get; set; }
        public abstract float Volume { get; }
        public abstract float TotalMass { get; set; }
        public abstract Matrix4 InitialWorldTransform { get; set; }
    }
}
