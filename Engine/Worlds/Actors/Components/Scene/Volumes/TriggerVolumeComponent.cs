using System;

namespace CustomEngine.Worlds.Actors
{
    public delegate void DelOnOverlapEnter(IActor actor);
    public delegate void DelOnOverlapLeave(IActor actor);
    public class TriggerVolumeComponent : BoxComponent
    {
        public DelOnOverlapEnter OnEntered;
        public DelOnOverlapLeave OnLeft;
        public TriggerVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, null) { }
    }
}
