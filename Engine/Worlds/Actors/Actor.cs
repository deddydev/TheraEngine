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
    public class Actor : FileObject
    {
        public Actor()
        {
            _isConstructing = true;
            RootComponent = SetupComponents();
            SetDefaults();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }
        public Actor(SceneComponent root, params LogicComponent[] logicComponents)
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

        [State]
        public bool IsSpawned { get { return _spawnIndex >= 0; } }
        [State]
        public World OwningWorld { get { return _owningWorld; } }

        public ReadOnlyCollection<SceneComponent> SceneComponentCache { get { return _sceneComponentCache; } }
        public SceneComponent RootComponent
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
        private SceneComponent _rootSceneComponent;
        protected ReadOnlyCollection<SceneComponent> _sceneComponentCache;
        private MonitoredList<LogicComponent> _logicComponents = new MonitoredList<LogicComponent>();

        public MonitoredList<LogicComponent> LogicComponents { get { return _logicComponents; } }
        public bool IsConstructing { get { return _isConstructing; } }

        public List<PrimitiveComponent> RenderableComponentCache { get { return _renderableComponentCache; } }
        public bool HasRenderableComponents { get { return _renderableComponentCache.Count > 0; } }

        public override ResourceType ResourceType { get { return ResourceType.Actor; } }

        protected virtual void SetDefaults() { }
        protected virtual SceneComponent SetupComponents() { return new TRSComponent(); }
        public void GenerateSceneComponentCache()
        {
            if (!_isConstructing)
            {
                _renderableComponentCache = new List<PrimitiveComponent>();
                _sceneComponentCache = _rootSceneComponent == null ? null : _rootSceneComponent.GenerateChildCache().AsReadOnly();
            }
        }
        internal void RebaseOrigin(Vec3 newOrigin)
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
        internal virtual void OnSpawned(World world)
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
        internal virtual void OnDespawned()
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

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ActorHeader
    {
        public bint _logicCompCount;
        public bint _sceneCompCount;

        public VoidPtr Address { get { fixed (void* ptr = &this) return ptr; } }
    }
}
