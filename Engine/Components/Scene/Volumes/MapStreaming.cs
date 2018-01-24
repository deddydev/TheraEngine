using System;
using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Worlds;

namespace TheraEngine.Components.Scene.Volumes
{
    public class MapStreamingVolumeComponent : BoxComponent
    {
        public MapStreamingVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, null) { }

        [TSerialize]
        public GlobalFileRef<Map> MapToLoad { get; set; }

    }
}
