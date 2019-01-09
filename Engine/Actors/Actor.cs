using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEngine.Actors
{
    public delegate void DelRootComponentChanged(OriginRebasableComponent oldRoot, OriginRebasableComponent newRoot);
    public interface IActor : IFileObject
    {
        event DelRootComponentChanged RootComponentChanged;

        bool AttachedToMap { get; set; }
        bool IsConstructing { get; }
        TWorld OwningWorld { get; }

        bool IsSpawned { get; }
        void Spawned(TWorld world);
        void Despawned();

        IReadOnlyCollection<SceneComponent> SceneComponentCache { get; }
        void GenerateSceneComponentCache();
        OriginRebasableComponent RootComponent { get; }
        void RebaseOrigin(Vec3 newOrigin);

        EventList<LogicComponent> LogicComponents { get; }
        T1 FindFirstLogicComponentOfType<T1>() where T1 : LogicComponent;
        T1[] FindLogicComponentsOfType<T1>() where T1 : LogicComponent;
        LogicComponent FindFirstLogicComponentOfType(Type type);
        LogicComponent[] FindLogicComponentsOfType(Type type);
#if EDITOR
        void OnHighlightChanged(bool highlighted);
        void OnSelectedChanged(bool selected);
#endif
    }

    #region Generic Actor
    /// <summary>
    /// An actor with a TRS component as the root.
    /// Generally used for actors which manage their own world matrix or do not need one,
    /// but provide traits that affect the scene.
    /// </summary>
    [Description("An actor with a TRS component as the root. " +
        "Generally used for actors which manage their own world matrix or do not need one, " +
        "but provide traits that affect the scene.")]
    public class Actor : Actor<TRSComponent>
    {
        public Actor() : base() { }
        public Actor(TRSComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
    #endregion

    //[File3rdParty(new string[] { "dae" }, null)]
    [TFileExt("actor")]
    [TFileDef("Actor")]
    public class Actor<T> : TFileObject, IActor where T : OriginRebasableComponent
    {
        public event DelRootComponentChanged RootComponentChanged;

        static Actor()
        {
            //Register3rdPartyLoader<Actor<T>>("dae", LoadDAE);
        }

        //[ThirdPartyLoader("dae", true)]
        //public static async Task<Actor<T2>> LoadDAEAsync<T2>(string path, IProgress<float> progress, CancellationToken cancel) where T2 : T
        //{
        //    ColladaImportOptions o = new ColladaImportOptions()
        //    {
        //        IgnoreFlags = Collada.EIgnoreFlags.Extra
        //    };
        //    Collada.Data data = await Collada.ImportAsync(path, o, progress, cancel);
        //    if (data != null)
        //    {
        //        return (Actor<T2>)data.Actor;
        //    }
        //    return null;
        //}
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

            IsConstructing = true;

            _logicComponents = new EventList<LogicComponent>(logicComponents.ToList());
            _logicComponents.PostAnythingAdded += _logicComponents_PostAnythingAdded;
            _logicComponents.PostAnythingRemoved += _logicComponents_PostAnythingRemoved;

            PreConstruct();
            RootComponent = root;
            PostConstruct();

            IsConstructing = false;
            GenerateSceneComponentCache();
        }

        public float _lifeSpan = -1.0f;
        public int _spawnIndex = -1;
        private T _rootComponent;

        private EventList<LogicComponent> _logicComponents;

        //Do not call Initialize when deserializing!
        //The constructor is run before deserializing, so set defaults there.
        /// <summary>
        /// Sets all defaults, creates the root component, and generates the scene component cache.
        /// </summary>
        public virtual void Initialize()
        {
            IsConstructing = true;
            PreConstruct();
            RootComponent = OnConstructRoot();
            PostConstruct();
            IsConstructing = false;
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
        protected virtual T OnConstructRoot() => RootComponent ?? Activator.CreateInstance<T>();
        [Browsable(false)]
        public DateTime SpawnTime { get; private set; }
        [Browsable(false)]
        public TimeSpan ActiveTime => DateTime.Now - SpawnTime;
        public float LifeSpan
        {
            get => _lifeSpan;
            set
            {
                bool doNothing = (_lifeSpan > 0.0f && value > 0.0f) || (_lifeSpan <= 0.0f && value <= 0.0f);
                if (doNothing)
                {
                    _lifeSpan = value;
                    return;
                }
                if (_lifeSpan > 0.0f)
                {
                    if (IsSpawned)
                        UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, DespawnTimer, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
                }
                _lifeSpan = value;
                if (_lifeSpan > 0.0f)
                {
                    if (IsSpawned)
                        RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, DespawnTimer, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
                }
            }
        }
        [Browsable(false)]
        public bool IsSpawned => _spawnIndex >= 0;
        [Browsable(false)]
        public TWorld OwningWorld { get; private set; } = null;
        [Browsable(false)]
        public BaseScene OwningScene => OwningWorld?.Scene;
        [Browsable(false)]
        public Scene3D OwningScene3D => OwningScene as Scene3D;
        [Browsable(false)]
        public Scene2D OwningScene2D => OwningScene as Scene2D;

        private List<SceneComponent> _sceneComponentCache;
        [Browsable(false)]
        public IReadOnlyCollection<SceneComponent> SceneComponentCache => _sceneComponentCache;
        
        OriginRebasableComponent IActor.RootComponent => RootComponent;

        /// <summary>
        /// The root component is the main scene component that controls this actor's transform in the world and acts as the main ancestor for all scene components in the actor's tree.
        /// </summary>
        [Description("The root component is the main scene component that controls this actor's transform in the world" +
            " and acts as the main ancestor for all scene components in the actor's tree.")]
        [TSerialize]
        [Category("Actor")]
        [Browsable(false)]
        public T RootComponent
        {
            get => _rootComponent;
            set
            {
                if (_rootComponent == value)
                    return;

                if (_rootComponent != null)
                    _rootComponent.OwningActor = null;

                T oldRoot = _rootComponent;
                _rootComponent = value;

                if (_rootComponent != null)
                {
                    _rootComponent.OwningActor = this;
                    _rootComponent.RecalcWorldTransform();
                }
                GenerateSceneComponentCache();
                RootComponentChanged?.Invoke(oldRoot, _rootComponent);
            }
        }

        /// <summary>
        /// Determines if this actor is loaded and unloaded with the map that owns it.
        /// </summary>
        [TSerialize]
        [Category("Actor")]
        public bool AttachedToMap { get; set; } = false;
        /// <summary>
        /// Logic components handle plug-n-play code for certain features.
        /// For example, a logic component could give any actor health and/or allow it to take damage.
        /// </summary>
        [Description(@"Logic components handle plug-n-play code for certain features.
For example, a logic component could give any actor health and/or allow it to take damage.")]
        [Category("Actor")]
        [Browsable(false)]
        [TSerialize]
        public EventList<LogicComponent> LogicComponents
        {
            get => _logicComponents;
            set
            {
                if (_logicComponents != null)
                {
                    _logicComponents.PostAnythingAdded -= _logicComponents_PostAnythingAdded;
                    _logicComponents.PostAnythingRemoved -= _logicComponents_PostAnythingRemoved;
                    foreach (LogicComponent comp in _logicComponents)
                        _logicComponents_PostAnythingRemoved(comp);
                }
                _logicComponents = value ?? new EventList<LogicComponent>();
                _logicComponents.PostAnythingAdded += _logicComponents_PostAnythingAdded;
                _logicComponents.PostAnythingRemoved += _logicComponents_PostAnythingRemoved;
                foreach (LogicComponent comp in _logicComponents)
                    _logicComponents_PostAnythingAdded(comp);
            }
        }
        [Browsable(false)]
        public bool IsConstructing { get; private set; }

        public T1 FindFirstLogicComponentOfType<T1>() where T1 : LogicComponent
            => LogicComponents.FirstOrDefault(x => x is T1) as T1;
        public T1[] FindLogicComponentsOfType<T1>() where T1 : LogicComponent
            => LogicComponents.Where(x => x is T1).Select(x => (T1)x).ToArray();
        public LogicComponent FindFirstLogicComponentOfType(Type type)
            => LogicComponents.FirstOrDefault(x => type.IsAssignableFrom(x.GetType()));
        public LogicComponent[] FindLogicComponentsOfType(Type type)
            => LogicComponents.Where(x => type.IsAssignableFrom(x.GetType())).ToArray();

        //[Browsable(false)]
        //public List<I3DRenderable> RenderableComponentCache => _renderableComponentCache;
        //[Browsable(false)]
        //public bool HasRenderableComponents => RenderableComponentCache.Count > 0;

        public void GenerateSceneComponentCache()
        {
            if (!IsConstructing)
            {
                //_renderableComponentCache = new List<I3DRenderable>();
                _sceneComponentCache = _rootComponent?.GenerateChildCache() ?? new List<SceneComponent>();
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

        public void Spawned(TWorld world)
        {
            if (IsSpawned)
                return;

            _spawnIndex = -1;
            OwningWorld = world;

            OnSpawnedPreComponentSpawn();

            _rootComponent.OnSpawned();
            foreach (LogicComponent comp in _logicComponents)
                comp.OnSpawned();

            if (this is I3DRenderable r3d)
                r3d.RenderInfo.LinkScene(r3d, OwningScene3D);

            if (this is I2DRenderable r2d)
                r2d.RenderInfo.LinkScene(r2d, OwningScene2D);

            OnSpawnedPostComponentSpawn();

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.SpawnedActorCount - 1;

            if (Animations != null && Animations.Count > 0)
                Animations.ForEach(x => 
                {
                    //if (x.BeginOnSpawn)
                    //    x.Start();
                });

            SpawnTime = DateTime.Now;
            if (_lifeSpan > 0.0f)
                RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, DespawnTimer, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
        }

        private void DespawnTimer(float delta)
        {
            _lifeSpan -= delta;
            if (_lifeSpan <= 0.0f)
            {
                UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, DespawnTimer, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
                Despawn();
            }
        }

        public void Despawned()
        {
            if (!IsSpawned)
                return;

            if (this is I3DRenderable r3d)
                r3d.RenderInfo.UnlinkScene();

            if (this is I2DRenderable r2d)
                r2d.RenderInfo.UnlinkScene();

            OnDespawned();

            foreach (LogicComponent comp in _logicComponents)
                comp.OnDespawned();
            _rootComponent.OnDespawned();

            _spawnIndex = -1;
            OwningWorld = null;
        }
        /// <summary>
        /// Called before OnSpawned() is called for all logic and scene components.
        /// </summary>
        public virtual void OnSpawnedPreComponentSpawn() { }
        /// <summary>
        /// Called after OnSpawned() is called for all logic and scene components.
        /// </summary>
        public virtual void OnSpawnedPostComponentSpawn() { }
        /// <summary>
        /// Called when this actor is removed from the world.
        /// </summary>
        public virtual void OnDespawned() { }
        #endregion

#if EDITOR
        void IActor.OnHighlightChanged(bool highlighted) => OnHighlightChanged(highlighted);
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            foreach (SceneComponent s in SceneComponentCache)
            {
                s.OnHighlightChanged(highlighted);
            }
        }
        void IActor.OnSelectedChanged(bool selected) => OnSelectedChanged(selected);
        protected internal override void OnSelectedChanged(bool selected)
        {
            foreach (SceneComponent s in SceneComponentCache)
            {
                s.OnSelectedChanged(selected);
                //if (s is I3DRenderable r3d)
                //    r3d.RenderInfo.Visible = selected;
                //if (s is I2DRenderable r2d)
                //    r2d.RenderInfo.Visible = selected;
            }
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
