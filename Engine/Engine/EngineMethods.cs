using TheraEngine.Files;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Worlds.Actors;
using BulletSharp;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using TheraEngine.Timers;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing;
using System.Text;
using TheraEngine.GameModes;
using TheraEngine.Core.Shapes;

namespace TheraEngine
{
    public static partial class Engine
    {
        static Engine()
        {
            //Steamworks.SteamAPI.Init();

            _timer = new EngineTimer();
            _timer.UpdateFrame += Tick;
            
            ActivePlayers.PostAdded += ActivePlayers_Added;
            ActivePlayers.PostRemoved += ActivePlayers_Removed;

            _tickLists = new ThreadSafeList<DelTick>[45];
            for (int i = 0; i < _tickLists.Length; ++i)
                _tickLists[i] = new ThreadSafeList<DelTick>();

            PersistentManifold.ContactProcessed += PersistentManifold_ContactProcessed;
            PersistentManifold.ContactDestroyed += PersistentManifold_ContactDestroyed;
            ManifoldPoint.ContactAdded += ManifoldPoint_ContactAdded;
        }

        #region Collision Handling
        private class PhysicsDriverPair
        {
            public PhysicsDriverPair(PhysicsDriver driver0, PhysicsDriver driver1)
            {
                _driver0 = driver0;
                _driver1 = driver1;
            }
            public PhysicsDriver _driver0, _driver1;
        }
        private static void PersistentManifold_ContactProcessed(ManifoldPoint cp, CollisionObject body0, CollisionObject body1)
        {
            //PhysicsDriver driver0 = (PhysicsDriver)body0.UserObject;
            //PhysicsDriver driver1 = (PhysicsDriver)body1.UserObject;
            //cp.UserPersistentData = new PhysicsDriverPair(driver0, driver1);
            //driver0.ContactStarted(driver1, cp);
        }
        private static void PersistentManifold_ContactDestroyed(object userPersistantData)
        {
            //PhysicsDriverPair drivers = (PhysicsDriverPair)userPersistantData;
            //drivers._driver0.ContactEnded(drivers._driver1);
            //drivers._driver1.ContactEnded(drivers._driver0);
        }
        private static void ManifoldPoint_ContactAdded(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            PhysicsDriver driver0 = (PhysicsDriver)colObj0Wrap.CollisionObject.UserObject;
            PhysicsDriver driver1 = (PhysicsDriver)colObj1Wrap.CollisionObject.UserObject;
            driver0.ContactStarted(driver1, cp);
        }
        #endregion

