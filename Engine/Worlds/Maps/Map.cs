using TheraEngine.Worlds.Maps;
using TheraEngine.Files;
using TheraEngine.Actors;
using System.ComponentModel;

namespace TheraEngine.Worlds
{
    public class Map : TFileObject
    {
        public Map() :this(new MapSettings()) { }
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
            OwningWorld = null;
        }
        public virtual void BeginPlay(World world)
        {
            OwningWorld = world;
            foreach (IActor actor in Settings.StaticActors)
                OwningWorld.SpawnActor(actor, Settings.SpawnPosition);
        }
    }
}
