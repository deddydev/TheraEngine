﻿using System.Collections.Generic;
using System.Collections;
using CustomEngine.Rendering.Models;
using System;
using CustomEngine.Worlds.Actors.Components;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using CustomEngine.Files;
using System.IO;
using System.Xml;

namespace CustomEngine.Worlds
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public interface IActor
    {
        bool IsConstructing { get; }
        World OwningWorld { get; }
        bool IsSpawned { get; }
        void OnSpawned(World world);
        void OnDespawned();
        void GenerateSceneComponentCache();
        SceneComponent RootComponent { get; }
        void RebaseOrigin(Vec3 newOrigin);
    }
    public class Actor : Actor<SceneComponent>
    {
        public Actor() : base() { }
        public Actor(SceneComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public unsafe struct Header
        {
            public const int Size = 8;

            public bint _nameOffset;
            public bint _logicCompCount;
            public bint _sceneCompCount;

            public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
        }
    }
    public class Actor<T> : FileObject, IActor where T : SceneComponent
    {
        public Actor(bool deferInitialization = false)
        {
            _logicComponents = new MonitoredList<LogicComponent>();
            _logicComponents.Added += _logicComponents_Added;
            _logicComponents.AddedRange += _logicComponents_AddedRange;
            _logicComponents.Removed += _logicComponents_Removed;
            _logicComponents.RemovedRange += _logicComponents_RemovedRange;
            _logicComponents.Inserted += _logicComponents_Inserted;
            _logicComponents.InsertedRange += _logicComponents_InsertedRange;
            if (!deferInitialization)
                Initialize();
        }
        public Actor(T root, params LogicComponent[] logicComponents)
        {
            _isConstructing = true;
            PreConstruct();
            RootComponent = root;
            _logicComponents = new MonitoredList<LogicComponent>(logicComponents.ToList());
            _logicComponents.Added += _logicComponents_Added;
            _logicComponents.AddedRange += _logicComponents_AddedRange;
            _logicComponents.Removed += _logicComponents_Removed;
            _logicComponents.RemovedRange += _logicComponents_RemovedRange;
            _logicComponents.Inserted += _logicComponents_Inserted;
            _logicComponents.InsertedRange += _logicComponents_InsertedRange;
            PostConstruct();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }
        //Do not call Initialize when deserializing!
        //The constructor is run before deserializing, so set defaults there.
        /// <summary>
        /// Sets all defaults, creates the root component, and generates the scene component cache.
        /// </summary>
        public virtual void Initialize()
        {
            //Has the actor already been initialized?
            if (RootComponent != null)
                return;
            _isConstructing = true;
            PreConstruct();
            RootComponent = OnConstruct();
            PostConstruct();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }
        /// <summary>
        /// Called before OnConstruct.
        /// </summary>
        protected virtual void PreConstruct() { }        
        /// <summary>
        /// Called after OnConstruct.
        /// </summary>
        protected virtual void PostConstruct() { }
        /// <summary>
        /// Sets the root component (and usually any logic components as well).
        /// </summary>
        /// <returns></returns>
        protected virtual T OnConstruct() { return Activator.CreateInstance<T>(); }

        public bool IsSpawned => _spawnIndex >= 0;
        public World OwningWorld => _owningWorld;

        public ReadOnlyCollection<SceneComponent> SceneComponentCache => _sceneComponentCache;

        SceneComponent IActor.RootComponent => RootComponent;
        public T RootComponent
        {
            get => _rootSceneComponent;
            set
            {
                if (_rootSceneComponent != null)
                    _rootSceneComponent.Owner = null;
                
                _rootSceneComponent = value;

                if (_rootSceneComponent != null)
                {
                    _rootSceneComponent.Owner = this;
                    _rootSceneComponent.RecalcGlobalTransform();
                }
                GenerateSceneComponentCache();
            }
        }

        private bool _isConstructing;
        private List<PrimitiveComponent> _renderableComponentCache = new List<PrimitiveComponent>();
        public int _spawnIndex = -1;
        private World _owningWorld;
        private ReadOnlyCollection<SceneComponent> _sceneComponentCache;

        [Serialize("RootSceneComponent")]
        private T _rootSceneComponent;
        [Serialize("LogicComponents")]
        private MonitoredList<LogicComponent> _logicComponents;

        public MonitoredList<LogicComponent> LogicComponents
        {
            get
            {
                return _logicComponents;
            }
        }
        public bool IsConstructing
            => _isConstructing;
        public List<PrimitiveComponent> RenderableComponentCache
            => _renderableComponentCache;
        public bool HasRenderableComponents
            => RenderableComponentCache.Count > 0;
        public void GenerateSceneComponentCache()
        {
            if (!_isConstructing)
            {
                _renderableComponentCache = new List<PrimitiveComponent>();
                _sceneComponentCache = _rootSceneComponent?.GenerateChildCache().AsReadOnly();
            }
        }
        public void RebaseOrigin(Vec3 newOrigin)
        {
            RootComponent?.OriginRebased(newOrigin);
        }
        protected internal override void Tick(float delta)
        {
            _rootSceneComponent.Tick(delta);
            foreach (Component c in _logicComponents)
                c.Tick(delta);
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
            foreach (LogicComponent comp in _logicComponents)
                comp.OnSpawned();
        }
        public virtual void OnDespawned()
        {
            if (!IsSpawned)
                return;

            foreach (LogicComponent comp in _logicComponents)
                comp.OnDespawned();
            _rootSceneComponent.OnDespawned();

            _spawnIndex = -1;
            _owningWorld = null;
        }
        private void _logicComponents_InsertedRange(IEnumerable<LogicComponent> items, int index)
        {
            foreach (LogicComponent item in items)
                item.Owner = this;
        }
        private void _logicComponents_Inserted(LogicComponent item, int index)
        {
            item.Owner = this;
        }
        private void _logicComponents_RemovedRange(IEnumerable<LogicComponent> items)
        {
            foreach (LogicComponent item in items)
                item.Owner = null;
        }
        private void _logicComponents_Removed(LogicComponent item)
        {
            item.Owner = null;
        }
        private void _logicComponents_AddedRange(IEnumerable<LogicComponent> items)
        {
            foreach (LogicComponent item in items)
                item.Owner = this;
        }
        private void _logicComponents_Added(LogicComponent item)
        {
            item.Owner = this;
        }
        //protected override int OnCalculateSize(StringTable table)
        //{
        //    int size = Actor.Header.Size;
        //    LogicComponents.ForEach(x => size += x.CalculateSize(table));
        //    size += RootComponent.CalculateSize(table);
        //    return size;
        //}
        //public unsafe override void Write(VoidPtr address, StringTable table)
        //{
        //    VoidPtr addr = address;
        //    Actor.Header* h = (Actor.Header*)addr;
        //    h->_nameOffset = table[_name];
        //    h->_sceneCompCount = SceneComponentCache.Count;
        //    h->_logicCompCount = LogicComponents.Count;
        //    foreach (LogicComponent comp in LogicComponents)
        //    {
        //        comp.Write(addr, table);
        //        addr += comp.CalculatedSize;
        //    }
        //    RootComponent.Write(addr, table);
        //}
    }
}
