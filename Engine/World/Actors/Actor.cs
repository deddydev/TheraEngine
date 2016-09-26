using CustomEngine.Components;
using System.Collections.Generic;
using System.Collections;
using CustomEngine.Rendering.Models;
using OpenTK;
using System;

namespace CustomEngine.World
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public class Actor : IEnumerable<Component>
    {
        public Actor()
        {
            _instanceComponents = new List<Component>();
            
            SetupComponents();
        }

        public bool IsSpawned { get { return _spawnIndex >= 0; } }
        public WorldBase OwningWorld { get { return _owningWorld; } }

        public DateTime _lastRendered;
        public int _spawnIndex = -1;
        private WorldBase _owningWorld;
        private SceneComponent _rootSceneComponent;
        private List<Component> _instanceComponents;

        public FrameState Transform
        {
            get { return _rootSceneComponent != null ? _rootSceneComponent.Transform : FrameState.Identity; }
            set
            {
                if (_rootSceneComponent != null)
                    _rootSceneComponent.Transform = value;
            }
        }

        public void OnOriginRebased(Vector3 delta)
        {
            _rootSceneComponent?.Transform.Translate(delta);
        }

        protected virtual void SetupComponents() { }

        public void AddComponent(Component c)
        {

        }

        public virtual void Update()
        {
            _rootSceneComponent.Update();
            foreach (Component c in _instanceComponents)
                c.Update();
        }
        public virtual void Render()
        {
            _lastRendered = DateTime.Now;
            _rootSceneComponent?.Render();
        }

        public void Despawn()
        {
            if (IsSpawned && OwningWorld != null)
                OwningWorld.DespawnActor(this);
        }
        public virtual void OnSpawned(WorldBase world)
        {
            if (IsSpawned)
                return;

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.ActorCount - 1;
            _owningWorld = world;

            _rootSceneComponent.OnSpawned();
            foreach (Component comp in _instanceComponents)
                comp.OnSpawned();
        }
        public virtual void OnDespawned()
        {
            if (!IsSpawned)
                return;

            foreach (Component comp in _instanceComponents)
                comp.OnDespawned();
            _rootSceneComponent.OnDespawned();
            
            _spawnIndex = -1;
            _owningWorld = null;
        }

        public IEnumerator<Component> GetEnumerator() { return ((IEnumerable<Component>)_instanceComponents).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Component>)_instanceComponents).GetEnumerator(); }
    }
}
