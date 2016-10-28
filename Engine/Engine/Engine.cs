using CustomEngine.Worlds;
using System;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;
using CustomEngine.Audio;
using CustomEngine.Worlds.Maps;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Rendering.Models;

namespace CustomEngine
{
    public static class Engine
    {
        public const string SettingsPath = "/Config/EngineSettings.xml";

        public static int PhysicsSubsteps = 10;
        private static ComputerInfo _computerInfo;

        public static Dictionary<TickGroup, Dictionary<TickOrder, List<ObjectBase>>> _tick = 
            new Dictionary<TickGroup, Dictionary<TickOrder, List<ObjectBase>>>();

        static Engine()
        {
            _timer = new GlobalTimer();
            _timer.UpdateFrame += Tick;
            for (int i = 0; i < 2; ++i)
            {
                TickGroup order = (TickGroup)i;
                _tick.Add(order, new Dictionary<TickOrder, List<ObjectBase>>());
                for (int j = 0; j < 4; ++j)
                    _tick[order].Add((TickOrder)j, new List<ObjectBase>());
            }
        }

        public static void RegisterRenderTick(EventHandler<FrameEventArgs> func)
        {
            _timer.RenderFrame += func;
        }

        public static AbstractRenderer Renderer
        {
            get { return _renderer; }
            internal set { _renderer = value; }
        }
        public static AbstractAudioManager AudioManager { get { return _audioManager; } set { _audioManager = value; } }

        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }

        public static double TargetRenderFreq
        {
            get { return _timer.TargetRenderFrequency; }
            set { _timer.TargetRenderFrequency = value; }
        }
        public static double TargetUpdateFreq
        {
            get { return _timer.TargetUpdateFrequency; }
            set { _timer.TargetUpdateFrequency = value; }
        }

        [Default]
        public static World TransitionWorld
        {
            get { return _transitionWorld; }
            set { _transitionWorld = value; }
        }
        [State]
        public static World World
        {
            get { return _currentWorld; }
            set { SetCurrentWorld(value); }
        }
        
        public static BindingList<World> LoadedWorlds = new BindingList<World>();
        public static BindingList<GameTimer> ActiveTimers = new BindingList<GameTimer>();
        public static BindingList<PlayerController> ActivePlayers = new BindingList<PlayerController>();
        public static BindingList<AIController> ActiveAI = new BindingList<AIController>();

        private static World _transitionWorld = null;
        private static World _currentWorld = null;
        private static GlobalTimer _timer = new GlobalTimer();
        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine settings accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo { get { return _computerInfo; } }
        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static double RenderRate
        {
            get { return _timer.TargetRenderFrequency; }
            set { _timer.TargetRenderFrequency = value; }
        }
        /// <summary>
        /// Updates per second that the game will try to run logic at.
        /// </summary>
        public static double UpdateRate
        {
            get { return _timer.TargetRenderFrequency; }
            set { _timer.TargetRenderFrequency = value; }
        }
        public static RenderPanel CurrentPanel
        {
            get
            {
                RenderContext ctx = RenderContext.Current;
                if (ctx != null)
                    return ctx.Control;
                return null;
            }
        }
        public static void Run() { _timer.Run(); }
        public static void Stop() { _timer.Stop(); }

        #region Tick
        private static void PhysicsTick(TickGroup order, float delta)
        {
            foreach (var g in _tick[order])
                foreach (ObjectBase b in g.Value)
                    b.Tick(delta);
        }
        public static void RegisterTick(ObjectBase obj)
        {
            if (obj.TickOrder != null && obj.TickGroup != null)
            {
                TickOrder order = obj.TickOrder.Value;
                TickGroup group = obj.TickGroup.Value;
                if (!_tick[group][order].Contains(obj))
                    _tick[group][order].Add(obj);
            }
            else
                UnregisterTick(obj);
        }
        public static void UnregisterTick(ObjectBase obj)
        {
            if (obj.TickOrder != null && obj.TickGroup != null)
            {
                TickOrder order = obj.TickOrder.Value;
                TickGroup group = obj.TickGroup.Value;
                if (_tick[group][order].Contains(obj))
                    _tick[group][order].Remove(obj);
            }
        }
        private static void Tick(object sender, FrameEventArgs e)
        {
            float delta = (float)e.Time;
            foreach (GameTimer timer in ActiveTimers)
                timer.Tick(delta);
            foreach (PlayerController c in ActivePlayers)
                c.Tick(delta);
            foreach (AIController c in ActiveAI)
                c.Tick(delta);
            //Task t = DuringPhysics(delta);
            delta /= PhysicsSubsteps;
            for (int i = 0; i < PhysicsSubsteps; i++)
            {
                PhysicsTick(TickGroup.PrePhysics, delta);
                World.StepSimulation(delta);
                PhysicsTick(TickGroup.PostPhysics, delta);
            }
            //await t;
        }
        #endregion

        public static void Initialize(World startupWorld)
        {
            _computerInfo = ComputerInfo.Analyze();

            EngineSettings s = LoadSettings();

            if (startupWorld == null)
            {
                _currentWorld = new World(s.OpeningWorldPath);
                _transitionWorld = new World(s.TransitionWorldPath);

                _transitionWorld.Load();
                _currentWorld.Load();
            }
            else
                _currentWorld = startupWorld;

            TargetRenderFreq = 60.0f;
            TargetUpdateFreq = 90.0f;
            Run();
        }
        public static void ShutDown()
        {
            Stop();
            var v = RenderContext.BoundContexts;
            foreach (RenderContext c in v)
                c?.Dispose();
        }
        public static EngineSettings LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                EngineSettings settings = new EngineSettings();
                settings.ToXML(SettingsPath);
                return settings;
            }
            else
                return EngineSettings.FromXML(SettingsPath);
        }
        public static void RemakePlayerNumbers()
        {
            int i = 0;
            foreach (PlayerController pc in ActivePlayers)
                pc._number = i++;
        }
        public static void ShowMessage(string message, int viewport = -1)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return;
            if (viewport >= 0)
                panel.GetViewport(viewport)?.ShowMessage(message);
            else
                panel._overallHud.ShowMessage(message);
        }
        public static void SetCurrentWorld(World world)
        {
            World oldCurrent = _currentWorld;
            if (world != null && !world.IsLoaded)
                world.Load();
            _currentWorld = world;
            if (oldCurrent != null && oldCurrent.IsLoaded)
                oldCurrent.Unload();
        }
    }
}