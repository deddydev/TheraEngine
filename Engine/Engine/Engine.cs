﻿using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using CustomEngine.Rendering;
using CustomEngine.Worlds;
using CustomEngine.Input;
using CustomEngine.Audio;
using CustomEngine.Input.Devices;
using CustomEngine.Input.Devices.OpenTK;
using CustomEngine.Input.Devices.DirectX;

namespace CustomEngine
{
    public static class Engine
    {
        public const string EngineSettingsPath = "\\Config\\EngineSettings.xml";
        public const string UserSettingsPath = "\\Config\\UserSettings.xml";

        public static int PhysicsSubsteps = 10;
        private static ComputerInfo _computerInfo;
        public static List<World> LoadedWorlds = new List<World>();
        public static List<GameTimer> ActiveTimers = new List<GameTimer>();
        public static List<LocalPlayerController> ActivePlayers = new List<LocalPlayerController>();
        public static List<AIController> ActiveAI = new List<AIController>();
        private static World _transitionWorld = null;
        private static World _currentWorld = null;
        private static GlobalTimer _timer = new GlobalTimer();
        private static AbstractRenderer _renderer;
        private static AbstractAudioManager _audioManager;

        public static Dictionary<ETickGroup, Dictionary<ETickOrder, List<ObjectBase>>> _tick = 
            new Dictionary<ETickGroup, Dictionary<ETickOrder, List<ObjectBase>>>();

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
        public static AbstractAudioManager AudioManager
        {
            get { return _audioManager; }
            internal set { _audioManager = value; }
        }

        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }

        /// <summary>
        /// Frames per second that the game will try to render at.
        /// </summary>
        public static double TargetRenderFreq
        {
            get { return _timer.TargetRenderFrequency; }
            set { _timer.TargetRenderFrequency = value; }
        }
        /// <summary>
        /// Frames per second that the game will try to update at.
        /// </summary>
        public static double TargetUpdateFreq
        {
            get { return _timer.TargetUpdateFrequency; }
            set { _timer.TargetUpdateFrequency = value; }
        }
        /// <summary>
        /// How fast/slow the game time looks
        /// </summary>
        public static double TimeDilation
        {
            get { return _timer.TimeDilation; }
            set { _timer.TimeDilation = value; }
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

        /// <summary>
        /// Class containing this computer's specs. Use to adjust engine settings accordingly.
        /// </summary>
        public static ComputerInfo ComputerInfo { get { return _computerInfo; } }
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

        private static RenderLibrary _renderLibrary;
        private static AudioLibrary _audioLibrary;
        private static InputLibrary _inputLibrary;
        public static RenderLibrary RenderLibrary
        {
            get { return _renderLibrary; }
            set
            {
                _renderLibrary = value;
                List<RenderContext> contexts = new List<RenderContext>(RenderContext.BoundContexts);
                foreach (RenderContext c in contexts)
                    c.Control?.SetRenderLibrary();
            }
        }
        public static AudioLibrary AudioLibrary
        {
            get { return _audioLibrary; }
            set { _audioLibrary = value; }
        }
        public static InputLibrary InputLibrary
        {
            get { return _inputLibrary; }
            set
            {
                _inputLibrary = value;
                _inputAwaiter?.Dispose();
                switch (_inputLibrary)
                {
                    case InputLibrary.OpenTK:
                        _inputAwaiter = new TKInputAwaiter(FoundInput);
                        break;
                    case InputLibrary.XInput:
                        _inputAwaiter = new DXInputAwaiter(FoundInput);
                        break;
                }
                //foreach (LocalPlayerController c in ActivePlayers)
                //    c.SetInputLibrary();
            }
        }

        #region Tick        
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
                PhysicsTick(ETickGroup.PrePhysics, delta);
                World.StepSimulation(delta);
                PhysicsTick(ETickGroup.PostPhysics, delta);
            }
            //await t;
        }
        #endregion

        public static void Initialize(World startupWorld)
        {
            _computerInfo = ComputerInfo.Analyze();

            EngineSettings engineSettings = LoadEngineSettings();
            UserSettings userSettings = LoadUserSettings();

            RenderLibrary = userSettings.RenderLibrary;
            AudioLibrary = userSettings.AudioLibrary;
            InputLibrary = userSettings.InputLibrary;

            if (startupWorld == null)
            {
                _currentWorld = new World(engineSettings.OpeningWorldPath);
                _transitionWorld = new World(engineSettings.TransitionWorldPath);

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
            var v = new List<RenderContext>(RenderContext.BoundContexts);
            foreach (RenderContext c in v)
                c?.Dispose();
        }
        public static EngineSettings LoadEngineSettings()
        {
            string path = Application.StartupPath + EngineSettingsPath;
            if (!File.Exists(path))
            {
                EngineSettings settings = new EngineSettings();
                settings.ToXML(path);
                return settings;
            }
            else
                return EngineSettings.FromXML(path);
        }
        public static UserSettings LoadUserSettings()
        {
            string path = Application.StartupPath + UserSettingsPath;
            if (!File.Exists(path))
            {
                UserSettings settings = new UserSettings();
                settings.ToXML(path);
                return settings;
            }
            else
                return UserSettings.FromXML(path);
        }
        public static Viewport GetViewport(int index)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return null;
            return panel.GetViewport(index);
        }
        public static void ShowMessage(string message, int viewport = -1)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return;
            if (viewport >= 0)
                panel.GetViewport(viewport)?.ShowMessage(message);
            else
                panel.GlobalHud.ShowMessage(message);
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
        static InputAwaiter _inputAwaiter;
        internal static void FoundInput(InputDevice device)
        {
            LocalPlayerController controller = new LocalPlayerController();
        }
    }
}