using System.Collections.Generic;
using System.Collections;
using eyecm.PhysX;

namespace CustomEngine.World
{
    public abstract class WorldBase : IRenderable, IEnumerable<Actor>
    {
        protected List<Map> _spawnedMaps = new List<Map>();
        private List<Actor> _actors;

        private Scene _physicsScene;
        private WorldDefaults _defaults;
        private WorldSettings _settings;
        private string _name;

        public int ActorCount { get { return _actors.Count; } }

        public WorldBase(string name)
        {
            _name = name;
        }

        public void SpawnActor(Actor actor)
        {
            if (!_actors.Contains(actor))
                _actors.Add(actor);
            actor.OnSpawned(this);
        }
        public void DespawnActor(Actor actor)
        {
            if (_actors.Contains(actor))
                _actors.Remove(actor);
            actor.OnDespawned();
        }
        public void Update()
        {
            foreach (Actor actor in _actors)
                actor.Update();
        }
        public void Render()
        {
            foreach (Actor actor in _actors)
                if (actor.IsSpawned)
                    actor.Render();
        }
        public void OnUnload()
        {
            foreach (Actor a in _actors)
                a.Despawn();
        }
        public void OnLoad()
        {
            foreach (Actor a in _actors)
                a.OnSpawned(this);
        }
        public void SerializeState(string path)
        {
            
        }
        public void ExportMapFile(string path)
        {

        }
        public Actor this[int index]
        {
            get { return _actors[index]; }
            set { _actors[index] = value; }
        }
        public IEnumerator<Actor> GetEnumerator() { return ((IEnumerable<Actor>)_actors).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Actor>)_actors).GetEnumerator(); }
    }
}
