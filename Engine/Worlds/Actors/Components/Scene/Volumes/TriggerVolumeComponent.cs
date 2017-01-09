using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class TriggerVolumeComponent : BoxComponent
    {
        public TriggerVolumeComponent(Vec3 extents) : base(extents, null) { }
    }
}
