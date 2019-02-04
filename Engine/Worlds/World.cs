using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

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
    public class World : TFileObject, I3DRenderable, IEnumerable<IActor>, IDisposable
    {
        public World() : this(new WorldSettings(), new WorldState()) { }
        public World(GlobalFileRef<WorldSettings> settings) : this(settings, new WorldState()) { }
        public World(GlobalFileRef<WorldSettings> settings, GlobalFileRef<WorldState> state)
        {
            StateRef = state;
            SettingsRef = settings;
            _rc = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render);
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
        public void SpawnActor(BaseActor actor)
        {
            if (State.SpawnedActors.Add(actor))
            {
                actor.Spawned(this);
                //Engine.PrintLine("Spawned " + actor.Name);
            }
        }
        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(BaseActor actor, Vec3 position)
        {
            actor.Spawned(this);
            actor.RebaseOrigin(-position);
            State.SpawnedActors.Add(actor);
        }
        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        public void DespawnActor(BaseActor actor)
        {
            if (!State.SpawnedActors.Contains(actor))
                return;

            State.SpawnedActors.Remove(actor);
            actor.Despawned();
            //Engine.PrintLine("Despawned " + actor.Name);
        }
        
        internal void StepSimulation(float delta)
            => PhysicsWorld?.StepSimulation(delta);
        
        public BaseActor this[int index]
        {
            get => State.SpawnedActors[index];
            set => State.SpawnedActors[index] = value;
        }

        public bool IsRebasingOrigin { get; private set; } = false;
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true);

        /// <summary>
        /// Moves the origin to preserve float precision when traveling large distances from the origin.
        /// Provide any world point and that point will become the new (0,0,0).
        /// </summary>
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
            await Task.Run(() => Parallel.ForEach(State.SpawnedActors, a => a.RebaseOrigin(newOrigin)));
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
            RenderInfo.UnlinkScene();
        }
        public virtual void BeginPlay()
        {
            State.Scene = new Scene3D(Settings.Bounds.HalfExtents);
            CreatePhysicsScene();

            Engine.TimeDilation = Settings.TimeDilation;

            foreach (Map m in Settings.Maps)
                if (m.VisibleByDefault)
                    m.BeginPlay(this);

            RenderInfo.LinkScene(this, Scene3D);
        }
        
        private readonly RenderCommandMethod3D _rc;
        private void Render()
        {
            if (!(Engine.EditorState?.InEditMode ?? false))
                return;

            Engine.Renderer.RenderBox(Settings.Bounds.HalfExtents, Settings.Bounds.Translation.AsTranslationMatrix(), false, Color.Green);
            if (Settings.EnableOriginRebasing)
                Engine.Renderer.RenderSphere(Vec3.Zero, Settings.OriginRebaseRadius, false, Color.Aqua);
            Frustum frustum = Engine.Renderer.CurrentCamera?.Frustum;
            if (Settings.PreviewOctrees && frustum != null)
                Scene3D?.RenderTree?.DebugRender(frustum, true);
            if (Settings.PreviewPhysics)
                PhysicsWorld?.DrawDebugWorld();
        }
        void I3DRenderable.AddRenderables(RenderPasses passes, Camera camera) => passes.Add(_rc);
    }
}
