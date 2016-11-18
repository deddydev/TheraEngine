using CustomEngine.Files;
using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;

namespace CustomEngine
{
    public static partial class Engine
    {
        static Engine()
        {
            _timer = new GlobalTimer();
            _timer.UpdateFrame += Tick;
            for (int i = 0; i < 2; ++i)
            {
                ETickGroup order = (ETickGroup)i;
                _tick.Add(order, new Dictionary<ETickOrder, List<ObjectBase>>());
                for (int j = 0; j < 4; ++j)
                    _tick[order].Add((ETickOrder)j, new List<ObjectBase>());
            }
            ActivePlayers.Added += ActivePlayers_Added;
            ActivePlayers.Removed += ActivePlayers_Removed;
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
        public static int StartDebugTimer()
        {
            int id = _debugTimers.Count;
            _debugTimers.Add(0);
            return id;
        }
        public static float EndDebugTimer(int id)
        {
            float seconds = _debugTimers[id];
            _debugTimers.RemoveAt(id);
            return seconds;
        }
        #region Tick
        public static void Pause()
        {

        }
        public static void Run() { _timer.Run(); }
        public static void Stop() { _timer.Stop(); }
        private static void PhysicsTick(ETickGroup order, float delta)
        {
            foreach (var g in _tick[order])
                for (int i = 0; i < g.Value.Count; ++i)
                {
                    ObjectBase b = g.Value[i];
                    int oldCount = g.Value.Count;
                    b.Tick(delta);
                    if (g.Value.Count < oldCount)
                        --i;
                }
        }
        public static async Task AsyncDuringPhysicsTick(float delta)
        {
            foreach (var g in _tick[ETickGroup.DuringPhysics])
            {
                for (int i = 0; i < g.Value.Count; ++i)
                {
                    ObjectBase b = g.Value[i];
                    int oldCount = g.Value.Count;
                    b.Tick(delta);
                    if (g.Value.Count < oldCount)
                        --i;

                    await Task.Delay(100);
                }
            }
        }
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
            _debugTimers.ForEach(x => x += delta);
            //delta /= PhysicsSubsteps;
            //for (int i = 0; i < 1; i++)
            //{
                PhysicsTick(ETickGroup.PrePhysics, delta);
                //Task t = AsyncDuringPhysicsTick(delta);
                World.StepSimulation(delta);
                //await t;
                PhysicsTick(ETickGroup.PostPhysics, delta);
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
            if (_engineSettings.File == null)
                _engineSettings.SetFile(engineSettings = new EngineSettings(), true);
            else
                engineSettings = _engineSettings;

            UserSettings userSettings;
            if (_userSettings.File == null)
                _userSettings.SetFile(userSettings = new UserSettings(), true);
            else
                userSettings = _userSettings;

            RenderLibrary = userSettings.RenderLibrary;
            AudioLibrary = userSettings.AudioLibrary;
            InputLibrary = userSettings.InputLibrary;

            World = engineSettings.OpeningWorld;
            _transitionWorld = engineSettings.TransitionWorld;

            TargetRenderFreq = 60.0f;
            TargetUpdateFreq = 90.0f;
            Run();
        }
        public static void ShutDown()
        {
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
                Viewport port = panel.GetViewport(viewport);
                if (port != null)
                {
                    port.DebugPrint(message);
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
        public static void QueuePossession(Pawn pawn, PlayerIndex possessor)
        {
            int index = (int)possessor;
            if (index < ActivePlayers.Count)
                ActivePlayers[index].EnqueuePosession(pawn);
            else if (_possessionQueue.ContainsKey(possessor))
                _possessionQueue[possessor].Enqueue(pawn);
            else
            {
                Queue<Pawn> queue = new Queue<Pawn>();
                queue.Enqueue(pawn);
                _possessionQueue.Add(possessor, queue);
            }
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
