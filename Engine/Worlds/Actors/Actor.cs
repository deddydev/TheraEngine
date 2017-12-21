using System.Collections.Generic;
using System;
using TheraEngine.Worlds.Actors;
using System.Collections.ObjectModel;
using System.Linq;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors.Components;
using TheraEngine.Worlds.Actors.Components.Scene;

namespace TheraEngine.Worlds.Actors
{
    public enum EActorType
    {
        Static, //This actor is part of the map
        Dynamic, //This actor can be changed/manipulated
    }
    public interface IActor : IFileObject
    {
        bool IsConstructing { get; }
        World OwningWorld { get; }
        bool IsSpawned { get; }
        void Spawned(World world);
        void Despawned();
        void GenerateSceneComponentCache();
        SceneComponent RootComponent { get; }
        MonitoredList<LogicComponent> LogicComponents { get; }
        void RebaseOrigin(Vec3 newOrigin);
    }
    public class Actor : Actor<SceneComponent>
    {
        public Actor() : base() { }
        public Actor(SceneComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
    [FileClass("ACTOR", "Actor", ImportableExtensions = new string[] { "DAE" })]
    public class Actor<T> : FileObject, IActor where T : SceneComponent
    {
        [ThirdPartyLoader("DAE")]
        public static FileObject LoadDAE(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags = Core.Files.IgnoreFlags.Extra
            };
            Collada.Data data = Collada.Import(path, o);
            if (data != null)
            {
            }
            return null;
        }
        public Actor() : this(false) { }
        public Actor(bool deferInitialization)
        {
            _logicComponents = new MonitoredList<LogicComponent>();
            _logicComponents.PostAdded += _logicComponents_Added;
            _logicComponents.PostAddedRange += _logicComponents_AddedRange;
            _logicComponents.PostRemoved += _logicComponents_Removed;
            _logicComponents.PostRemovedRange += _logicComponents_RemovedRange;
            _logicComponents.PostInserted += _logicComponents_Inserted;
            _logicComponents.PostInsertedRange += _logicComponents_InsertedRange;
            if (!deferInitialization)
                Initialize();
        }
        public Actor(T root, params LogicComponent[] logicComponents)
        {
            _isConstructing = true;
            PreConstruct();
            RootComponent = root;
            _logicComponents = new MonitoredList<LogicComponent>(logicComponents.ToList());
            _logicComponents.PostAdded += _logicComponents_Added;
            _logicComponents.PostAddedRange += _logicComponents_AddedRange;
            _logicComponents.PostRemoved += _logicComponents_Removed;
            _logicComponents.PostRemovedRange += _logicComponents_RemovedRange;
            _logicComponents.PostInserted += _logicComponents_Inserted;
            _logicComponents.PostInsertedRange += _logicComponents_InsertedRange;
            PostConstruct();
            _isConstructing = false;
            GenerateSceneComponentCache();
        }

        private bool _isConstructing;
        private List<PrimitiveComponent> _renderableComponentCache = new List<PrimitiveComponent>();
        public int _spawnIndex = -1;
        private World _owningWorld;
        private ReadOnlyCollection<SceneComponent> _sceneComponentCache;
        private T _rootComponent;

        [TSerialize("LogicComponents")]
        private MonitoredList<LogicComponent> _logicComponents;

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
        /// <returns>The root scene component for this actor.</returns>
        protected virtual T OnConstruct() => Activator.CreateInstance<T>();

        [Browsable(false)]
        public bool IsSpawned => _spawnIndex >= 0;
        [Browsable(false)]
        public World OwningWorld => _owningWorld;

        [Browsable(false)]
        public ReadOnlyCollection<SceneComponent> SceneComponentCache => _sceneComponentCache;

        [Browsable(false)]
        SceneComponent IActor.RootComponent => RootComponent;

        [Description(@"The root component is the main scene component that controls this actor's transform in the world and acts as the main ancestor for all scene components in the actor's tree.")]
        [TSerialize]
        [Category("Actor")]
        public T RootComponent
        {
            get => _rootComponent;
            set
            {
                if (_rootComponent != null)
                    _rootComponent.OwningActor = null;
                
                _rootComponent = value;

                if (_rootComponent != null)
                {
                    _rootComponent.OwningActor = this;
                    _rootComponent.RecalcGlobalTransform();
                }
                GenerateSceneComponentCache();
            }
        }

        [Description(@"Logic components handle plug-n-play code for certain features.
For example, a logic component could give any actor health and/or allow it to take damage.")]
        [Category("Actor")]
        public MonitoredList<LogicComponent> LogicComponents => _logicComponents;
        [Browsable(false)]
        public bool IsConstructing => _isConstructing;
        [Browsable(false)]
        public List<PrimitiveComponent> RenderableComponentCache => _renderableComponentCache;
        [Browsable(false)]
        public bool HasRenderableComponents => RenderableComponentCache.Count > 0;

        public void GenerateSceneComponentCache()
        {
            if (!_isConstructing)
            {
                _renderableComponentCache = new List<PrimitiveComponent>();
                _sceneComponentCache = _rootComponent?.GenerateChildCache().AsReadOnly();
            }
        }
        public void RebaseOrigin(Vec3 newOrigin)
        {
            RootComponent?.OriginRebased(newOrigin);
        }
        public void Despawn()
        {
            if (IsSpawned && OwningWorld != null)
                OwningWorld.DespawnActor(this);
        }

        public void Spawned(World world)
        {
            if (IsSpawned)
                return;

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.SpawnedActorCount - 1;
            _owningWorld = world;

            OnSpawnedPreComponentSetup(world);

            _rootComponent.OnSpawned();
            foreach (LogicComponent comp in _logicComponents)
                comp.OnSpawned();

            OnSpawnedPostComponentSetup(world);
        }
        public void Despawned()
        {
            if (!IsSpawned)
                return;

            OnDespawned();

            foreach (LogicComponent comp in _logicComponents)
                comp.OnDespawned();
            _rootComponent.OnDespawned();

            _spawnIndex = -1;
            _owningWorld = null;
        }

        public virtual void OnSpawnedPreComponentSetup(World world) { }
        public virtual void OnSpawnedPostComponentSetup(World world) { }
        public virtual void OnDespawned() { }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            foreach (SceneComponent s in SceneComponentCache)
                s.OnSelectedChanged(selected);
        }
#endif

        #region Logic Components
        private void _logicComponents_InsertedRange(IEnumerable<LogicComponent> items, int index)
        {
            foreach (LogicComponent item in items)
                item.OwningActor = this;
        }
        private void _logicComponents_Inserted(LogicComponent item, int index)
        {
            item.OwningActor = this;
        }
        private void _logicComponents_RemovedRange(IEnumerable<LogicComponent> items)
        {
            foreach (LogicComponent item in items)
                item.OwningActor = null;
        }
        private void _logicComponents_Removed(LogicComponent item)
        {
            item.OwningActor = null;
        }
        private void _logicComponents_AddedRange(IEnumerable<LogicComponent> items)
        {
            foreach (LogicComponent item in items)
                item.OwningActor = this;
        }
        private void _logicComponents_Added(LogicComponent item)
        {
            item.OwningActor = this;
        }
        #endregion
    }
}
