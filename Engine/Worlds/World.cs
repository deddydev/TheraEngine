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
using Extensions;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Worlds
{
    public delegate void DelGameModeChange(IWorld world, IGameMode previous, IGameMode next);
    public interface IWorld : IFileObject, I3DRenderable, I2DRenderable, IEnumerable<IActor>, IDisposable
    {
        event DelGameModeChange CurrentGameModePreChanged;

        event DelGameModeChange CurrentGameModePostChanged;
        event Action PreBeginPlay;
        event Action PostBeginPlay;
        event Action PreEndPlay;
        event Action PostEndPlay;

        GlobalFileRef<WorldSettings> SettingsRef { get; set; }
        GlobalFileRef<WorldState> StateRef { get; set; }

        WorldSettings Settings { get; set; }
        WorldState State { get; set; }

        IScene Scene { get; set; }
        IScene3D Scene3D { get; }
        IScene2D Scene2D { get; }
        IGameMode CurrentGameMode { get; set; }

        AbstractPhysicsWorld PhysicsWorld3D { get; }

        bool IsPlaying { get; }
        bool IsRebasingOrigin { get; }
        int SpawnedActorCount { get; }
        WorldManager Manager { get; set; }

        void GlobalCollectVisible(float delta);
        void GlobalSwap(float delta);
        void GlobalPreRender(float delta);

        void SpawnActor(IActor item);
        void SpawnActor(IActor actor, Vec3 position);
        void DespawnActor(IActor baseActor);

        void SpawnMap(IMap map);
        void DespawnMap(IMap map);

        void RebaseOrigin(Vec3 newOrigin);
        void BeginPlay();
        void EndPlay();
        void StepSimulation(float delta);
        void MakeCurrent();

        IGameMode GetDefaultGameMode();
        T GetGameMode<T>() where T : class, IGameMode;

        IActor this[int index] { get;set; }
    }
    /// <summary>
    /// Manages all 3D scene data for a particular consistent instance.
    /// For example, multiple viewports can point to cameras in this world and see the same 3D scene from different viewpoints.
    /// </summary>
    [Serializable]
    [TFileExt("world")]
    [TFileDef("World")]
    public class World : TFileObject, IWorld
    {
        public event DelGameModeChange CurrentGameModePreChanged;
        public event DelGameModeChange CurrentGameModePostChanged;
        public event Action PreBeginPlay;
        public event Action PostBeginPlay;
        public event Action PreEndPlay;
        public event Action PostEndPlay;

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
            set => Set(ref _settingsRef, value ?? new GlobalFileRef<WorldSettings>(),
                () =>
                {
                    _settingsRef.Loaded -= SettingsLoaded;
                    _settingsRef.Unloaded -= SettingsUnloaded;
                },
                () =>
                {
                    _settingsRef.Loaded += SettingsLoaded;
                    _settingsRef.Unloaded += SettingsUnloaded;
                });
        }

        private void SettingsLoaded(WorldSettings obj)
        {
            obj.OwningWorld = this;
            OnPropertyChanged(nameof(Settings));
        }
        private void SettingsUnloaded(WorldSettings obj)
        {
            if (obj.OwningWorld == this)
                obj.OwningWorld = null;
            OnPropertyChanged(nameof(Settings));
        }

        //[TSerialize(State = true)]
        public GlobalFileRef<WorldState> StateRef
        {
            get => _stateRef;
            set => Set(ref _stateRef, value ?? new GlobalFileRef<WorldState>());
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
        public IScene Scene
        {
            get => State.Scene;
            set => State.Scene = value;
        }
        public IScene3D Scene3D => Scene as IScene3D;
        public IScene2D Scene2D => Scene as IScene2D;
        public AbstractPhysicsWorld PhysicsWorld3D { get; private set; }

        public IGameMode CurrentGameMode
        {
            get => State.GameMode;
            set
            {
                var mode = State.GameMode;
                if (mode == value)
                    return;

                CurrentGameModePreChanged?.Invoke(this, mode, value);

                if (IsPlaying)
                    mode?.EndGameplay();

                State.GameMode = value;

                if (IsPlaying)
                    State.GameMode?.BeginGameplay(this);

                CurrentGameModePostChanged?.Invoke(this, mode, value);
            }
        }

        public void MakeCurrent()
            => Engine.SetCurrentWorld(this);
        
        public IGameMode GetDefaultGameMode()
            => Settings?.DefaultGameModeRef?.File;
        public T GetGameMode<T>() where T : class, IGameMode
            => Settings?.DefaultGameModeRef?.File as T;

        [Browsable(false)]
        public int SpawnedActorCount => State.SpawnedActors.Count;

        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor)
        {
            if (actor is null)
                return;

            actor.Spawned(this);
            State.SpawnedActors.Add(actor);
            //Engine.PrintLine("Spawned " + actor.Name);
        }
        /// <summary>
        /// Adds an actor to the scene.
        /// </summary>
        public void SpawnActor(IActor actor, Vec3 position)
        {
            if (actor is null)
                return;

            actor.Spawned(this);
            actor.RebaseOrigin(-position);
            State.SpawnedActors.Add(actor);
        }
        /// <summary>
        /// Removes an actor from the scene.
        /// </summary>
        public void DespawnActor(IActor actor)
        {
            if (actor is null || !State.SpawnedActors.Contains(actor))
                return;

            State.SpawnedActors.Remove(actor);
            actor.Despawned();
            //Engine.PrintLine("Despawned " + actor.Name);
        }
        
        void IWorld.StepSimulation(float delta)
            => PhysicsWorld3D?.StepSimulation(delta);
        
        public IActor this[int index]
        {
            get => State.SpawnedActors[index];
            set => State.SpawnedActors[index] = value;
        }

        public bool IsPlaying { get; private set; }
        public bool IsRebasingOrigin { get; private set; } = false;

        public IRenderInfo3D RenderInfo3D { get; } = new RenderInfo3D(true, true);
        public IRenderInfo2D RenderInfo2D { get; } = new RenderInfo2D(0, 0);

        IRenderInfo3D I3DRenderable.RenderInfo => RenderInfo3D;
        IRenderInfo2D I2DRenderable.RenderInfo => RenderInfo2D;

        public WorldManager Manager { get; set; }

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
            await Task.Run(() => State.SpawnedActors.ForEachParallelIList(a => a.RebaseOrigin(newOrigin)));
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
        protected override void OnUnload() => Dispose();
        public void Dispose()
        {
            PhysicsWorld3D?.Dispose();
            PhysicsWorld3D = null;
        }

        public IEnumerator<IActor> GetEnumerator() => State.SpawnedActors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => State.SpawnedActors.GetEnumerator();

        public void BeginPlay()
        {
            Engine.PrintLine("World : Beginning play.");
            PreBeginPlay?.Invoke();
            OnBeginPlay();
            PostBeginPlay?.Invoke();
            Engine.PrintLine("World : Finished beginning play.");
        }
        protected virtual void OnBeginPlay()
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
                SpawnMap(m.Value.File);

            IScene3D s3D = Scene3D;
            if (s3D != null)
            {
                s3D.RenderTree.Swap();
            }
            IScene2D s2D = Scene2D;
            if (s2D != null)
            {
                s2D.RenderTree.Swap();
            }

            IsPlaying = true;

            CurrentGameMode?.BeginGameplay(this);

            string cut = Settings.CutsceneToPlayOnBeginPlay;
            if (!string.IsNullOrWhiteSpace(cut) && Settings.Cutscenes.ContainsKey(cut))
                Settings.Cutscenes[cut]?.Start();
        }

        public void SpawnMap(IMap value)
        {
            if (value is null)
                return;

            if (value.VisibleByDefault)
                value.BeginPlay(this);

            State.SpawnedMaps.Add(value);
            Engine.PrintLine("World : Spawned map.");
        }
        public void DespawnMap(IMap value)
        {
            if (value is null)
                return;

            if (value.IsVisible)
                value.EndPlay();

            State.SpawnedMaps.Remove(value);
            Engine.PrintLine("World : Despawned map.");
        }

        public void EndPlay()
        {
            Engine.PrintLine("World : Ending play.");
            PreEndPlay?.Invoke();
            OnEndPlay();
            PostEndPlay?.Invoke();
            Engine.PrintLine("World : Finished ending play.");
        }
        protected virtual void OnEndPlay()
        {
            State.EndPlay();

            if (Settings.TwoDimensional)
                RenderInfo2D.UnlinkScene();
            else
                RenderInfo3D.UnlinkScene();

            CurrentGameMode?.EndGameplay();
            IsPlaying = false;
        }

        private readonly RenderCommandMethod3D _rc3D;
        private readonly RenderCommandMethod2D _rc2D;

        private void Render3D(bool shadowPass)
        {
            if (Settings is null)
                return;

            if (Settings.PreviewWorldBounds)
                Engine.Renderer.RenderBox(Settings.Bounds.HalfExtents, Settings.Bounds.Translation.AsTranslationMatrix(), false, Color.Green);
            
            if (Settings.EnableOriginRebasing)
                Engine.Renderer.RenderSphere(Vec3.Zero, Settings.OriginRebaseRadius, false, Color.Aqua);

            IFrustum frustum = Engine.Renderer.CurrentCamera?.Frustum;
            if (Settings.PreviewOctrees && frustum != null)
                Scene3D?.RenderTree?.DebugRender(frustum, true);

            if (Settings.PreviewPhysics)
                PhysicsWorld3D?.DrawDebugWorld();

            //if (!shadowPass)
            //{
            //    while (PhysicsWorld3D.ConsumingRayTraces.TryDequeue(out RayTrace trace))
            //        trace.Render();

            //    while (PhysicsWorld3D.ConsumingShapeTraces.TryDequeue(out ShapeTrace trace))
            //        trace.Render();
            //}
        }
        private void Render2D()
        {
#if EDITOR
            if (!(Engine.EditorState?.InEditMode ?? false))
                return;
#endif

            if (Settings.PreviewQuadtrees)
                Scene2D?.RenderTree?.DebugRender(null, true);
        }

        void I3DRenderable.AddRenderables(RenderPasses passes, ICamera camera)
        {
#if EDITOR
            if (!(Engine.EditorState?.InEditMode ?? false))
                return;
#endif
            passes.Add(_rc3D);
        }
        void I2DRenderable.AddRenderables(RenderPasses passes, ICamera camera)
        {
#if EDITOR
            if (!(Engine.EditorState?.InEditMode ?? false))
                return;
#endif
            passes.Add(_rc2D);
        }

        void IWorld.GlobalCollectVisible(float delta)
        {
            if (!Engine.IsPaused)
                PhysicsWorld3D?.StepSimulation(delta);

            Scene?.GlobalCollectVisible();
        }
        void IWorld.GlobalSwap(float delta)
        {
            Scene?.GlobalSwap();
            PhysicsWorld3D?.Swap();
        }
        void IWorld.GlobalPreRender(float delta)
        {
            Scene?.GlobalRender();
        }
    }
}
