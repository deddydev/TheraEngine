using TheraEngine.Actors;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Volumes
{
    public delegate void DelOnOverlapEnter(IActor actor);
    public delegate void DelOnOverlapLeave(IActor actor);
    public class TriggerVolumeComponent : BoxComponent
    {
        public DelOnOverlapEnter OnEntered;
        public DelOnOverlapLeave OnLeft;

        public TriggerVolumeComponent()
            : base(null) { }
        public TriggerVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, null) { }
    }
}
