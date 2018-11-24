using TheraEngine.Worlds.Maps;
using TheraEngine.Core.Files;
using TheraEngine.Actors;
using System.ComponentModel;

namespace TheraEngine.Worlds
{
    [TFileExt("map")]
    [TFileDef("Map")]
    public class Map : TFileObject
    {
        public Map() :this(new MapSettings()) { }
        public Map(MapSettings settings)
        {
            _settings = settings;
        }

        public TWorld OwningWorld { get; private set; }

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
        public virtual void BeginPlay(TWorld world)
        {
            OwningWorld = world;
            foreach (IActor actor in Settings.StaticActors)
                OwningWorld.SpawnActor(actor, Settings.SpawnPosition);
        }
    }
}
