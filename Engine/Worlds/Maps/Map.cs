using CustomEngine.Worlds.Maps;
using CustomEngine.Files;

namespace CustomEngine.Worlds
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
        public MapSettings Settings { get { return _settings; } set { _settings = value; } }

        enum enum1
        {
            thing1,
            thing2
        }
        enum enum2
        {
            thing3 = enum1.thing1,
        }

        public virtual void EndPlay()
        {
            foreach (Actor actor in Settings._defaultActors)
                _owningWorld.DespawnActor(actor);
        }
        public virtual void BeginPlay()
        {
            foreach (Actor actor in Settings._defaultActors)
                _owningWorld.SpawnActor(actor, Settings.SpawnPosition);
        }
    }
}
