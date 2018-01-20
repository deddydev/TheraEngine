using System.Collections.Generic;
using System.Collections;
using System;
using TheraEngine.Files;
using System.ComponentModel;
using TheraEngine.GameModes;
using TheraEngine.Worlds.Actors;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using System.Threading.Tasks;

namespace TheraEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
        AbstractPhysicsWorld PhysicsWorld { get; }
    }
    [FileExt("world")]
    [FileDef("World")]
    public class World : FileObject, IEnumerable<IActor>, IDisposable
    {
        public World() : this(new WorldSettings(), new WorldState()) { }
        public World(GlobalFileRef<WorldSettings> settings) : this(settings, new WorldState()) { }
        public World(GlobalFileRef<WorldSettings> settings, GlobalFileRef<WorldState> state)
        {
            StateRef = state;
            SettingsRef = settings;
        }
        
        private GlobalFileRef<WorldSettings> _settingsRef;
        [TSerialize("Settings")]
        public GlobalFileRef<WorldSettings> SettingsRef
        {
            get => _settingsRef;
            set => _settingsRef = value ?? new GlobalFileRef<WorldSettings>();
        }
        public WorldSettings Settings
        {
            get => SettingsRef.File;
            set => SettingsRef.File = value;
        }
        private GlobalFileRef<WorldState> _stateRef;
        [TSerialize("State", State = true)]
        public GlobalFileRef<WorldState> StateRef
        {
            get => _stateRef;
            set => _stateRef = value ?? new GlobalFileRef<WorldState>();
        }
        public WorldState State
        {
            get => StateRef.File;
            set => StateRef.File = value;
        }
        public Scene3D Scene
        {
            get => State.Scene;
            set
            {
                State.Scene = value;
            }
        }

        private AbstractPhysicsWorld _physicsWorld;
        public AbstractPhysicsWorld PhysicsWorld => _physicsWorld;
        
        public BaseGameMode GetGameMode()
            => Settings?.GameModeOverrideRef?.File;
        public T GetGameMode<T>() where T : class, IGameMode
            => Settings?.GameModeOverrideRef?.File as T;

        [Browsable(false)]
        public int SpawnedActorCount => State.SpawnedActors.Count;

        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor)
        {
            if (State.SpawnedActors.Contains(actor))
                return;
            
            State.SpawnedActors.Add(actor);
            actor.Spawned(this);
            //Engine.PrintLine("Spawned " + actor.Name);
        }
        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor, Vec3 position)
        {
            if (State.SpawnedActors.Contains(actor))
                return;

            State.SpawnedActors.Add(actor);
            actor.Spawned(this);
            actor.RebaseOrigin(-position);
        }
        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        public void DespawnActor(IActor actor)
        {
            if (!State.SpawnedActors.Contains(actor))
                return;

            State.SpawnedActors.Remove(actor);
            actor.Despawned();
            //Engine.PrintLine("Despawned " + actor.Name);
        }
        
        internal void StepSimulation(float delta)
            => _physicsWorld?.StepSimulation(delta);
        
        public IActor this[int index]
        {
            get => State.SpawnedActors[index];
            set => State.SpawnedActors[index] = value;
        }
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// Provide any world point and that point will become the new (0,0,0).
        /// </summary>

        bool _isRebasing = false;
        public async void RebaseOrigin(Vec3 newOrigin)
        {
            if (_isRebasing)
                throw new Exception();
            _isRebasing = true;

            //Engine.PrintLine("Beginning origin rebase.");
            await Task.Run(() => Parallel.ForEach(State.SpawnedActors, a => a.RebaseOrigin(newOrigin)));
            //foreach (IActor a in State.SpawnedActors)
            //    a.RebaseOrigin(newOrigin);
            //Engine.PrintLine("Finished origin rebase.");

            _isRebasing = false;
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

        public IEnumerator<IActor> GetEnumerator() => State.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.SpawnedActors.GetEnumerator();

        protected override void OnUnload() => Dispose();
        public virtual void EndPlay()
        {
            foreach (Map m in Settings.Maps)
                m.EndPlay();
        }
        public virtual void BeginPlay()
        {
            State.Scene = new Scene3D(Settings.Bounds.HalfExtents);
            State.Scene.Lights.GlobalAmbient = Settings.GlobalAmbient;
            CreatePhysicsScene();

            Engine.TimeDilation = Settings.TimeDilation;

            foreach (Map m in Settings.Maps)
            {
                if (m.Settings.VisibleByDefault)
                {
                    //m.State.Visible = true;
                    m.BeginPlay(this);
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
