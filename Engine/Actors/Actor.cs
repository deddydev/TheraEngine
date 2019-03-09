using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Actors
{
    //[File3rdParty(new string[] { "dae" }, null)]
    [TFileExt("actor")]
    [TFileDef("Actor")]
    public class Actor<T> : BaseActor where T : OriginRebasableComponent
    {
        public Actor() : this(false) { }
        public Actor(string name) : this(name, false) { }
        public Actor(bool deferInitialization) : this(null, deferInitialization) { }
        public Actor(string name, bool deferInitialization) : base(name, deferInitialization) { }
        public Actor(T root, params LogicComponent[] logicComponents)
            : this(null, root, logicComponents) { }
        public Actor(string name, T root, params LogicComponent[] logicComponents) : base(name, true)
        {
            IsConstructing = true;

            if (logicComponents != null && logicComponents.Length > 0)
                _logicComponents.AddRange(logicComponents);

            PreConstruct();
            RootComponent = root;
            PostConstruct();

            IsConstructing = false;
            GenerateSceneComponentCache();
        }

        private T _rootComponent;
        
        [Browsable(false)]
        public override OriginRebasableComponent RootComponentGeneric => RootComponent;

        /// <summary>
        /// The root component is the main scene component that controls this actor's transform in the world and acts as the main ancestor for all scene components in the actor's tree.
        /// </summary>
        [Description("The root component is the main scene component that controls this actor's transform in the world" +
                     " and acts as the main ancestor for all scene components in the actor's tree.")]
        [TSerialize]
        [Category("Actor")]
        //[Browsable(false)]
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
                _rootComponent = value ?? OnConstructRoot();

                if (_rootComponent != null)
                {
                    _rootComponent.OwningActor = this;
                    _rootComponent.RecalcWorldTransform();
                }
                GenerateSceneComponentCache();
                OnRootComponentChanged(oldRoot);
            }
        }
        /// <summary>
        /// Sets the root component (and usually any logic components as well).
        /// </summary>
        /// <returns>The root scene component for this actor.</returns>
        protected virtual T OnConstructRoot() => RootComponent ?? Activator.CreateInstance<T>();

        public override void Initialize()
        {
            IsConstructing = true;
            PreConstruct();
            RootComponent = OnConstructRoot();
            PostConstruct();
            IsConstructing = false;
            GenerateSceneComponentCache();
        }
        internal override void GenerateSceneComponentCache()
        {
            if (IsConstructing)
                return;

            _sceneComponentCache = _rootComponent?.GenerateChildCache() ?? new List<SceneComponent>();
            OnSceneComponentCacheRegenerated();
        }
        internal override void RebaseOrigin(Vec3 newOrigin) => RootComponent?.RebaseOrigin(newOrigin);
    }
}
