using System.Collections.Generic;
using System.Collections;
using CustomEngine.Rendering.Models;
using System;
using CustomEngine.Worlds.Actors.Components;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using CustomEngine.Files;

namespace CustomEngine.Worlds
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public class Actor : FileObject
    {
        private bool _isConstructing;
        public bool IsConstructing { get { return _isConstructing; } }
        public Actor()
        {
            _isConstructing = true;
            SetDefaults();
            RootComponent = SetupComponents();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }
        public Actor(SceneComponent root, params LogicComponent[] logicComponents)
        {
            _isConstructing = true;
            SetDefaults();
            RootComponent = root;
            _logicComponents = new MonitoredList<LogicComponent>(logicComponents.ToList());
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
                MonitoredList<SceneComponent> children = null;
                if (_rootSceneComponent != null)
                {
                    _rootSceneComponent.Owner = null;
                    children = _rootSceneComponent.Children;
                    _rootSceneComponent.Children = null;
                }
                
                _rootSceneComponent = value;

                if (_rootSceneComponent != null)
                {
                    _rootSceneComponent.Children = children;
                    _rootSceneComponent.Owner = this;
                    _rootSceneComponent.RecalcGlobalTransform();
                }
                GenerateSceneComponentCache();
            }
        }

        public int _spawnIndex = -1;
        private World _owningWorld;
        private SceneComponent _rootSceneComponent;
        protected ReadOnlyCollection<SceneComponent> _sceneComponentCache;
        private MonitoredList<LogicComponent> _logicComponents = new MonitoredList<LogicComponent>();
        private bool _isMovable = true, _simulatingPhysics = false;

        public MonitoredList<LogicComponent> LogicComponents { get { return _logicComponents; } }

        public Matrix4 WorldMatrix
        {
            get { return _rootSceneComponent != null ? _rootSceneComponent.LocalMatrix : Matrix4.Identity; }
        }
        public Matrix4 InverseWorldMatrix
        {
            get { return _rootSceneComponent != null ? _rootSceneComponent.InverseLocalMatrix : Matrix4.Identity; }
        }
        
        public bool IsMovable { get { return _isMovable; } set { _isMovable = value; } }
        public bool SimulatingPhysics { get { return _simulatingPhysics; } }

        protected virtual void SetDefaults() { }
        protected virtual SceneComponent SetupComponents() { return new GenericSceneComponent(); }
        public void GenerateSceneComponentCache()
        {
            if (!_isConstructing)
                _sceneComponentCache = _rootSceneComponent == null ? new ReadOnlyCollection<SceneComponent>() : _rootSceneComponent.GenerateChildCache().AsReadOnly();
        }
        internal void RebaseOrigin(Vec3 newOrigin)
        {
            if (_rootSceneComponent != null)
            {
                Matrix4 mtx = Matrix4.CreateTranslation(-newOrigin) * _rootSceneComponent.LocalMatrix;
                Matrix4 inv = _rootSceneComponent.InverseLocalMatrix * Matrix4.CreateTranslation(newOrigin);
                _rootSceneComponent.SetLocalMatrix(mtx, inv);
            }
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
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct ActorHeader
    {
        public VoidPtr Address { get { fixed (void* ptr = &this) return (VoidPtr)ptr; } }
    }
}