        #region Startup/Shutdown
        /// <summary>
        /// Call this in the program's main method run a game in the engine.
        /// Will create a render form,  initialize the engine, and start the game, so no other methods are needed.
        /// </summary>
        /// <param name="game">The game to play.</param>
        public static void Run(Game game)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RenderForm(game));
        }
        /// <summary>
        /// Call this to shut down the engine, deallocate all resources, and close the application.
        /// </summary>
        public static void CloseApplication()
        {
            ShutDown();
            Application.Exit();
        }
        /// <summary>
        /// Sets the game information for all code to grab preferences from.
        /// Call this BEFORE ANYTHING ELSE!
        /// </summary>
        public static void SetGame(Game game)
        {
            MainThreadID = Thread.CurrentThread.ManagedThreadId;
            _game = game;
        }
        public static void SetGamePanel(RenderPanel panel, bool registerTickNow = true)
        {
            RenderPanel.GamePanel = panel;
            if (registerTickNow)
                panel?.RegisterTick();
        }
        /// <summary>
        /// Initializes the engine to its beginning state.
        /// Call AFTER SetGame is called and all initial render panels are created and ready.
        /// </summary>
        public static void Initialize(bool deferOpeningWorldPlay = false, bool loadOpeningWorldGameMode = true)
        {
            //Analyze computer and determine if it can run what the game wants.
            _computerInfo = ComputerInfo.Analyze();
            
            RenderLibrary = _game.UserSettings.File.RenderLibrary;
            AudioLibrary = _game.UserSettings.File.AudioLibrary;
            InputLibrary = _game.UserSettings.File.InputLibrary;

            //Set initial world (this would generally be a world for opening videos or the main menu)
            SetCurrentWorld(Game.OpeningWorld, true, deferOpeningWorldPlay, loadOpeningWorldGameMode);

            //Preload transition world now
            Task<World> world = Game.TransitionWorld.GetInstanceAsync();
            
            TargetRenderFreq = Settings.CapFPS ? Settings.TargetFPS.ClampMin(1.0f) : 0.0f;
            TargetUpdateFreq = Settings.CapUPS ? Settings.TargetUPS.ClampMin(1.0f) : 0.0f;
        }
        /// <summary>
        /// Call this to stop the engine and dispose of all allocated data.
        /// </summary>
        public static void ShutDown()
        {
            //Steamworks.SteamAPI.Shutdown();
            Stop();
            World = null;
            var files = new List<FileObject>(LoadedFiles.SelectMany(x => x.Value));
            foreach (FileObject o in files)
                o?.Unload();
            var contexts = new List<RenderContext>(RenderContext.BoundContexts);
            foreach (RenderContext c in contexts)
                c?.Dispose();
        }
        #endregion

        public static BaseGameMode ActiveGameMode
        {
            get => Game?.State.ActiveGameMode;
            set
            {
                if (Game != null)
                    Game.State.ActiveGameMode = value;
            }
        }

        private static void ActivePlayers_Removed(LocalPlayerController item)
        {
            ActiveGameMode?.HandleLocalPlayerLeft(item);
        }

        private static void ActivePlayers_Added(LocalPlayerController item)
        {
            ActiveGameMode?.HandleLocalPlayerJoined(item);
        }
        
        #region Timing
        /// <summary>
        /// Starts deployment of update and render ticks.
        /// </summary>
        public static void Run() => _timer.Run();
        /// <summary>
        /// HALTS update and render ticks. Not recommended for use as this literally halts all visuals and user input.
        /// </summary>
        public static void Stop() => _timer.Stop();
        /// <summary>
        /// Registers the given function to be called every update tick.
        /// </summary>
        public static void RegisterRenderTick(EventHandler<FrameEventArgs> func)
            => _timer.RenderFrame += func;
        /// <summary>
        /// Registers the given function to be called every render tick.
        /// </summary>
        public static void UnregisterRenderTick(EventHandler<FrameEventArgs> func)
            => _timer.RenderFrame -= func;
        /// <summary>
        /// Registers a method to execute in a specific order every update tick.
        /// </summary>
        /// <param name="group">The first grouping of when to tick: before, after, or during the physics tick update.</param>
        /// <param name="order">The order to execute the function within its group.</param>
        /// <param name="function">The function to execute per update tick.</param>
        /// <param name="pausedBehavior">If the function should even execute at all, depending on the pause state.</param>
        internal static void RegisterTick(ETickGroup group, ETickOrder order, DelTick function, InputPauseType pausedBehavior = InputPauseType.TickAlways)
        {
            if (function != null)
            {
                var list = GetTickList(group, order, pausedBehavior);
                int tickIndex = (int)group + (int)order + (int)pausedBehavior;
                if (_currentTickList == tickIndex)
                    _tickListQueue.Enqueue(new Tuple<bool, DelTick>(true, function));
                else
                    list.Add(function);
            }
        }
        /// <summary>
        /// Stops running a tick method that was previously registered with the same parameters.
        /// </summary>
        internal static void UnregisterTick(ETickGroup group, ETickOrder order, DelTick function, InputPauseType pausedBehavior = InputPauseType.TickAlways)
        {
            if (function != null)
            {
                var list = GetTickList(group, order, pausedBehavior);
                int tickIndex = (int)group + (int)order + (int)pausedBehavior;
                if (_currentTickList == tickIndex)
                    _tickListQueue.Enqueue(new Tuple<bool, DelTick>(false, function));
                else
                    list.Remove(function);
            }
        }
        /// <summary>
        /// Gets a list of items to tick (in no particular order) that were registered with the following parameters.
        /// </summary>
        private static ThreadSafeList<DelTick> GetTickList(ETickGroup group, ETickOrder order, InputPauseType pausedBehavior)
            => _tickLists[(int)group + (int)order + (int)pausedBehavior];
        /// <summary>
        /// Ticks the before, during, and after physics groups. Also steps the physics simulation during the during physics tick group.
        /// Does not tick physics if paused.
        /// </summary>
        private static void Tick(object sender, FrameEventArgs e)
        {
            float delta = (float)(e.Time * TimeDilation);
            TickGroup(ETickGroup.PrePhysics, delta);
            //using (Task task = new Task(() => TickGroup(ETickGroup.DuringPhysics, delta)))
            //{
                //task.Start();
                if (!_isPaused && World != null)
                    World.StepSimulation(delta);
                //task.Wait();
            //}
            TickGroup(ETickGroup.PostPhysics, delta);
        }
        /// <summary>
        /// Ticks all lists of methods registered to this group.
        /// Goes through timers, input, animation, logic and scene ticks in that order,
        /// and executes methods based on whether the engine is paused or not (or regardless).
        /// </summary>
        private static void TickGroup(ETickGroup group, float delta)
        {
            int start = (int)group;
            for (int i = start; i < start + 15; i += 3)
            {
                for (int j = 0; j < 3; ++j)
                //Parallel.For(0, 3, (int j) => 
                {
                    if (j == 0 || (j == 1 && !IsPaused) || (j == 2 && IsPaused))
                        TickList(i + j, delta);
                }
                //);
            }
        }
        /// <summary>
        /// Ticks the list of items at the given index (created by adding the tick group, order and pause type together).
        /// </summary>
        private static void TickList(int index, float delta)
        {
            ThreadSafeList<DelTick> currentList = _tickLists[_currentTickList = index];

            //Parallel.ForEach(currentList, currentFunc => currentFunc(delta));
            currentList.ForEach(x => x(delta));

            _currentTickList = -1;

            //Add or remove the list of methods that tried to register to or unregister from this group while it was ticking.
            while (!_tickListQueue.IsEmpty && _tickListQueue.TryDequeue(out Tuple<bool, DelTick> result))
            {
                if (result.Item1)
                    currentList.Add(result.Item2);
                else
                    currentList.Remove(result.Item2);
            }
        }
        /// <summary>
        /// Starts a quick timer to track the number of sceonds elapsed.
        /// Returns the id of the timer.
        /// </summary>
        public static int StartTimer()
        {
            int id = _debugTimers.Count;
            _debugTimers.Add(DateTime.Now);
            return id;
        }
        /// <summary>
        /// Ends the timer and returns the amount of time elapsed, in seconds.
        /// </summary>
        /// <param name="id">The id of the timer.</param>
        public static float EndTimer(int id)
        {
            float seconds = (float)(DateTime.Now - _debugTimers[id]).TotalSeconds;
            _debugTimers.RemoveAt(id);
            return seconds;
        }
        /// <summary>
        /// Toggles the pause state. If currently paused, will unpause. If currently unpaused, will pause.
        /// </summary>
        /// <param name="toggler">The player that's pausing the game.</param>
        public static void TogglePause(PlayerIndex toggler)
        {
            SetPaused(!_isPaused, toggler);
        }
        /// <summary>
        /// Sets the pause state regardless of what it is currently.
        /// </summary>
        /// <param name="wantsPause">The desired pause state.</param>
        /// <param name="toggler">The player that's pausing the game.</param>
        public static void SetPaused(bool wantsPause, PlayerIndex toggler, bool force = false)
        {
            if (!force && wantsPause && ActiveGameMode.DisallowPausing)
                return;
            _isPaused = wantsPause;
            Paused?.Invoke(_isPaused, toggler);
        }
        #endregion

        /// <summary>
        /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
        /// </summary>
        public static void LoadCustomFont(string path) => LoadCustomFont(path, Path.GetFileNameWithoutExtension(path));
        /// <summary>
        /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
        /// </summary>
        public static void LoadCustomFont(string path, string fontFamilyName)
        {
            if (!File.Exists(path))
                return;
            string ext = Path.GetExtension(path).ToLowerInvariant().Substring(1);
            if (!(ext.Equals("ttf") || ext.Equals("otf")))
                return;
            _fontIndexMatching[fontFamilyName] = _fontCollection.Families.Length;
            _fontCollection.AddFontFile(path);
        }
        /// <summary>
        /// Gets a custom font family using its name.
        /// </summary>
        /// <param name="fontFamilyName">The name of the font family.</param>
        public static FontFamily GetCustomFontFamily(string fontFamilyName)
            => _fontIndexMatching.ContainsKey(fontFamilyName) ? GetCustomFontFamily(_fontIndexMatching[fontFamilyName]) : null;
        /// <summary>
        /// Gets a custom font family using its load index.
        /// </summary>
        /// <param name="fontFamilyIndex">The index of the font, in the order it was loaded in.</param>
        public static FontFamily GetCustomFontFamily(int fontFamilyIndex)
            => _fontCollection.Families.IndexInRange(fontFamilyIndex) ? _fontCollection.Families[fontFamilyIndex] : null;
        /// <summary>
        /// Retrieves the viewport with the same index.
        /// </summary>
        public static Viewport GetViewport(int index)
            => RenderPanel.GamePanel?.GetViewport(index);
        
        public static void LogError(string message, params string[] args)
        {
#if DEBUG
            if (args.Length != 0)
                message = string.Format(message, args);
            throw new Exception(message);
#endif
        }
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void Print(string message, params string[] args)
        {
#if DEBUG
            if (args.Length != 0)
                message = string.Format(message, args);
            Debug.Write(message);
            DebugOutput?.Invoke(message);
#endif
        }
        /// <summary>
        /// Prints a message for debugging purposes.
        /// </summary>
        public static void PrintLine(string message, params string[] args)
        {
#if DEBUG
            if (args.Length != 0)
                message = string.Format(message, args);
            Debug.WriteLine(message);
            DebugOutput?.Invoke(message + Environment.NewLine);
            //RenderPanel panel = RenderPanel.CapturedPanel;
            //if (panel == null)
            //    return;
            //if (viewport >= 0)
            //{
            //    Viewport v = panel.GetViewport(viewport);
            //    if (v != null)
            //    {
            //        v.DebugPrint(message);
            //        return;
            //    }
            //}
            //else
            //    panel.GlobalHud.DebugPrint(message);
#endif
        }
        /// <summary>
        /// Tells the engine to play in a new world.
        /// </summary>
        /// <param name="world">The world to play in.</param>
        /// <param name="unloadPrevious">Whether or not the engine should deallocate all resources utilized by the current world before loading the new one.</param>
        public static void SetCurrentWorld(World world, bool unloadPrevious = true, bool deferBeginPlay = false, bool loadWorldGameMode = true)
        {
            bool wasRunning = _timer.IsRunning;
            World previous = World;

            DestroyLocalPlayerControllers();
            Game.State.ActiveGameMode?.EndGameplay();
            World?.EndPlay();

            //Stop();
            
            _currentWorld = world;
            Scene.WorldChanged();
            if (World != null)
            {
                World.Initialize();
                if (!deferBeginPlay)
                    World.BeginPlay();
            }

            if (loadWorldGameMode)
                Game.State.ActiveGameMode = World?.GetGameMode();

            Game.State.ActiveGameMode?.BeginGameplay();

            //if (wasRunning)
            //    Run();

            if (unloadPrevious)
                previous?.Unload();
        }
        private static void DestroyLocalPlayerControllers()
        {
            foreach (LocalPlayerController controller in ActivePlayers)
                controller.Destroy();
            ActivePlayers.Clear();
        }

        //public static BaseGameMode ActiveGameMode { get; set; }
        //public static T ActiveGameModeAs<T>() where T : BaseGameMode
        //    => ActiveGameMode as T;

        /// <summary>
        /// Enqueues a pawn to be possessed by the given local player as soon as its current controlled pawn is set to null.
        /// </summary>
        /// <param name="pawn">The pawn to possess.</param>
        /// <param name="possessor">The controller to possess the pawn.</param>
        public static void QueuePossession(IPawn pawn, PlayerIndex possessor)
        {
            int index = (int)possessor;
            if (index < ActivePlayers.Count)
                ActivePlayers[index].EnqueuePosession(pawn);
            else if (_possessionQueues.ContainsKey(possessor))
                _possessionQueues[possessor].Enqueue(pawn);
            else
            {
                Queue<IPawn> queue = new Queue<IPawn>();
                queue.Enqueue(pawn);
                _possessionQueues.Add(possessor, queue);
            }
        }
        internal static void AddLoadedFile<T>(string relativePath, T file) where T : FileObject
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            if (LoadedFiles.ContainsKey(relativePath))
                LoadedFiles[relativePath].Add(file);
            else
                LoadedFiles.Add(relativePath, new List<FileObject>() { file });
        }
        internal static void FoundInput(InputDevice device)
        {
            if (device is CKeyboard || device is CMouse)
            {
                if (ActivePlayers.Count == 0)
                {
                    LocalPlayerController controller;
                    PlayerIndex index = PlayerIndex.One;
                    if (_possessionQueues.ContainsKey(index))
                    {
                        //Transfer possession queue to the controller itself
                        controller = ActiveGameMode?.CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(controller.LocalPlayerIndex);
                    }
                    else
                        controller = ActiveGameMode?.CreateLocalController(index);
                    ActivePlayers.Add(controller);
                }
                else
                    ActivePlayers[0].Input.UpdateDevices();
            }
            else
            {
                if (device.Index >= ActivePlayers.Count)
                {
                    LocalPlayerController controller;
                    PlayerIndex index = (PlayerIndex)ActivePlayers.Count;
                    if (_possessionQueues.ContainsKey(index))
                    {
                        //Transfer possession queue to the controller itself
                        controller = ActiveGameMode?.CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(controller.LocalPlayerIndex);
                    }
                    else
                        controller = ActiveGameMode?.CreateLocalController(index);
                    ActivePlayers.Add(controller);
                }
                else
                    ActivePlayers[device.Index].Input.UpdateDevices();
            }
        }

        #region Tracing
        /// <summary>
        /// Finds the closest ray intersection with any physics object.
        /// </summary>
        /// <returns></returns>
        public static ClosestRayResultCallback RaycastClosest(Segment ray)
            => RaycastClosest(ray.StartPoint, ray.EndPoint);
        public static ClosestRayResultCallback RaycastClosest(Vec3 from, Vec3 to)
        {
            if (World == null)
                return null;
            Vector3 fromRef = from;
            Vector3 toRef = to;
            ClosestRayResultCallback callback = new ClosestRayResultCallback(ref fromRef, ref toRef)
            {
                CollisionFilterMask = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
                CollisionFilterGroup = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
            };
            World.PhysicsScene.RayTest(from, to, callback);
            return callback;
        }
        public static ClosestRayResultExceptCallback RaycastClosestExcept(Segment ray, params CollisionObject[] ignore)
            => RaycastClosestExcept(ray.StartPoint, ray.EndPoint, ignore);
        public static ClosestRayResultExceptCallback RaycastClosestExcept(Vec3 from, Vec3 to, params CollisionObject[] ignore)
        {
            if (World == null)
                return null;
            Vector3 fromRef = from;
            Vector3 toRef = to;
            ClosestRayResultExceptCallback callback = new ClosestRayResultExceptCallback(ref fromRef, ref toRef, ignore)
            {
                CollisionFilterMask = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
                CollisionFilterGroup = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
            };
            World.PhysicsScene.RayTest(from, to, callback);
            return callback;
        }
        public static AllHitsRayResultCallback RaycastMultiple(Segment ray)
            => RaycastMultiple(ray.StartPoint, ray.EndPoint);
        public static AllHitsRayResultCallback RaycastMultiple(Vec3 from, Vec3 to)
        {
            if (World == null)
                return null;
            AllHitsRayResultCallback callback = new AllHitsRayResultCallback(from, to);
            World.PhysicsScene.RayTest(from, to, callback);
            return callback;
        }

        public static void ShapeCastClosest(ConvexShape s, Matrix4 start, Matrix4 end, ClosestConvexResultCallback result)
        {
            World.PhysicsScene.ConvexSweepTest(s, start, end, result);
        }
        #endregion
    }
    public class ClosestConvexResultExceptCallback : ClosestConvexResultCallback
    {
        CollisionObject[] _ignore;
        public ClosestConvexResultExceptCallback(params CollisionObject[] ignore) : base()
            => _ignore = ignore;
        public ClosestConvexResultExceptCallback(ref Vector3 from, ref Vector3 to, params CollisionObject[] ignore) : base(ref from, ref to) 
            => _ignore = ignore;
        public override float AddSingleResult(LocalConvexResult convexResult, bool normalInWorldSpace)
        {
            if (_ignore.Any(x => x == convexResult.HitCollisionObject))
                return 1.0f;
            return base.AddSingleResult(convexResult, normalInWorldSpace);
        }
    }
    public class ClosestRayResultExceptCallback : ClosestRayResultCallback
    {
        CollisionObject[] _ignore;
        public ClosestRayResultExceptCallback(params CollisionObject[] ignore) : base()
            => _ignore = ignore;
        public ClosestRayResultExceptCallback(ref Vector3 from, ref Vector3 to, params CollisionObject[] ignore) : base(ref from, ref to) 
            => _ignore = ignore;
        public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
        {
            if (_ignore.Any(x => x == rayResult.CollisionObject))
                return 1.0f;
            return base.AddSingleResult(rayResult, normalInWorldSpace);
        }
    }
    //public class CustomClosestRayResultCallback : RayResultCallback
    //{
    //    public CustomClosestRayResultCallback() : base()
    //    {

    //    }
    //    public CustomClosestRayResultCallback(Vec3 rayFromWorld, Vec3 rayToWorld) : base()
    //    {

    //    }

    //    private Vec3 _rayStartWorld;
    //    private Vec3 _rayEndWorld;
    //    private Vec3 _hitPointWorld;
    //    private Vec3 _hitNormalWorld;
    //    private float _hitFraction = 1.0f;
    //    private CustomCollisionGroup
    //        _collidesWith = CustomCollisionGroup.All,
    //        //_group = CustomCollisionGroup.All,
    //        _ignore = CustomCollisionGroup.None;

    //    public Vec3 RayStartWorld { get => _rayStartWorld; set => _rayStartWorld = value; }
    //    public Vec3 RayEndWorld { get => _rayEndWorld; set => _rayEndWorld = value; }
    //    public Vec3 HitPointWorld { get => _hitPointWorld; }
    //    public Vec3 HitNormalWorld { get => _hitNormalWorld; }
    //    public CustomCollisionGroup CollidesWith { get => _collidesWith; set => _collidesWith = value; }
    //    public CustomCollisionGroup Ignore { get => _ignore; set => _ignore = value; }

    //    public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
    //    {
    //        if (rayResult.HitFraction < _hitFraction)
    //        {
    //            CollisionObject = rayResult.CollisionObject;
    //            _hitFraction = rayResult.HitFraction;
    //            _hitNormalWorld = normalInWorldSpace ? (Vec3)rayResult.HitNormalLocal : Vec3.TransformNormal(rayResult.HitNormalLocal, rayResult.CollisionObject.WorldTransform).NormalizedFast();
    //            _hitPointWorld = Vec3.Lerp(_rayStartWorld, _rayEndWorld, _hitFraction);
    //        }

    //        return rayResult.HitFraction;
    //    }
    //    public override bool NeedsCollision(BroadphaseProxy proxy0)
    //    {
    //        CustomCollisionGroup g = (CustomCollisionGroup)(short)proxy0.CollisionFilterGroup;
    //        if ((_collidesWith & g) != 0 && (_ignore & g) == 0)
    //            return Collision.SegmentIntersectsAABB(RayStartWorld, RayEndWorld, proxy0.AabbMin, proxy0.AabbMax, out Vec3 enterPoint, out Vec3 exitPoint);
    //        return false;
    //    }
    //}
}
