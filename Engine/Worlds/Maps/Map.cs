using TheraEngine.Worlds.Maps;
using TheraEngine.Files;

namespace TheraEngine.Worlds
{
    public class Map : FileObject
    {
        public Map(World owner, MapSettings settings)
        {
            _settings = settings;
            _owningWorld = owner;
        }

        private World _owningWorld;
        private MapSettings _settings;

        public MapSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public virtual void EndPlay()
        {
            foreach (IActor actor in Settings.DefaultActors)
                _owningWorld.DespawnActor(actor);
        }
        public virtual void BeginPlay()
        {
            foreach (IActor actor in Settings.DefaultActors)
                _owningWorld.SpawnActor(actor);
        }
    }
}
