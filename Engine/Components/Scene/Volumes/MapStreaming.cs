using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
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

        protected override async void OnEntered(TCollisionObject obj)
        {
            base.OnEntered(obj);

            //TODO: check if obj is linked to pawn, and if player is allowed to trigger map streaming
            var map = await MapToLoad.GetInstanceAsync();
            OwningWorld.SpawnMap(map);
        }
        protected override void OnLeft(TCollisionObject obj)
        {
            base.OnLeft(obj);

            if (MapToLoad.IsLoaded)
                OwningWorld.DespawnMap(MapToLoad.File);
        }
    }
}
