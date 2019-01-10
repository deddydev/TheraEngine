using System.Collections.Generic;
using System.Collections;
using System;
using TheraEngine.Core.Files;
using System.ComponentModel;
using TheraEngine.GameModes;
using TheraEngine.Actors;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using System.Threading.Tasks;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Worlds
{
    public interface IWorld
    {
        WorldSettings Settings { get; set; }
        AbstractPhysicsWorld PhysicsWorld { get; }
    }
    /// <summary>
    /// Manages all 3D scene data for a particular consistent instance.
    /// For example, multiple viewports can point to cameras in this world and see the same 3D scene from different viewpoints.
    /// </summary>
    [TFileExt("world")]
    [TFileDef("World")]
    public class TWorld : TFileObject, IEnumerable<IActor>, IDisposable
    {
        public TWorld() : this(new WorldSettings(), new WorldState()) { }
        public TWorld(GlobalFileRef<WorldSettings> settings) : this(settings, new WorldState()) { }
        public TWorld(GlobalFileRef<WorldSettings> settings, GlobalFileRef<WorldState> state)
        {
            StateRef = state;
            SettingsRef = settings;
        }
        
        private GlobalFileRef<WorldSettings> _settingsRef;
        private GlobalFileRef<WorldState> _stateRef;

        [TSerialize("Settings")]
        public GlobalFileRef<WorldSettings> SettingsRef
        {
            get => _settingsRef;
            set => _settingsRef = value ?? new GlobalFileRef<WorldSettings>();
        }
        [TSerialize("State", State = true)]
        public GlobalFileRef<WorldState> StateRef
        {
            get => _stateRef;
            set => _stateRef = value ?? new GlobalFileRef<WorldState>();
        }

        public WorldSettings Settings
        {
            get => SettingsRef.File;
            set => SettingsRef.File = value;
        }
        public WorldState State
        {
            get => StateRef.File;
            set => StateRef.File = value;
        }
        public BaseScene Scene
        {
            get => State.Scene;
            set => State.Scene = value;
        }
        public Scene3D Scene3D => Scene as Scene3D;
        public Scene2D Scene2D => Scene as Scene2D;
        public AbstractPhysicsWorld PhysicsWorld { get; private set; }

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
            if (actor is IActor_Internal internalActor && State.SpawnedActors.Add(internalActor))
            {
                internalActor.Spawned(this);
                //Engine.PrintLine("Spawned " + actor.Name);
            }
        }
        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor, Vec3 position)
        {
            if (actor is IActor_Internal internalActor && State.SpawnedActors.Add(internalActor))
            {
                internalActor.Spawned(this);
                internalActor.RebaseOrigin(-position);
            }
        }
        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        public void DespawnActor(IActor actor)
        {
            if (!(actor is IActor_Internal internalActor) || !State.SpawnedActors.Contains(actor))
                return;

            State.SpawnedActors.Remove(internalActor);
            internalActor.Despawned();
            //Engine.PrintLine("Despawned " + actor.Name);
        }
        
        internal void StepSimulation(float delta)
            => PhysicsWorld?.StepSimulation(delta);
        
        public IActor this[int index]
        {
            get => State.SpawnedActors[index];
            set => State.SpawnedActors[index] = value;
        }
        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// Provide any world point and that point will become the new (0,0,0).
        /// </summary>

        public bool IsRebasingOrigin { get; private set; }
        public async void RebaseOrigin(Vec3 newOrigin)
        {
            if (!Settings.EnableOriginRebasing)
                return;

            if (IsRebasingOrigin)
                return;
                //throw new Exception("Cannot rebase origin while already rebasing. Check to make sure there are no RebaseOrigin calls within rebasing code.");
            IsRebasingOrigin = true;

            Engine.PrintLine("Beginning origin rebase.");

            if (PhysicsWorld != null)
                PhysicsWorld.AllowIndividualAabbUpdates = false;

            //Update each actor in parallel; they should not depend on one another
            await Task.Run(() => Parallel.ForEach(State.SpawnedActors, a => ((IActor_Internal)a).RebaseOrigin(newOrigin)));
            //foreach (IActor a in State.SpawnedActors)
            //    a.RebaseOrigin(newOrigin);

            if (PhysicsWorld != null)
            {
                PhysicsWorld.AllowIndividualAabbUpdates = true;
                PhysicsWorld.UpdateAabbs();
            }

            Scene.RegenerateTree();

            Engine.PrintLine("Finished origin rebase.");

            IsRebasingOrigin = false;
        }

        private void CreatePhysicsScene()
        {
            PhysicsWorld?.Dispose();
            PhysicsWorld = Engine.Physics.NewScene();
        }
        public void Dispose()
        {
            PhysicsWorld?.Dispose();
            PhysicsWorld = null;
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
            //State.Scene.Lights.GlobalAmbient = Settings.GlobalAmbient;
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

//#if DEBUG
//            if (Settings.EnableOriginRebasing)
//                Scene.Add(Settings.OriginRebaseBounds);
//#endif

            //State.Scene.Add(Settings.Bounds);
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
