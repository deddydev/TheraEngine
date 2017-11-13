using System;
using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors
{
    public class MapStreamingVolumeComponent : BoxComponent
    {
        public MapStreamingVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, null) { }

        [TSerialize]
        public SingleFileRef<Map> MapToLoad { get; set; }

    }
}
