using TheraEngine.Worlds.Maps;
using TheraEngine.Files;
using TheraEngine.Worlds.Actors;
using System.ComponentModel;

namespace TheraEngine.Worlds
{
    public class Map : FileObject
    {
        public Map(MapSettings settings)
        {
            _settings = settings;
        }

        public World OwningWorld { get; private set; }

        private MapSettings _settings;

        [TSerialize]
        public MapSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }

        public virtual void EndPlay()
        {
            foreach (IActor actor in Settings.StaticActors)
                OwningWorld.DespawnActor(actor);
            //TODO: determine what dynamic actors lie within this map's bounds
            //and ONLY despawn those actors if this 
        }
        public virtual void BeginPlay(World world)
        {
            OwningWorld = world;
            foreach (IActor actor in Settings.StaticActors)
                OwningWorld.SpawnActor(actor, Settings.SpawnPosition);
        }
    }
}
