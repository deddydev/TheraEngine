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
        public static void Initialize(Game game)
        {
            _game = game;
            _computerInfo = ComputerInfo.Analyze();

            RenderLibrary = game.UserSettings.RenderLibrary;
            AudioLibrary = game.UserSettings.AudioLibrary;
            InputLibrary = game.UserSettings.InputLibrary;
            
            World = Game.OpeningWorld;
            Game.TransitionWorld.GetInstance();

            TargetRenderFreq = Settings.CapFPS ? Settings.TargetFPS.ClampMin(1.0f) : 0.0f;
            TargetUpdateFreq = Settings.CapUPS ? Settings.TargetUPS.ClampMin(1.0f) : 0.0f;
            Run();
        }
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
            _isPaused = paused;
            Paused?.Invoke(_isPaused, toggler);
        }

        #region Tick
        public static void Run() => _timer.Run();
        public static void Stop() => _timer.Stop();
        public static void RegisterRenderTick(EventHandler<FrameEventArgs> func)
            => _timer.RenderFrame += func;
        public static void UnregisterRenderTick(EventHandler<FrameEventArgs> func)
            => _timer.RenderFrame -= func;
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
            float delta = (float)e.Time;
            TickGroup(ETickGroup.PrePhysics, delta);
            if (!_isPaused && World != null)
                World.StepSimulation(delta * (float)TimeDilation);
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

        public static void LoadFont(string path)
        {
            if (!File.Exists(path))
                return;
            string ext = Path.GetExtension(path).ToLower().Substring(1);
            if (!(ext.Equals("ttf") || ext.Equals("otf")))
                return;
            _fontCollection.AddFontFile(path);
        }

        public static Viewport GetViewport(int index)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return null;
            return panel.GetViewport(index);
        }
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
        public static void SetCurrentWorld(World world, bool unloadPrevious)
        {
            World previous = World;
            World?.EndPlay();
            _currentWorld = world;
            Scene.WorldChanged();
            World?.BeginPlay();
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
                        controller = new LocalPlayerController(_possessionQueues[index]);
                        _possessionQueues.Remove(controller.LocalPlayerIndex);
                    }
                    else
                        controller = new LocalPlayerController();
                    World?.OnLocalPlayerAdded(controller);
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
                        controller = new LocalPlayerController(_possessionQueues[index]);
                        _possessionQueues.Remove(controller.LocalPlayerIndex);
                    }
                    else
                        controller = new LocalPlayerController();
                    World?.OnLocalPlayerAdded(controller);
                }
                else
                    ActivePlayers[device.Index].Input.UpdateDevices();
            }
        }
    }
}
