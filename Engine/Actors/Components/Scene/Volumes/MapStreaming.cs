using System;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Worlds;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Volumes
{
    public class MapStreamingVolumeComponent : TriggerVolumeComponent
    {
        public MapStreamingVolumeComponent(Vec3 halfExtents)
            : base(halfExtents) { }

        [TSerialize]
        public GlobalFileRef<Map> MapToLoad { get; set; }

    }
}
