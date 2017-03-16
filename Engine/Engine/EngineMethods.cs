﻿using CustomEngine.Files;
using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Worlds.Actors;
using BulletSharp;

namespace CustomEngine
{
    public static partial class Engine
    {
        static Engine()
        {
            //Steamworks.SteamAPI.Init();
            _timer = new GlobalTimer();
            _timer.UpdateFrame += Tick;
            for (int i = 0; i < 2; ++i)
            {
                ETickGroup order = (ETickGroup)i;
                _tick.Add(order, new Dictionary<ETickOrder, List<ObjectBase>>());
                for (int j = 0; j < 5; ++j)
                    _tick[order].Add((ETickOrder)j, new List<ObjectBase>());
            }
            ActivePlayers.Added += ActivePlayers_Added;
            ActivePlayers.Removed += ActivePlayers_Removed;
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
            ClosestRayResultCallback callback = new ClosestRayResultCallback();
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
        public static void TogglePause()
        {
            SetPaused(!_isPaused);
        }
        public static void SetPaused(bool paused)
        {
            _isPaused = paused;
        }
        public static void Run() => _timer.Run();
        public static void Stop() => _timer.Stop();
        private static void UpdateTick(ETickGroup order, float delta)
        {
            foreach (var g in _tick[order])
            {
                Top:
                int count = g.Value.Count;
                for (int i = 0; i < count; ++i)
                {
                    ObjectBase b = g.Value[i];
                    b.Tick(delta);
                    if (g.Value.Count != count)
                        goto Top;
                }
            }
        }
        //public static async Task AsyncDuringPhysicsTick(float delta)
        //{
        //    foreach (var g in _tick[ETickGroup.DuringPhysics])
        //    {
        //        for (int i = 0; i < g.Value.Count; ++i)
        //        {
        //            ObjectBase b = g.Value[i];
        //            int oldCount = g.Value.Count;
        //            b.Tick(delta);
        //            if (g.Value.Count < oldCount)
        //                --i;

        //            await Task.Delay(100);
        //        }
        //    }
        //}
        public static void RegisterTick(ObjectBase obj, ETickGroup group, ETickOrder order)
        {
            if (obj != null)
            {
                obj.TickGroup = group;
                obj.TickOrder = order;
                if (!_tick[group][order].Contains(obj))
                    _tick[group][order].Add(obj);
            }
        }
        public static void UnregisterTick(ObjectBase obj)
        {
            if (obj != null && obj.TickOrder != null && obj.TickGroup != null)
            {
                ETickOrder order = obj.TickOrder.Value;
                ETickGroup group = obj.TickGroup.Value;
                if (_tick[group][order].Contains(obj))
                    _tick[group][order].Remove(obj);
                obj.TickOrder = null;
                obj.TickGroup = null;
            }
        }
        private static /*async*/ void Tick(object sender, FrameEventArgs e)
        {
            float delta = (float)e.Time;
            //_debugTimers.ForEach(x => x += delta);
            //delta /= PhysicsSubsteps;
            //for (int i = 0; i < 1; i++)
            //{
            UpdateTick(ETickGroup.PrePhysics, delta);
            //Task t = AsyncDuringPhysicsTick(delta);
            if (!_isPaused)
                World.StepSimulation(delta);
            //await t;
            UpdateTick(ETickGroup.PostPhysics, delta);
            //}
        }

        internal static void AddLoadedFile<T>(string relativePath, T file) where T : FileObject
        {
            if (LoadedFiles.ContainsKey(relativePath))
                LoadedFiles[relativePath].Add(file);
            else
                LoadedFiles.Add(relativePath, new List<FileObject>() { file });
        }
        #endregion

        public static void Initialize()
        {
            _computerInfo = ComputerInfo.Analyze();

            EngineSettings engineSettings;
            if (Settings == null)
                _engineSettings.SetFile(engineSettings = new EngineSettings(), true);
            else
                engineSettings = _engineSettings;

            //UserSettings userSettings;
            //if (_userSettings.File == null)
            //    _userSettings.SetFile(userSettings = new UserSettings(), true);
            //else
            //    userSettings = _userSettings;

            RenderLibrary = RenderLibrary.OpenGL;
            AudioLibrary = AudioLibrary.OpenAL;
            InputLibrary = InputLibrary.OpenTK;

            if (Renderer == null)
                return;

            //RenderLibrary = userSettings.RenderLibrary;
            //AudioLibrary = userSettings.AudioLibrary;
            //InputLibrary = userSettings.InputLibrary;

            World = engineSettings.OpeningWorld;
            _transitionWorld = engineSettings.TransitionWorld;
            
            TargetRenderFreq = TARGETFPS;
            TargetUpdateFreq = TARGETUPS;
            Run();
        }
        public static void ShutDown()
        {
            //Steamworks.SteamAPI.Shutdown();
            Stop();
            var files = new List<FileObject>(LoadedFiles.SelectMany(x => x.Value));
            foreach (FileObject o in files)
                o?.Unload();
            var contexts = new List<RenderContext>(RenderContext.BoundContexts);
            foreach (RenderContext c in contexts)
                c?.Dispose();
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
        internal static void QueueCollisionSpawn(PhysicsDriver driver)
        {
            _queuedCollisions.Add(driver);
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
