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
    public interface IActor : IFileObject
    {
        bool IsConstructing { get; }
        World OwningWorld { get; }
        bool IsSpawned { get; }
        void Spawned(World world);
        void Despawned();
        void GenerateSceneComponentCache();
        SceneComponent RootComponent { get; }
        EventList<LogicComponent> LogicComponents { get; }
        void RebaseOrigin(Vec3 newOrigin);
    }

    #region Generic Actor
    /// <summary>
    /// An actor with a generic scene component as the root.
    /// Generally used for actors which manage their own world matrix or do not need one,
    /// but provide traits that affect the scene.
    /// </summary>
    [Description("An actor with a generic scene component as the root. " +
        "Generally used for actors which manage their own world matrix or do not need one, " +
        "but provide traits that affect the scene.")]
    public class Actor : Actor<SceneComponent>
    {
        public Actor() : base() { }
        public Actor(SceneComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
    #endregion

    [File3rdParty(new string[] { "dae" }, null)]
    [FileExt("actor")]
    [FileDef("Actor")]
    public class Actor<T> : FileObject, IActor where T : SceneComponent
    {
        static Actor()
        {
            //Register3rdPartyLoader<Actor<T>>("dae", LoadDAE);
        }

        [ThirdPartyLoader("dae")]
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
        public Actor(string name) : this(name, false) { }
        public Actor(bool deferInitialization) : this(null, deferInitialization) { }
        public Actor(string name, bool deferInitialization)
        {
            Name = string.IsNullOrEmpty(name) ? GetType().GetFriendlyName("[", "]") : name;

            _logicComponents = new EventList<LogicComponent>();
            _logicComponents.PostAnythingAdded += _logicComponents_PostAnythingAdded;
            _logicComponents.PostAnythingRemoved += _logicComponents_PostAnythingRemoved;
            if (!deferInitialization)
                Initialize();
        }
        public Actor(T root, params LogicComponent[] logicComponents)
            : this(null, root, logicComponents) { }
        public Actor(string name, T root, params LogicComponent[] logicComponents)
        {
            Name = string.IsNullOrEmpty(name) ? GetType().GetFriendlyName("[", "]") : name;

            _isConstructing = true;

            _logicComponents = new EventList<LogicComponent>(logicComponents.ToList());
            _logicComponents.PostAnythingAdded += _logicComponents_PostAnythingAdded;
            _logicComponents.PostAnythingRemoved += _logicComponents_PostAnythingRemoved;

            PreConstruct();
            RootComponent = root;
            PostConstruct();

            _isConstructing = false;
            GenerateSceneComponentCache();
        }

        private bool _isConstructing;
        private List<I2DRenderable> _renderableComponentCache = new List<I2DRenderable>();
        public int _spawnIndex = -1;
        private World _owningWorld;
        private ReadOnlyCollection<SceneComponent> _sceneComponentCache;
        private T _rootComponent;

        [TSerialize("LogicComponents")]
        private EventList<LogicComponent> _logicComponents;

        //Do not call Initialize when deserializing!
        //The constructor is run before deserializing, so set defaults there.
        /// <summary>
        /// Sets all defaults, creates the root component, and generates the scene component cache.
        /// </summary>
        public virtual void Initialize()
        {
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
        protected virtual T OnConstruct() => RootComponent ?? Activator.CreateInstance<T>();

        [Browsable(false)]
        public bool IsSpawned => _spawnIndex >= 0;
        [Browsable(false)]
        public World OwningWorld => _owningWorld;

        [Browsable(false)]
        public ReadOnlyCollection<SceneComponent> SceneComponentCache => _sceneComponentCache;

        [Browsable(false)]
        SceneComponent IActor.RootComponent => RootComponent;

        /// <summary>
        /// The root component is the main scene component that controls this actor's transform in the world and acts as the main ancestor for all scene components in the actor's tree.
        /// </summary>
        [Description("The root component is the main scene component that controls this actor's transform in the world" +
            " and acts as the main ancestor for all scene components in the actor's tree.")]
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

        /// <summary>
        /// Logic components handle plug-n-play code for certain features.
        /// For example, a logic component could give any actor health and/or allow it to take damage.
        /// </summary>
        [Description(@"Logic components handle plug-n-play code for certain features.
For example, a logic component could give any actor health and/or allow it to take damage.")]
        [Category("Actor")]
        public EventList<LogicComponent> LogicComponents => _logicComponents;
        [Browsable(false)]
        public bool IsConstructing => _isConstructing;

        [Browsable(false)]
        public List<I2DRenderable> RenderableComponentCache => _renderableComponentCache;
        [Browsable(false)]
        public bool HasRenderableComponents => RenderableComponentCache.Count > 0;

        public void GenerateSceneComponentCache()
        {
            if (!_isConstructing)
            {
                _renderableComponentCache = new List<I2DRenderable>();
                _sceneComponentCache = _rootComponent?.GenerateChildCache().AsReadOnly();
            }
        }
        public void RebaseOrigin(Vec3 newOrigin)
        {
            //Engine.PrintLine("Rebasing actor {0}", GetType().GetFriendlyName());
            RootComponent?.OriginRebased(newOrigin);
        }

        #region Spawning
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
        /// <summary>
        /// Called before OnSpawned() is called for all logic and scene components.
        /// </summary>
        /// <param name="world">The world this actor has been spawned in.</param>
        public virtual void OnSpawnedPreComponentSetup(World world) { }
        /// <summary>
        /// Called after OnSpawned() is called for all logic and scene components.
        /// </summary>
        /// <param name="world">The world this actor has been spawned in.</param>
        public virtual void OnSpawnedPostComponentSetup(World world) { }
        /// <summary>
        /// Called when this actor is removed from the world.
        /// </summary>
        public virtual void OnDespawned() { }
        #endregion

#if EDITOR
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            foreach (SceneComponent s in SceneComponentCache)
                s.OnHighlightChanged(highlighted);
            base.OnHighlightChanged(highlighted);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            foreach (SceneComponent s in SceneComponentCache)
                s.OnSelectedChanged(selected);
            base.OnSelectedChanged(selected);
        }
#endif

        private void _logicComponents_PostAnythingRemoved(LogicComponent item)
        {
            if (item.OwningActor == this)
                item.OwningActor = null;
        }
        private void _logicComponents_PostAnythingAdded(LogicComponent item)
        {
            item.OwningActor = this;
        }
    }
}
