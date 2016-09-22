﻿using CustomEngine.Components;
using System.Collections.Generic;
using System.Collections;
using CustomEngine.Rendering.Models;

namespace CustomEngine.World
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public class Actor : IEnumerable<Component>
    {
        public Actor() { SetupComponents(); }

        public bool IsSpawned { get { return _spawnIndex >= 0; } }
        public WorldBase OwningWorld { get { return _owningWorld; } }

        public List<Component> _components = new List<Component>();
        public int _spawnIndex = -1;
        private WorldBase _owningWorld;
        private FrameState _transform;

        public void OnOriginRebased()
        {

        }

        protected virtual void SetupComponents() { }

        public virtual void UpdateTick(double deltaTime)
        {
            
        }
        public virtual void RenderTick(double deltaTime)
        {
            foreach (Component comp in _components)
                comp.RenderTick(deltaTime);
        }

        public void Despawn()
        {
            if (IsSpawned && OwningWorld != null)
                OwningWorld.DespawnActor(this);
        }
        public virtual void OnSpawned(WorldBase world)
        {
            if (_components == null && IsSpawned)
                return;

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.ActorCount - 1;
            _owningWorld = world;

            foreach (Component comp in _components)
                comp.OnSpawned();
        }
        public virtual void OnDespawned()
        {
            if (_components == null || !IsSpawned)
                return;

            foreach (Component comp in _components)
                comp.OnDespawned();
            
            _spawnIndex = -1;
            _owningWorld = null;
        }

        public IEnumerator<Component> GetEnumerator() { return ((IEnumerable<Component>)_components).GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable<Component>)_components).GetEnumerator(); }
    }
}
