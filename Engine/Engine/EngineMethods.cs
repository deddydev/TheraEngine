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
using TheraEngine.Players;
using System.Collections.Concurrent;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace TheraEngine
{
    public class CustomClosestRayResultCallback : RayResultCallback
    {
        public CustomClosestRayResultCallback() : base()
        {

        }
        public CustomClosestRayResultCallback(Vec3 rayFromWorld, Vec3 rayToWorld) : base()
        {

        }

        private Vec3 _rayStartWorld;
        private Vec3 _rayEndWorld;
        private Vec3 _hitPointWorld;
        private Vec3 _hitNormalWorld;
        private float _hitFraction = 1.0f;
        private CustomCollisionGroup
            _collidesWith = CustomCollisionGroup.All,
            //_group = CustomCollisionGroup.All,
            _ignore = CustomCollisionGroup.None;

        public Vec3 RayStartWorld { get => _rayStartWorld; set => _rayStartWorld = value; }
        public Vec3 RayEndWorld { get => _rayEndWorld; set => _rayEndWorld = value; }
        public Vec3 HitPointWorld { get => _hitPointWorld; }
        public Vec3 HitNormalWorld { get => _hitNormalWorld; }
        public CustomCollisionGroup CollidesWith { get => _collidesWith; set => _collidesWith = value; }
        public CustomCollisionGroup Ignore { get => _ignore; set => _ignore = value; }

        public override float AddSingleResult(LocalRayResult rayResult, bool normalInWorldSpace)
        {
            if (rayResult.HitFraction < _hitFraction)
            {
                CollisionObject = rayResult.CollisionObject;
                _hitFraction = rayResult.HitFraction;
                _hitNormalWorld = normalInWorldSpace ? (Vec3)rayResult.HitNormalLocal : Vec3.TransformNormal(rayResult.HitNormalLocal, rayResult.CollisionObject.WorldTransform).NormalizedFast();
                _hitPointWorld = Vec3.Lerp(_rayStartWorld, _rayEndWorld, _hitFraction);
            }

            return rayResult.HitFraction;
        }
        public override bool NeedsCollision(BroadphaseProxy proxy0)
        {
            CustomCollisionGroup g = (CustomCollisionGroup)(short)proxy0.CollisionFilterGroup;
            if ((_collidesWith & g) != 0 && (_ignore & g) == 0)
                return Collision.SegmentIntersectsAABB(RayStartWorld, RayEndWorld, proxy0.AabbMin, proxy0.AabbMax, out Vec3 enterPoint, out Vec3 exitPoint);
            return false;
        }
    }
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

            Thread.CurrentThread.Name = "Main Thread";
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
            _game = game;
        }
        /// <summary>
        /// Initializes the engine to its beginning state.
        /// Call AFTER SetGame is called and all initial render panels are created and ready.
        /// </summary>
        public static void Initialize()
        {
            MainThreadID = Thread.CurrentThread.ManagedThreadId;

            //Analyze computer and determine if it can run what the game wants.
            _computerInfo = ComputerInfo.Analyze();
            
            RenderLibrary = _game.UserSettings.RenderLibrary;
            AudioLibrary = _game.UserSettings.AudioLibrary;
            InputLibrary = _game.UserSettings.InputLibrary;

            if (Renderer == null)
                throw new Exception("Unable to create renderer.");

            //Set initial world (this would generally be a world for opening videos or the main menu)
            World = Game.OpeningWorld;

            //Preload loading world now
            Game.TransitionWorld.GetInstance();
            
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

        public static ClosestConvexResultCallback ShapeCastClosest(ConvexShape s, Matrix4 start, Matrix4 end)
        {
            ClosestConvexResultCallback callback = new ClosestConvexResultCallback()
            {
                CollisionFilterMask = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
                CollisionFilterGroup = (CollisionFilterGroups)(short)CustomCollisionGroup.All,
            };
            World.PhysicsScene.ConvexSweepTest(s, start, end, callback);
            return callback;
        }

        private static void ActivePlayers_Removed(LocalPlayerController item)
        {
            World?.GetGameMode()?.HandleLocalPlayerLeft(item);
        }

        private static void ActivePlayers_Added(LocalPlayerController item)
        {
            World?.GetGameMode()?.HandleLocalPlayerJoined(item);
            if (CurrentPanel != null)
            {
                Viewport v = CurrentPanel.GetViewport((int)item.LocalPlayerIndex) ?? CurrentPanel.AddViewport();
                if (v != null)
                    v.Owner = item;
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

        public static void TogglePause(PlayerIndex toggler)
        {
            SetPaused(!_isPaused, toggler);
        }
        public static void SetPaused(bool paused, PlayerIndex toggler)
        {
            if (!World.Settings.GameMode.File.AllowPausing)
                return;
            _isPaused = paused;
            Paused?.Invoke(_isPaused, toggler);
        }

        #region Update Tick
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
        /// Registers a function to execute in a specific order every update tick.
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
        private static ThreadSafeList<DelTick> GetTickList(ETickGroup group, ETickOrder order, InputPauseType pausedBehavior)
            => _tickLists[(int)group + (int)order + (int)pausedBehavior];
        private static void Tick(object sender, FrameEventArgs e)
        {
            float delta = (float)(e.Time * TimeDilation);
            TickGroup(ETickGroup.PrePhysics, delta);
            if (!_isPaused && World != null)
                World.StepSimulation(delta);
            TickGroup(ETickGroup.PostPhysics, delta);
        }
        private static void TickGroup(ETickGroup group, float delta)
        {
            int start = (int)group;
            for (int i = start; i < start + 15; i += 3)
            {
                Parallel.For(0, 3, (int j) => 
                {
                    if (j == 0 || (j == 1 && !IsPaused) || (j == 2 && IsPaused))
                        TickList(i + j, delta);
                });
            }
        }
        private static void TickList(int index, float delta)
        {
            ThreadSafeList<DelTick> currentList = _tickLists[_currentTickList = index];

            Parallel.ForEach(currentList, currentFunc => currentFunc(delta));

            //foreach (var currentFunc in currentList)
            //    currentFunc(delta);

            _currentTickList = -1;

            while (!_tickListQueue.IsEmpty && _tickListQueue.TryDequeue(out Tuple<bool, DelTick> result))
            {
                if (result.Item1)
                    currentList.Add(result.Item2);
                else
                    currentList.Remove(result.Item2);
            }
        }
        #endregion

        /// <summary>
        /// Loads a ttf or otf font from the given path and adds it to the collection of fonts.
        /// </summary>
        public static void LoadFont(string path)
        {
            if (!File.Exists(path))
                return;
            string ext = Path.GetExtension(path).ToLower().Substring(1);
            if (!(ext.Equals("ttf") || ext.Equals("otf")))
                return;
            _fontCollection.AddFontFile(path);
        }
        /// <summary>
        /// Retrieves the viewport with the same index.
        /// </summary>
        public static Viewport GetViewport(int index)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return null;
            return panel.GetViewport(index);
        }
#if DEBUG
        /// <summary>
        /// Prints a message to the top left of the screen, for debugging purposes.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="viewport"></param>
        public static void DebugPrint(string message, int viewport = -1)
        {
            Debug.WriteLine(message);
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return;
            if (viewport >= 0)
            {
                Viewport v = panel.GetViewport(viewport);
                if (v != null)
                {
                    v.DebugPrint(message);
                    return;
                }
            }
            panel.GlobalHud.DebugPrint(message);
        }
#endif
        public static void SetCurrentWorld(World world, bool unloadPrevious)
        {
            World previous = World;
            if (World != null)
                World.EndPlay();
            _currentWorld = world;
            Scene.WorldChanged();
            if (World != null)
                World.BeginPlay();
            if (unloadPrevious)
                previous?.Unload();
        }
        
        internal static void QueuePossession(IPawn pawn, PlayerIndex possessor)
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
                        controller = World.Settings.GameMode.File?.CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(controller.LocalPlayerIndex);
                    }
                    else
                        controller = World.Settings.GameMode.File?.CreateLocalController(index);
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
                        controller = World.Settings.GameMode.File?.CreateLocalController(index, _possessionQueues[index]);
                        _possessionQueues.Remove(controller.LocalPlayerIndex);
                    }
                    else
                        controller = World.Settings.GameMode.File?.CreateLocalController(index);
                    ActivePlayers.Add(controller);
                }
                else
                    ActivePlayers[device.Index].Input.UpdateDevices();
            }
        }
    }
}
