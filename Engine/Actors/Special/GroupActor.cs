namespace TheraEngine.Actors.Types
{
    //    public sealed class GroupActorRootComponent : TRSComponent
    //    {
    //        public override void OnSpawned()
    //        {
    //            //do not spawn child components
    //        }
    //    }
    //    /// <summary>
    //    /// Groups a collection of actors under a single transform.
    //    /// </summary>
    //    [FileDef("Group Actor", "Groups a collection of actors under a single transform.")]
    //    public sealed class GroupActor : TFileObject, IActor
    //    {
    //        public GroupActor() : base()
    //        {
    //            Actors.PostAnythingAdded += Actors_PostAnythingAdded;
    //            Actors.PostAnythingRemoved += Actors_PostAnythingRemoved;
    //        }

    //        [TSerialize]
    //        public EventList<IActor> Actors { get; } = new EventList<IActor>();

    //        private void Actors_PostAnythingRemoved(IActor item)
    //        {
    //            RootComponent.ChildComponents.Remove(item.RootComponent);
    //            if (IsSpawned)
    //                OwningWorld.DespawnActor(item);
    //        }
    //        private void Actors_PostAnythingAdded(IActor item)
    //        {
    //            RootComponent.ChildComponents.Add(item.RootComponent);
    //            if (IsSpawned)
    //                OwningWorld.SpawnActor(item);
    //        }

    //        public int _spawnIndex = -1;
    //        private TRSComponent _rootComponent;

    //        public bool AttachedToMap { get; set; }
    //        [Browsable(false)]
    //        public bool IsSpawned => _spawnIndex >= 0;
    //        [Browsable(false)]
    //        public World OwningWorld { get; private set; } = null;
    //        [Browsable(false)]
    //        SceneComponent IActor.RootComponent => RootComponent;
    //        [Browsable(false)]
    //        World IActor.OwningWorld => OwningWorld;
    //        [Browsable(false)]
    //        EventList<LogicComponent> IActor.LogicComponents => null;
    //        [Browsable(false)]
    //        bool IActor.IsConstructing => false;

    //        public void GenerateSceneComponentCache() { }

    //        /// <summary>
    //        /// The root component is the main scene component that controls this actor's transform in the world and acts as the main ancestor for all scene components in the actor's tree.
    //        /// </summary>
    //        [Description("The root component is the main scene component that controls this actor's transform in the world" +
    //            " and acts as the main ancestor for all scene components in the actor's tree.")]
    //        [TSerialize]
    //        [Category("Actor")]
    //        public TRSComponent RootComponent
    //        {
    //            get => _rootComponent;
    //            set
    //            {
    //                if (_rootComponent != null)
    //                    _rootComponent.OwningActor = null;

    //                _rootComponent = value;

    //                if (_rootComponent != null)
    //                {
    //                    _rootComponent.OwningActor = this;
    //                    _rootComponent.RecalcWorldTransform();
    //                }
    //            }
    //        }

    //        public ReadOnlyCollection<SceneComponent> SceneComponentCache => throw new NotImplementedException();

    //        public void RebaseOrigin(Vec3 newOrigin)
    //        {
    //            //Engine.PrintLine("Rebasing actor {0}", GetType().GetFriendlyName());
    //            RootComponent?.OriginRebased(newOrigin);
    //        }

    //        #region Spawning
    //        public void Despawn()
    //        {
    //            if (IsSpawned && OwningWorld != null)
    //                OwningWorld.DespawnActor(this);
    //        }

    //        public void Spawned(World world)
    //        {
    //            if (IsSpawned)
    //                return;

    //            _spawnIndex = -1;
    //            OwningWorld = world;

    //            foreach (IActor actor in Actors)
    //                OwningWorld.SpawnActor(actor);

    //            //OnSpawned is called just after the actor is added to the actor list
    //            _spawnIndex = world.SpawnedActorCount - 1;
    //        }
    //        public void Despawned()
    //        {
    //            if (!IsSpawned)
    //                return;

    //            foreach (IActor actor in Actors)
    //                OwningWorld.DespawnActor(actor);

    //            _spawnIndex = -1;
    //            OwningWorld = null;
    //        }
    //        #endregion

    //#if EDITOR
    //        protected internal override void OnHighlightChanged(bool highlighted)
    //        {
    //            foreach (Actor s in Actors)
    //                s.OnHighlightChanged(highlighted);
    //            base.OnHighlightChanged(highlighted);
    //        }
    //        protected internal override void OnSelectedChanged(bool selected)
    //        {
    //            foreach (Actor s in Actors)
    //                s.OnSelectedChanged(selected);
    //            base.OnSelectedChanged(selected);
    //        }

    //        public T GetLogicComponent<T>() where T : LogicComponent
    //        {
    //            return null;
    //        }

    //        public T[] GetLogicComponents<T>() where T : LogicComponent
    //        {
    //            return null;
    //        }

    //        void IActor.OnHighlightChanged(bool highlighted)
    //        {
    //            throw new NotImplementedException();
    //        }

    //        void IActor.OnSelectedChanged(bool selected)
    //        {
    //            throw new NotImplementedException();
    //        }
    //#endif
    //    }
}
