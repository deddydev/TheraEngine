using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Volumes
{
    public class BoostVolumeComponent : TriggerVolumeComponent
    {
        public Vec3 Force { get; set; }

        public BoostVolumeComponent()
            : base() { }
        public BoostVolumeComponent(Vec3 halfExtents)
            : base(halfExtents) { }
    }
}
