using System.Collections.Generic;
using System.Collections;
using eyecm.PhysX;
using System.Threading.Tasks;

namespace CustomEngine.World
{
    public abstract class WorldBase : IRenderable, IEnumerable<Actor>
    {
        protected List<Map> _spawnedMaps = new List<Map>();
        protected List<Actor> _spawnedActors;

        protected List<Map> _allMaps;

        private Scene _physicsScene;
        private WorldDefaults _defaults;
        private WorldSettings _settings;
        private string _name;
        private bool _visible;

        public int ActorCount { get { return _spawnedActors.Count; } }

        public WorldBase(string name)
        {
            _name = name;
        }

        public void SpawnActor(Actor actor)
        {
            if (!_spawnedActors.Contains(actor))
                _spawnedActors.Add(actor);
            actor.OnSpawned(this);
        }
        public void DespawnActor(Actor actor)
        {
            if (_spawnedActors.Contains(actor))
                _spawnedActors.Remove(actor);
            actor.OnDespawned();
        }
        public void Update()
        {
            foreach (Actor actor in _spawnedActors)
                actor.Update();
        }
        public void Render()
        {
            foreach (Actor actor in _spawnedActors)
                if (actor.IsSpawned)
                    actor.Render();
        }
        public static async Task Unload()
        {
            
        }
        public static async Task Load()
        {
            
        }
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible == value)
                    return;

                if (_visible = value)
                {
//                     foreach (Map m in _allMaps)
//                         m.Visible = m.
                }
                else
                {
                    foreach (Actor a in _spawnedActors)
                        DespawnActor(a);
                }
            }
        }

        public void SerializeState(string path)
        {
            
        }
        public void ExportMapFile(string path)
        {

        }
        public Actor this[int index]
        {
            get { return _spawnedActors[index]; }
            set { _spawnedActors[index] = value; }
        }
        public IEnumerator<Actor> GetEnumerator() { return ((IEnumerable<Actor>)_spawnedActors).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Actor>)_spawnedActors).GetEnumerator(); }
    }
}
