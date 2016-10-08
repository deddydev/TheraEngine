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

        public static Dictionary<TickOrder, Dictionary<TickGroup, List<ObjectBase>>> _tick = 
            new Dictionary<TickOrder, Dictionary<TickGroup, List<ObjectBase>>>();
        
        static Engine()
        {
            for (int i = 0; i < 2; ++i)
            {
                TickOrder order = (TickOrder)i;
                _tick.Add(order, new Dictionary<TickGroup, List<ObjectBase>>());
                for (int j = 0; j < 4; ++j)
                    _tick[order].Add((TickGroup)j, new List<ObjectBase>());
            }
        }

        public static AbstractRenderer Renderer { get { return _renderer; } set { _renderer = value; } }
        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }
        [Default]
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
        public static void Run() { _timer.Run(UpdateRate, RenderRate); }
        public static void Stop() { _timer.Stop(); }
        private static void PhysicsTick(TickOrder order, float delta)
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
                if (!_tick[order][group].Contains(obj))
                    _tick[order][group].Add(obj);
            }
        }
        public static void UnregisterTick(ObjectBase obj)
        {
            if (obj.TickOrder != null && obj.TickGroup != null)
            {
                TickOrder order = obj.TickOrder.Value;
                TickGroup group = obj.TickGroup.Value;
                if (_tick[order][group].Contains(obj))
                    _tick[order][group].Remove(obj);
            }
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
                PhysicsTick(TickOrder.PrePhysics, delta);
                World.StepSimulation(delta);
                PhysicsTick(TickOrder.PostPhysics, delta);
            }
            //await t;
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
        public static async void SetCurrentWorld(World world)
        {
            Task load = null;
            if (world != null && !world.IsLoaded)
                load = world.Load();
            World = world;
            if (load != null)
                await load;
        }
    }
}