using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Worlds;

namespace TheraEngine.Components.Scene.Volumes
{
    public class MapStreamingVolumeComponent : TriggerVolumeComponent
    {
        public MapStreamingVolumeComponent() : this(Vec3.Zero) { }
        public MapStreamingVolumeComponent(Vec3 halfExtents)
            : base(halfExtents) { }

        [TSerialize]
        public GlobalFileRef<Map> MapToLoad { get; set; }

    }
}
