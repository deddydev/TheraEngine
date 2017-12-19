using System.Collections.Generic;
using System.Collections;
using System;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.GameModes;
using TheraEngine.Worlds.Actors;
using TheraEngine.Physics;

namespace TheraEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
        AbstractPhysicsWorld PhysicsWorld { get; }
    }
    [FileClass("WORLD", "World")]
    public unsafe class World : FileObject, IEnumerable<IActor>, IDisposable
    {
        public World()
        {
            _settings = new WorldSettings();
            _state = new WorldState();
        }
        public World(WorldSettings settings)
        {
            _settings = settings;
            _state = new WorldState();
        }
        public World(WorldSettings settings, WorldState state)
        {
            _settings = settings;
            _state = state;
        }
        
        [TSerialize("Settings")]
        protected GlobalFileRef<WorldSettings> _settings;
        [TSerialize("State")]
        protected GlobalFileRef<WorldState> _state;
        private AbstractPhysicsWorld _physicsWorld;

        public AbstractPhysicsWorld PhysicsWorld => _physicsWorld;
        public GlobalFileRef<WorldSettings> Settings
        {
            get => _settings;
            set => _settings = value;
        }
        public GlobalFileRef<WorldState> State
        {
            get => _state;
            set => _state = value;
        }
        
        public BaseGameMode GetGameMode()
            => Settings?.File?.GameModeOverride?.File;
        public T GetGameMode<T>() where T : class, IGameMode
            => Settings?.File?.GameModeOverride?.File as T;

        [Browsable(false)]
        public int SpawnedActorCount => State.File.SpawnedActors.Count;

        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor)
        {
            if (!State.File.SpawnedActors.Contains(actor))
                State.File.SpawnedActors.Add(actor);

            actor.Spawned(this);
            Engine.PrintLine("Spawned " + actor.Name);
        }
        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor, Vec3 position)
        {
            if (!State.File.SpawnedActors.Contains(actor))
                State.File.SpawnedActors.Add(actor);
            actor.Spawned(this);
            actor.RebaseOrigin(-position);
        }
        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        public void DespawnActor(IActor actor)
        {
            if (State.File.SpawnedActors.Contains(actor))
                State.File.SpawnedActors.Remove(actor);
            actor.Despawned();
            Engine.PrintLine("Despawned " + actor.Name);
        }
        
        internal void StepSimulation(float delta)
            => _physicsWorld?.StepSimulation(delta);
        
        public IActor this[int index]
        {
            get => State.File.SpawnedActors[index];
            set => State.File.SpawnedActors[index] = value;
        }
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// Provide any world point and that point will become the new (0,0,0).
        /// </summary>
        public void RebaseOrigin(Vec3 newOrigin)
        {
            foreach (IActor a in State.File.SpawnedActors)
                a.RebaseOrigin(newOrigin);
        }

        private void CreatePhysicsScene()
        {
            _physicsWorld?.Dispose();
            _physicsWorld = Engine.Physics.NewScene();
        }
        public void Dispose()
        {
            _physicsWorld?.Dispose();
            _physicsWorld = null;
        }

        public IEnumerator<IActor> GetEnumerator() => State.File.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.File.SpawnedActors.GetEnumerator();

        protected override void OnUnload() => Dispose();
        public virtual void EndPlay()
        {
            foreach (Map m in _settings.File.Maps)
                m.EndPlay();
        }
        public virtual void BeginPlay()
        {
            foreach (Map m in _settings.File.Maps)
                m.BeginPlay();
        }
        internal protected virtual void Initialize()
        {
            CreatePhysicsScene();

            Engine.TimeDilation = _settings.File.TimeDilation;

            foreach (Map m in _settings.File.Maps)
            {
                if (m.Settings.VisibleByDefault)
                {
                    m.Initialize();
                }
            }
        }

        //public event Action<LocalPlayerController> LocalPlayerAdded;
        //internal protected virtual void OnLocalPlayerAdded(LocalPlayerController controller)
        //{
        //    LocalPlayerAdded?.Invoke(controller);
        //}
    }

    //internal class CustomOvelapFilter : OverlapFilterCallback
    //{
    //    public override bool NeedBroadphaseCollision(BroadphaseProxy proxy0, BroadphaseProxy proxy1)
    //    {
    //        return 
    //            ((short)proxy0.CollisionFilterGroup & (short)proxy1.CollisionFilterMask) != 0 &&
    //            ((short)proxy1.CollisionFilterGroup & (short)proxy0.CollisionFilterMask) != 0;
    //    }
    //}
}
