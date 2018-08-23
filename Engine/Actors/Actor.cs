﻿using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.Rendering.Models;
using TheraEngine.Components;
using TheraEngine.Worlds;
using TheraEngine.Components.Scene.Transforms;
using System.Threading.Tasks;
using TheraEngine.Rendering;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Actors
{
    public interface IActor : IFileObject
    {
        bool IsConstructing { get; }
        World OwningWorld { get; }
        bool IsSpawned { get; }
        void Spawned(World world);
        void Despawned();
        ReadOnlyCollection<SceneComponent> SceneComponentCache { get; }
        void GenerateSceneComponentCache();
        SceneComponent RootComponent { get; }
        EventList<LogicComponent> LogicComponents { get; }
        void RebaseOrigin(Vec3 newOrigin);
        T GetLogicComponent<T>() where T : LogicComponent;
        T[] GetLogicComponents<T>() where T : LogicComponent;
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
    public class Actor : Actor<OriginRebasableComponent>
    {
        public Actor() : base() { }
        public Actor(OriginRebasableComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
    #endregion

    [File3rdParty(new string[] { "dae" }, null)]
    [FileExt("actor")]
    [FileDef("Actor")]
    public class Actor<T> : TFileObject, IActor where T : OriginRebasableComponent
    {
        static Actor()
        {
            //Register3rdPartyLoader<Actor<T>>("dae", LoadDAE);
        }

        [ThirdPartyLoader("dae", true)]
        public static async Task<TFileObject> LoadDAEAsync(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags = Collada.EIgnoreFlags.Extra
            };
            Collada.Data data = await Collada.ImportAsync(path, o);
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

        //private List<I3DRenderable> _renderableComponentCache = new List<I3DRenderable>();
        public int _spawnIndex = -1;
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
            IsConstructing = true;
            PreConstruct();
            RootComponent = OnConstruct();
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
        protected virtual T OnConstruct() => RootComponent ?? Activator.CreateInstance<T>();

        [Browsable(false)]
        public bool IsSpawned => _spawnIndex >= 0;
        [Browsable(false)]
        public World OwningWorld { get; private set; } = null;
        [Browsable(false)]
        public BaseScene OwningScene => OwningWorld?.Scene;
        [Browsable(false)]
        public Scene3D OwningScene3D => OwningScene as Scene3D;
        [Browsable(false)]
        public Scene2D OwningScene2D => OwningScene as Scene2D;

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
        [Browsable(false)]
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
                    _rootComponent.RecalcWorldTransform();
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
        [Browsable(false)]
        public EventList<LogicComponent> LogicComponents => _logicComponents;
        [Browsable(false)]
        public bool IsConstructing { get; private set; }

        public T1 GetLogicComponent<T1>() where T1 : LogicComponent
            => LogicComponents.FirstOrDefault(x => x is T1) as T1;
        public T1[] GetLogicComponents<T1>() where T1 : LogicComponent
            => LogicComponents.Where(x => x is T1).Select(x => (T1)x).ToArray();

        //[Browsable(false)]
        //public List<I3DRenderable> RenderableComponentCache => _renderableComponentCache;
        //[Browsable(false)]
        //public bool HasRenderableComponents => RenderableComponentCache.Count > 0;

        public void GenerateSceneComponentCache()
        {
            if (!IsConstructing)
            {
                //_renderableComponentCache = new List<I3DRenderable>();
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

            _spawnIndex = -1;
            OwningWorld = world;

            OnSpawnedPreComponentSpawn();

            _rootComponent.OnSpawned();
            foreach (LogicComponent comp in _logicComponents)
                comp.OnSpawned();

            if (this is I3DRenderable r3d)
            {
                bool spawn = r3d.RenderInfo.Visible;
#if EDITOR
                if (r3d.RenderInfo.VisibleInEditorOnly)
                    spawn = spawn && Engine.EditorState.InEditMode;
#endif
                if (spawn && OwningScene != null)
                    OwningScene.Add(r3d);
            }

            if (this is I2DRenderable r2d)
            {
                bool spawn = r2d.RenderInfo.Visible;
#if EDITOR
                if (r2d.RenderInfo.VisibleInEditorOnly)
                    spawn = spawn && Engine.EditorState.InEditMode;
#endif
                if (spawn && OwningScene != null)
                    OwningScene.Add(r2d);
            }

            OnSpawnedPostComponentSpawn();

            //OnSpawned is called just after the actor is added to the actor list
            _spawnIndex = world.SpawnedActorCount - 1;
        }
        public void Despawned()
        {
            if (!IsSpawned)
                return;

            if (this is I3DRenderable r3d && r3d.RenderInfo.Visible)
                OwningScene.Remove(r3d);

            if (this is I2DRenderable r2d && r2d.RenderInfo.Visible)
                OwningScene.Remove(r2d);

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
