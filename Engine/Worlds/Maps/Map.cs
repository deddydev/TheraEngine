using CustomEngine.Worlds.Maps;
using CustomEngine.Files;
using System;
using System.IO;
using System.Xml;

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

        public MapSettings Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        public virtual void EndPlay()
        {
            foreach (IActor actor in Settings._defaultActors)
                _owningWorld.DespawnActor(actor);
        }
        public virtual void BeginPlay()
        {
            foreach (IActor actor in Settings._defaultActors)
                _owningWorld.SpawnActor(actor, Matrix4.Identity);
        }
    }
}
