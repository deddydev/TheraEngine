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
    public delegate void DelRootComponentChanged(BaseActor actor, OriginRebasableComponent oldRoot);
    public abstract class BaseActor : TFileObject, IActor
    {
        public event DelRootComponentChanged RootComponentChanged;
        public event Action<BaseActor> SceneComponentCacheRegenerated;
        public event Action<BaseActor> LogicComponentsChanged;

        protected void OnRootComponentChanged(OriginRebasableComponent oldRoot) => RootComponentChanged?.Invoke(this, oldRoot);
        protected void OnSceneComponentCacheRegenerated() => SceneComponentCacheRegenerated?.Invoke(this);
        protected void OnLogicComponentsChanged() => LogicComponentsChanged?.Invoke(this);

        protected BaseActor(string name, bool deferInitialization)
        {
            Name = string.IsNullOrEmpty(name) ? GetType().GetFriendlyName("[", "]") : name;

            _logicComponents = new EventList<LogicComponent>();
            _logicComponents.PostAnythingAdded += LogicComponents_PostAnythingAdded;
            _logicComponents.PostAnythingRemoved += LogicComponents_PostAnythingRemoved;

            if (!deferInitialization)
                Initialize();
        }

        public float _lifeSpan = -1.0f;
        public int _spawnIndex = -1;
        protected BaseScene _scene;
        protected EventList<LogicComponent> _logicComponents;
        protected List<SceneComponent> _sceneComponentCache;

        [Browsable(false)]
        public Scene3D OwningScene3D => OwningScene as Scene3D;
        [Browsable(false)]
        public Scene2D OwningScene2D => OwningScene as Scene2D;
        [Browsable(false)]
        public IReadOnlyCollection<SceneComponent> SceneComponentCache => _sceneComponentCache;
        [Browsable(false)]
        public abstract OriginRebasableComponent RootComponentGeneric { get; }

        OriginRebasableComponent IActor.RootComponent => RootComponentGeneric;

        [Category("Actor")]
        public float CurrentLife { get; private set; }
        [Category("Actor")]
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
        public DateTime SpawnTime { get; private set; }
        [Browsable(false)]
        public TimeSpan ActiveTime => DateTime.Now - SpawnTime;
        [Browsable(false)]
        public bool IsSpawned => _spawnIndex >= 0 && OwningWorld != null;
        [Browsable(false)]
        public World OwningWorld { get; private set; } = null;
        [Browsable(false)]
        public BaseScene OwningScene
        {
            get => _scene ?? OwningWorld?.Scene;
            set => _scene = value;
        }
        [TSerialize]
        [Browsable(false)]
        public Map MapAttachment { get; set; } = null;
        /// <summary>
        /// Logic components handle plug-n-play code for certain features.
        /// For example, a logic component could give any actor health and/or allow it to take damage.
        /// </summary>
        //[Browsable(false)]
        [TSerialize]
        [Category("Actor")]
        public EventList<LogicComponent> LogicComponents
        {
            get => _logicComponents;
            set
            {
                if (_logicComponents != null)
                {
                    _logicComponents.PostAnythingAdded -= LogicComponents_PostAnythingAdded;
                    _logicComponents.PostAnythingRemoved -= LogicComponents_PostAnythingRemoved;
                    foreach (LogicComponent comp in _logicComponents)
                        LogicComponents_PostAnythingRemoved(comp);
                }
                _logicComponents = value ?? new EventList<LogicComponent>();
                _logicComponents.PostAnythingAdded += LogicComponents_PostAnythingAdded;
                _logicComponents.PostAnythingRemoved += LogicComponents_PostAnythingRemoved;
                foreach (LogicComponent comp in _logicComponents)
                    LogicComponents_PostAnythingAdded(comp);
            }
        }
        [Browsable(false)]
        public bool IsConstructing { get; protected set; }

        public T1 FindFirstLogicComponentOfType<T1>() where T1 : LogicComponent
            => LogicComponents.FirstOrDefault(x => x is T1) as T1;
        public T1[] FindLogicComponentsOfType<T1>() where T1 : LogicComponent
            => LogicComponents.OfType<T1>().ToArray();
        public LogicComponent FindFirstLogicComponentOfType(Type type)
            => LogicComponents.FirstOrDefault(type.IsInstanceOfType);
        public LogicComponent[] FindLogicComponentsOfType(Type type)
            => LogicComponents.Where(type.IsInstanceOfType).ToArray();

        //[Browsable(false)]
        //public List<I3DRenderable> RenderableComponentCache => _renderableComponentCache;
        //[Browsable(false)]
        //public bool HasRenderableComponents => RenderableComponentCache.Count > 0;

        //Do not call Initialize when deserializing!
        //The constructor is run before deserializing, so set defaults there.
        /// <summary>
        /// Sets all defaults, creates the root component, and generates the scene component cache.
        /// </summary>
        public abstract void Initialize();
        /// <summary>
        /// Called before OnConstruct.
        /// </summary>
        protected virtual void PreConstruct() { }
        /// <summary>
        /// Called after OnConstruct.
        /// </summary>
        protected virtual void PostConstruct() { }
        
        internal abstract void GenerateSceneComponentCache();
        internal abstract void RebaseOrigin(Vec3 newOrigin);

        #region Spawning
        public void Despawn()
        {
            if (IsSpawned)
                OwningWorld?.DespawnActor(this);
        }
        internal void Spawned(World world)
        {
            if (IsSpawned)
                return;

            _spawnIndex = -1;
            OwningWorld = world;

            OnSpawnedPreComponentSpawn();

            RootComponentGeneric.OnSpawned();
            foreach (LogicComponent comp in _logicComponents)
                comp.OnSpawned();

            if (this is IPreRendered r)
                OwningScene?.AddPreRenderedObject(r);

            if (this is I3DRenderable r3D)
                r3D.RenderInfo.LinkScene(r3D, OwningScene3D);

            if (this is I2DRenderable r2D)
                r2D.RenderInfo.LinkScene(r2D, OwningScene2D);

            OnSpawnedPostComponentSpawn();

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.SpawnedActorCount - 1;

            if (Animations != null && Animations.Count > 0)
                Animations.ForEach(anim =>
                {
                    if (anim.BeginOnSpawn)
                        anim.Start();
                });

            SpawnTime = DateTime.Now;
            if (_lifeSpan > 0.0f)
            {
                CurrentLife = _lifeSpan;
                RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, DespawnTimer, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            }
        }
        internal void Despawned()
        {
            if (!IsSpawned)
                return;

            if (this is IPreRendered r)
                OwningScene?.RemovePreRenderedObject(r);

            if (this is I3DRenderable r3D)
                r3D.RenderInfo.UnlinkScene();

            if (this is I2DRenderable r2D)
                r2D.RenderInfo.UnlinkScene();

            OnDespawned();

            foreach (LogicComponent comp in _logicComponents)
                comp.OnDespawned();
            RootComponentGeneric.OnDespawned();

            _spawnIndex = -1;
            OwningWorld = null;
        }
        private void DespawnTimer(float delta)
        {
            CurrentLife -= delta;
            if (CurrentLife > 0.0f)
                return;
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, DespawnTimer, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            OnLifeSpanOver();
        }
        /// <summary>
        /// Overridable method called when an actor's life span runs out.
        /// By default, despawns the actor.
        /// </summary>
        protected virtual void OnLifeSpanOver() => Despawn();

        /// <summary>
        /// Called before OnSpawned() is called for all logic and scene components.
        /// </summary>
        protected virtual void OnSpawnedPreComponentSpawn() { }
        /// <summary>
        /// Called after OnSpawned() is called for all logic and scene components.
        /// </summary>
        protected virtual void OnSpawnedPostComponentSpawn() { }
        /// <summary>
        /// Called when this actor is removed from the world.
        /// </summary>
        protected virtual void OnDespawned() { }
        #endregion

        protected void LogicComponents_PostAnythingRemoved(LogicComponent item)
        {
            if (item.OwningActor == this)
                item.OwningActor = null;
            LogicComponentsChanged?.Invoke(this);
        }
        protected void LogicComponents_PostAnythingAdded(LogicComponent item)
        {
            item.OwningActor = this;
            LogicComponentsChanged?.Invoke(this);
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

#if EDITOR
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            if (SceneComponentCache == null)
                return;

            foreach (SceneComponent s in SceneComponentCache)
                s.OnHighlightChanged(highlighted);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (SceneComponentCache == null)
                return;

            foreach (SceneComponent s in SceneComponentCache)
                s.OnSelectedChanged(selected);
        }
#endif
    }
}
