using System.Collections.Generic;
using System.Collections;
using CustomEngine.Rendering.Models;
using System;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Worlds
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public abstract class Actor : IEnumerable<Component>
    {
        public Actor()
        {
            SetupComponents();
        }

        public bool IsSpawned { get { return _spawnIndex >= 0; } }
        public World OwningWorld { get { return _owningWorld; } }

        public SceneComponent RootComponent
        {
            get { return _rootSceneComponent; }
            set
            {
                _rootSceneComponent = value;
                GenerateSceneComponentCache();
            }
        }
        public void GenerateSceneComponentCache()
        {
            _sceneComponentCache = _rootSceneComponent.GenerateChildCache();
        }

        public DateTime _lastRendered;
        public int _spawnIndex = -1;
        private World _owningWorld;
        private SceneComponent _rootSceneComponent;
        private List<SceneComponent> _sceneComponentCache = new List<SceneComponent>();
        private List<InstanceComponent> _instanceComponents = new List<InstanceComponent>();

        public FrameState Transform
        {
            get { return _rootSceneComponent != null ? _rootSceneComponent.Transform : FrameState.Identity; }
            set
            {
                if (_rootSceneComponent != null)
                    _rootSceneComponent.Transform = value;
            }
        }
        protected abstract void SetupComponents();
        public void OnOriginRebased(Vector3 delta)
        {
            _rootSceneComponent?.Transform.AddTranslation(delta);
        }
        public void AddComponent(InstanceComponent c)
        {
            _instanceComponents.Add(c);
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
        public virtual void OnSpawned(World world)
        {
            if (IsSpawned)
                return;

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.ActorCount - 1;
            _owningWorld = world;

            _rootSceneComponent.OnSpawned();
            //foreach (InstanceComponent comp in _instanceComponents)
            //    comp.OnSpawned();
        }
        public virtual void OnDespawned()
        {
            if (!IsSpawned)
                return;

            //foreach (InstanceComponent comp in _instanceComponents)
            //    comp.OnDespawned();
            _rootSceneComponent.OnDespawned();
            
            _spawnIndex = -1;
            _owningWorld = null;
        }

        public IEnumerator<Component> GetEnumerator() { return ((IEnumerable<Component>)_instanceComponents).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Component>)_instanceComponents).GetEnumerator(); }
    }
}
