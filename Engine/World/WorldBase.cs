using System.Collections.Generic;
using System.Collections;
using System;

namespace CustomEngine.World
{
    public abstract class WorldBase : IEnumerable<Actor>
    {
        public int ActorCount { get { return _actors.Count; } }
        protected List<Actor> _actors = new List<Actor>();

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

        public void Tick(double deltaTime)
        {
            foreach (Actor actor in _actors)
            {
                actor.Tick(deltaTime);
            }
        }

        public void RenderTick(double deltaTime)
        {
            foreach (Actor actor in _actors)
            {
                actor.RenderTick(deltaTime);
            }
        }

        public void OnUnload()
        {
            foreach (Actor a in _actors)
            {
                a.Despawn();
            }
        }

        public void OnLoad()
        {
            foreach (Actor a in _actors)
            {
                a.OnSpawned(this);
            }
        }

        public Actor this[int index]
        {
            get
            {
                if (index >= 0 && index < _actors.Count)
                    return _actors[index];
                //return null;
                throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < _actors.Count)
                    _actors[index] = value;
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public IEnumerator<Actor> GetEnumerator() { return ((IEnumerable<Actor>)_actors).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Actor>)_actors).GetEnumerator(); }
    }
}
