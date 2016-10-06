using CustomEngine.Worlds;
using System;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using System.Collections.Generic;

namespace CustomEngine
{
    public enum TickGroup
    {
        Timers = 0,
        Input = 1,
        Logic = 2,
        Scene = 3,
    }
    public static class Engine
    {
        public const string SettingsPath = "/Config/EngineSettings.xml";

        const float UpdateRate = 120.0f;
        const float RenderRate = 60.0f;
        public static int PhysicsSubsteps = 10;

        public static Dictionary<TickGroup, List<ObjectBase>> _prePhysicsTick = new Dictionary<TickGroup, List<ObjectBase>>();
        public static Dictionary<TickGroup, List<ObjectBase>> _postPhysicsTick = new Dictionary<TickGroup, List<ObjectBase>>();
        
        static Engine()
        {
            _prePhysicsTick.Add(TickGroup.Timers, new List<ObjectBase>());
            _prePhysicsTick.Add(TickGroup.Input, new List<ObjectBase>());
            _prePhysicsTick.Add(TickGroup.Logic, new List<ObjectBase>());
            _prePhysicsTick.Add(TickGroup.Scene, new List<ObjectBase>());

            _postPhysicsTick.Add(TickGroup.Timers, new List<ObjectBase>());
            _postPhysicsTick.Add(TickGroup.Input, new List<ObjectBase>());
            _postPhysicsTick.Add(TickGroup.Logic, new List<ObjectBase>());
            _postPhysicsTick.Add(TickGroup.Scene, new List<ObjectBase>());
        }

        public static AbstractRenderer Renderer { get { return _renderer; } set { _renderer = value; } }
        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }
        public static World TransitionWorld { get { return _transitionWorld; } set { _transitionWorld = value; } }
        public static World World
        {
            get { return _currentWorld; }
            set { _currentWorld = value; }
        }
        
        public static BindingList<World> LoadedWorlds = new BindingList<World>();
        public static BindingList<GameTimer> ActiveTimers = new BindingList<GameTimer>();
        public static BindingList<PlayerController> ActivePlayers = new BindingList<PlayerController>();
        public static BindingList<AIController> ActiveAI = new BindingList<AIController>();

        public static void RemakePlayerNumbers()
        {
            int i = 0;
            foreach (PlayerController pc in ActivePlayers)
                pc._number = i++;
        }

        private static World _transitionWorld = null;
        private static World _currentWorld = null;
        private static GlobalTimer _timer = new GlobalTimer();
        private static AbstractRenderer _renderer;

        public static RenderPanel CurrentPanel
        {
            get
            {
                RenderWindowContext ctx = RenderWindowContext.CurrentContext;
                if (ctx != null)
                    return ctx.Control;
                return null;
            }
        }
        private static void PrePhysicsTick(float delta)
        {
            foreach (TickGroup g in _prePhysicsTick.Keys)
                foreach (ObjectBase b in _prePhysicsTick[g])
                    b.Tick(delta);
        }
        private static void PostPhysicsTick(float delta)
        {
            foreach (TickGroup g in _postPhysicsTick.Keys)
                foreach (ObjectBase b in _postPhysicsTick[g])
                    b.Tick(delta);
        }
        public static void RegisterTick(ObjectBase obj)
        {
            if (obj.TickOrder != null && obj.TickGroup != null)
            {
                TickGroup group = obj.TickGroup.Value;
                switch (obj.TickOrder)
                {
                    case TickOrder.PrePhysics:
                        if (!_prePhysicsTick[group].Contains(obj))
                            _prePhysicsTick[group].Add(obj);
                        break;
                    case TickOrder.PostPhysics:
                        if (!_postPhysicsTick[group].Contains(obj))
                            _postPhysicsTick[group].Add(obj);
                        break;
                }
            }
        }
        public static void UnregisterTick(ObjectBase obj)
        {
            if (obj.TickOrder != null && obj.TickGroup != null)
            {
                TickGroup group = obj.TickGroup.Value;
                switch (obj.TickOrder)
                {
                    case TickOrder.PrePhysics:
                        if (_prePhysicsTick[group].Contains(obj))
                            _prePhysicsTick[group].Remove(obj);
                        break;
                    case TickOrder.PostPhysics:
                        if (_postPhysicsTick[group].Contains(obj))
                            _postPhysicsTick[group].Remove(obj);
                        break;
                }
            }
        }
        public static void Initialize()
        {
            EngineSettings s = LoadSettings();

            _currentWorld = new World(s._openingWorldPath);
            _transitionWorld = new World(s._transitionWorldPath);

            var task = _transitionWorld.Load();
            Task.Run(_currentWorld.Load);
            Run();
            task.Wait();
        }
        public static EngineSettings LoadSettings()
        {
            if (!File.Exists(SettingsPath))
            {
                EngineSettings settings = new EngineSettings();
                settings.SaveXML(SettingsPath);
                return settings;
            }
            else
                return EngineSettings.FromXML(SettingsPath);
        }
        public static void Run() { _timer.Run(UpdateRate, RenderRate); }
        public static void Stop() { _timer.Stop(); }
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
        public static void Tick(float delta)
        {
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
                PrePhysicsTick(delta);
                World.StepSimulation(delta);
                PostPhysicsTick(delta);
            }
            //await t;
        }
        public static async void SetCurrentWorld(World world)
        {
            if (world != null && !world.IsLoaded)
                await Task.Run(world.Load);
            World = world;
        }
    }
}