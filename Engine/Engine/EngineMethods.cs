using CustomEngine.Files;
using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Worlds.Actors;
using BulletSharp;
using System.Threading.Tasks;
using CustomEngine.Players;
using System.Collections.Concurrent;
using System.Threading;

namespace CustomEngine
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

            _tickLists = new ThreadSafeList<DelTick>[15];
            for (int i = 0; i < _tickLists.Length; ++i)
                _tickLists[i] = new ThreadSafeList<DelTick>();

            Thread.CurrentThread.Name = "Main Thread";
        }
        public static void Initialize()
        {
            _computerInfo = ComputerInfo.Analyze();

            RenderLibrary = RenderLibrary.OpenGL; //UserSettings.RenderLibrary;
            AudioLibrary = AudioLibrary.OpenAL; //UserSettings.AudioLibrary;
            InputLibrary = InputLibrary.OpenTK; //UserSettings.InputLibrary;

            if (Renderer == null)
                throw new Exception("Unable to create a renderer.");

            World = Settings.OpeningWorld;
            Settings.TransitionWorld.GetInstance();

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
            
        }

        private static void ActivePlayers_Added(LocalPlayerController item)
        {

        }
        public static void RegisterRenderTick(EventHandler<FrameEventArgs> func)
        {
            _timer.RenderFrame += func;
        }
        public static void UnregisterRenderTick(EventHandler<FrameEventArgs> func)
        {
            _timer.RenderFrame -= func;
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

        #region Tick
        public static void TogglePause(PlayerIndex toggler)
        {
            SetPaused(!_isPaused, toggler);
        }
        public static void SetPaused(bool paused, PlayerIndex toggler)
        {
            _isPaused = paused;
        }
        public static void Run() => _timer.Run();
        public static void Stop() => _timer.Stop();
        internal static void RegisterTick(ETickGroup group, ETickOrder order, DelTick function)
        {
            if (function != null)
            {
                var list = GetTickList(group, order);
                int tickIndex = (int)group + (int)order;
                if (_currentTickList == tickIndex)
                    _tickListQueue.Enqueue(new Tuple<bool, DelTick>(true, function));
                else
                    list.Add(function);
            }
        }
        internal static void UnregisterTick(ETickGroup group, ETickOrder order, DelTick function)
        {
            if (function != null)
            {
                var list = GetTickList(group, order);
                int tickIndex = (int)group + (int)order;
                if (_currentTickList == tickIndex)
                    _tickListQueue.Enqueue(new Tuple<bool, DelTick>(false, function));
                else
                    list.Remove(function);
            }
        }
        private static ThreadSafeList<DelTick> GetTickList(ETickGroup group, ETickOrder order)
            => _tickLists[(int)group + (int)order];
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
            ThreadSafeList<DelTick> currentList;
            for (int i = start; i < start + 5; ++i)
            {
                currentList = _tickLists[_currentTickList = i];

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
        }
        #endregion

        public static Viewport GetViewport(int index)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return null;
            return panel.GetViewport(index);
        }
        public static void DebugPrint(string message, int viewport = -1)
        {
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
            Renderer.Scene.WorldChanged();
            World?.BeginPlay();
            if (unloadPrevious)
                previous?.Unload();
        }
        internal static void QueuePossession(IPawn pawn, PlayerIndex possessor)
        {
            int index = (int)possessor;
            if (index < ActivePlayers.Count)
                ActivePlayers[index].EnqueuePosession(pawn);
            else if (_possessionQueue.ContainsKey(possessor))
                _possessionQueue[possessor].Enqueue(pawn);
            else
            {
                Queue<IPawn> queue = new Queue<IPawn>();
                queue.Enqueue(pawn);
                _possessionQueue.Add(possessor, queue);
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
            if (device.Index >= ActivePlayers.Count)
            {
                LocalPlayerController controller;
                PlayerIndex index = (PlayerIndex)ActivePlayers.Count;
                if (_possessionQueue.ContainsKey(index))
                {
                    controller = new LocalPlayerController(_possessionQueue[index]);
                    _possessionQueue.Remove(controller.LocalPlayerIndex);
                }
                else
                    controller = new LocalPlayerController();
            }
            else
                ActivePlayers[device.Index].Input.UpdateDevices();
        }
    }
}
