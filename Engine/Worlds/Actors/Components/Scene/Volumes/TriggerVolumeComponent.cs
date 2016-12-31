using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class TriggerVolumeComponent : BoxComponent
    {
        public TriggerVolumeComponent(Vec3 min, Vec3 max) : base(min, max, null) { }
    }
}
