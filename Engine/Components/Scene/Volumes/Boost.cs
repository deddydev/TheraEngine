using System;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Volumes
{
    public class BoostVolumeComponent : TriggerVolumeComponent
    {
        public BoostVolumeComponent()
            : base() { }
        public BoostVolumeComponent(Vec3 halfExtents)
            : base(halfExtents) { }
    }
}
