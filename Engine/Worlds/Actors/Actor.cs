using System.Collections.Generic;
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
        public Actor()
        {
            _isConstructing = true;
            RootComponent = SetupComponents();
            SetDefaults();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }
        public Actor(T root, params LogicComponent[] logicComponents)
        {
            _isConstructing = true;
            RootComponent = root;
            _logicComponents = new MonitoredList<LogicComponent>(logicComponents.ToList());
            _logicComponents.Added += _logicComponents_Added;
            _logicComponents.AddedRange += _logicComponents_AddedRange;
            _logicComponents.Removed += _logicComponents_Removed;
            _logicComponents.RemovedRange += _logicComponents_RemovedRange;
            _logicComponents.Inserted += _logicComponents_Inserted;
            _logicComponents.InsertedRange += _logicComponents_InsertedRange;
            SetDefaults();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }
        
        public bool IsSpawned { get { return _spawnIndex >= 0; } }
        public World OwningWorld { get { return _owningWorld; } }

        public ReadOnlyCollection<SceneComponent> SceneComponentCache { get { return _sceneComponentCache; } }

        SceneComponent IActor.RootComponent => RootComponent;
        public T RootComponent
        {
            get { return _rootSceneComponent; }
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
        protected ReadOnlyCollection<SceneComponent> _sceneComponentCache;

        [Serialize("RootSceneComponent")]
        private T _rootSceneComponent;
        [Serialize("LogicComponents")]
        private MonitoredList<LogicComponent> _logicComponents = new MonitoredList<LogicComponent>();

        public MonitoredList<LogicComponent> LogicComponents => _logicComponents;
        public bool IsConstructing => _isConstructing;
        public List<PrimitiveComponent> RenderableComponentCache => _renderableComponentCache;
        public bool HasRenderableComponents => _renderableComponentCache.Count > 0;

        protected virtual void SetDefaults() { }
        protected virtual T SetupComponents() { return Activator.CreateInstance<T>(); }
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
        internal override void Tick(float delta)
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
        protected override int OnCalculateSize(StringTable table)
        {
            int size = Actor.Header.Size;
            LogicComponents.ForEach(x => size += x.CalculateSize(table));
            size += RootComponent.CalculateSize(table);
            return size;
        }
        public unsafe override void Write(VoidPtr address, StringTable table)
        {
            VoidPtr addr = address;
            Actor.Header* h = (Actor.Header*)addr;
            h->_nameOffset = table[_name];
            h->_sceneCompCount = SceneComponentCache.Count;
            h->_logicCompCount = LogicComponents.Count;
            foreach (LogicComponent comp in LogicComponents)
            {
                comp.Write(addr, table);
                addr += comp.CalculatedSize;
            }
            RootComponent.Write(addr, table);
        }
        public override void Read(VoidPtr address, VoidPtr strings)
        {
            
        }
        public override void Write(XmlWriter writer)
        {
            
        }
        public override void Read(XMLReader reader)
        {
            
        }
    }
}
