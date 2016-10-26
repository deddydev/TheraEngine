﻿using System.Collections.Generic;
using System.Collections;
using CustomEngine.Rendering.Models;
using System;
using CustomEngine.Worlds.Actors.Components;
using System.Collections.ObjectModel;

namespace CustomEngine.Worlds
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public abstract class Actor : ObjectBase
    {
        public Actor()
        {
            SetDefaults();
            SetupComponents();
        }
        protected virtual void SetDefaults() { }
        protected virtual void SetupComponents() { }

        [State]
        public bool IsSpawned { get { return _spawnIndex >= 0; } }
        [State]
        public World OwningWorld { get { return _owningWorld; } }

        [PostChanged("GenerateSceneComponentCache")]
        public SceneComponent RootComponent
        {
            get { return _rootSceneComponent; }
            set { _rootSceneComponent = value; }
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
        private MonitoredList<LogicComponent> _logicComponents = new MonitoredList<LogicComponent>();

        public MonitoredList<LogicComponent> LogicComponents { get { return _logicComponents; } }

        public FrameState Transform
        {
            get { return _rootSceneComponent != null ? _rootSceneComponent.Transform : FrameState.Identity; }
            set
            {
                if (_rootSceneComponent != null)
                    _rootSceneComponent.Transform = value;
            }
        }
        public void OnOriginRebased(Vec3 newOrigin)
        {
            _rootSceneComponent?.Transform.AddTranslation(-newOrigin);
            //TODO: update child transforms here or wait?
        }
        internal override void Tick(float delta)
        {
            _rootSceneComponent.Tick(delta);
            foreach (Component c in _logicComponents)
                c.Tick(delta);
        }
        public virtual void Render(float delta)
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
    }
}
