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
    public class World : TFileObject, I3DRenderable, I2DRenderable, IEnumerable<IActor>, IDisposable
    {
        public World() : this(new WorldSettings(), new WorldState()) { }
        public World(GlobalFileRef<WorldSettings> settings) : this(settings, new WorldState()) { }
        public World(GlobalFileRef<WorldSettings> settings, GlobalFileRef<WorldState> state)
        {
            StateRef = state;
            SettingsRef = settings;

            _rc3D = new RenderCommandMethod3D(ERenderPass.OnTopForward, Render3D);
            _rc2D = new RenderCommandMethod2D(ERenderPass.OnTopForward, Render2D);
        }

        private GlobalFileRef<WorldSettings> _settingsRef;
        private GlobalFileRef<WorldState> _stateRef;

        [TSerialize]
        public GlobalFileRef<WorldSettings> SettingsRef
        {
            get => _settingsRef;
            set
            {
                if (_settingsRef != null)
                {
                    _settingsRef.UnregisterLoadEvent(SettingsLoaded);
                    _settingsRef.UnregisterUnloadEvent(SettingsUnloaded);
                }
                _settingsRef = value ?? new GlobalFileRef<WorldSettings>();
                _settingsRef.RegisterLoadEvent(SettingsLoaded);
                _settingsRef.RegisterUnloadEvent(SettingsUnloaded);
            }
        }

        private void SettingsLoaded(WorldSettings obj)
        {
            obj.OwningWorld = this;
        }
        private void SettingsUnloaded(WorldSettings obj)
        {
            if (obj.OwningWorld == this)
                obj.OwningWorld = null;
        }

        [TSerialize(State = true)]
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
        public AbstractPhysicsWorld PhysicsWorld3D { get; private set; }

        public delegate void DelGameModeChange(World world, BaseGameMode previous, BaseGameMode next);
        public event DelGameModeChange CurrentGameModePreChanged;
        public event DelGameModeChange CurrentGameModePostChanged;
        
        public BaseGameMode CurrentGameMode
        {
            get => State.GameMode;
            set
            {
                var mode = State.GameMode;
                CurrentGameModePreChanged?.Invoke(this, mode, value);
                if (IsPlaying)
                {
                    if (mode != null)
                    {
                        if (mode.TargetWorld == this)
                            mode.TargetWorld = null;
                        mode.EndGameplay();
                    }
                }
                State.GameMode = value;
                if (IsPlaying)
                {
                    var newMode = State.GameMode;
                    if (newMode != null)
                    {
                        newMode.TargetWorld = this;
                        newMode.BeginGameplay();
                    }
                }
                CurrentGameModePostChanged?.Invoke(this, mode, value);
            }
        }

        public BaseGameMode GetDefaultGameMode()
            => Settings?.DefaultGameModeRef?.File;
        public T GetGameMode<T>() where T : class, IGameMode
            => Settings?.DefaultGameModeRef?.File as T;

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
            => PhysicsWorld3D?.StepSimulation(delta);
        
        public BaseActor this[int index]
        {
            get => State.SpawnedActors[index];
            set => State.SpawnedActors[index] = value;
        }

        public bool IsRebasingOrigin { get; private set; } = false;

        public RenderInfo3D RenderInfo3D { get; } = new RenderInfo3D(true, true);
        public RenderInfo2D RenderInfo2D { get; } = new RenderInfo2D(0, 0);

        RenderInfo3D I3DRenderable.RenderInfo => RenderInfo3D;
        RenderInfo2D I2DRenderable.RenderInfo => RenderInfo2D;

        public Scene2D OwningScene2D { get; }
        public bool IsPlaying { get; private set; }

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

            if (PhysicsWorld3D != null)
                PhysicsWorld3D.AllowIndividualAabbUpdates = false;

            //Update each actor in parallel; they should not depend on one another
            await Task.Run(() => Parallel.ForEach(State.SpawnedActors, a => a.RebaseOrigin(newOrigin)));
            //foreach (IActor a in State.SpawnedActors)
            //    a.RebaseOrigin(newOrigin);

            if (PhysicsWorld3D != null)
            {
                PhysicsWorld3D.AllowIndividualAabbUpdates = true;
                PhysicsWorld3D.UpdateAabbs();
            }

            //Scene.RegenerateTree();

            Engine.PrintLine("Finished origin rebase.");

            IsRebasingOrigin = false;
        }

        private void CreatePhysicsScene3D()
        {
            PhysicsWorld3D?.Dispose();
            PhysicsWorld3D = Engine.Physics.NewScene();
        }
        private void CreatePhysicsScene2D()
        {

        }
        public void Dispose()
        {
            PhysicsWorld3D?.Dispose();
            PhysicsWorld3D = null;
        }

        public IEnumerator<IActor> GetEnumerator() => State.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.SpawnedActors.GetEnumerator();

        protected override void OnUnload() => Dispose();
        public virtual void EndPlay()
        {
            foreach (var m in Settings.Maps)
                m.Value.File.EndPlay();

            if (Settings.TwoDimensional)
                RenderInfo2D.UnlinkScene();
            else
                RenderInfo3D.UnlinkScene();

            CurrentGameMode?.EndGameplay();
            IsPlaying = false;
        }
        public virtual void BeginPlay()
        {
            if (Settings.TwoDimensional)
            {
                State.Scene = new Scene2D(Settings.Bounds.HalfExtents.Xy);
                CreatePhysicsScene2D();
                RenderInfo2D.LinkScene(this, Scene2D);
            }
            else
            {
                State.Scene = new Scene3D(Settings.Bounds.HalfExtents);
                CreatePhysicsScene3D();
                RenderInfo3D.LinkScene(this, Scene3D);
            }

            Engine.TimeDilation = Settings.TimeDilation;

            foreach (var m in Settings.Maps)
                if (m.Value.File.VisibleByDefault)
                    m.Value.File.BeginPlay(this);

            Scene3D s3D = Scene3D;
            if (s3D != null)
            {
                s3D.RenderTree.Swap();
                if (s3D.IBLProbeActor != null)
                    s3D.IBLProbeActor.InitAndCaptureAll(512);
            }
            Scene2D s2D = Scene2D;
            if (s2D != null)
            {
                s2D.RenderTree.Swap();
            }

            CurrentGameMode?.BeginGameplay();
            IsPlaying = true;

            string cut = Settings.CutsceneToPlayOnBeginPlay;
            if (!string.IsNullOrWhiteSpace(cut) && Settings.Cutscenes.ContainsKey(cut))
                Settings.Cutscenes[cut]?.Start();
        }

        private readonly RenderCommandMethod3D _rc3D;
        private readonly RenderCommandMethod2D _rc2D;

        private void Render3D()
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
                PhysicsWorld3D?.DrawDebugWorld();
        }
        private void Render2D()
        {
            if (!(Engine.EditorState?.InEditMode ?? false))
                return;

            if (Settings.PreviewQuadtrees)
                Scene2D?.RenderTree?.DebugRender(null, true);
        }

        void I3DRenderable.AddRenderables(RenderPasses passes, Camera camera) 
            => passes.Add(_rc3D);
        void I2DRenderable.AddRenderables(RenderPasses passes, Camera camera)
            => passes.Add(_rc2D);
    }
}
