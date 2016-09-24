using System.Collections.Generic;
using System.Collections;
using System;
using System;
using CustomEngine.Input;

namespace CustomEngine.World
{
    public abstract class WorldBase : IEnumerable<Actor>
    {
        public int ActorCount { get { return _actors.Count; } }
        protected List<Actor> _actors = new List<Actor>();
        protected List<Map> _allMaps = new List<Map>();
        protected List<Map> _spawnedMaps = new List<Map>();
        
        private WorldSettings _settings;
        private string _name;

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
        public void UpdateTick(double deltaTime)
        {
            foreach (Actor actor in _actors)
                actor.Update();
        }
        public void RenderTick(double deltaTime)
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
