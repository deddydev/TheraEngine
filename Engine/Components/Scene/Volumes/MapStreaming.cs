using System;
using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Worlds.Actors.Components.Scene.Shapes;

namespace TheraEngine.Worlds.Actors.Components.Scene.Volumes
{
    public class MapStreamingVolumeComponent : BoxComponent
    {
        public MapStreamingVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, null) { }

        [TSerialize]
        public GlobalFileRef<Map> MapToLoad { get; set; }

    }
}
