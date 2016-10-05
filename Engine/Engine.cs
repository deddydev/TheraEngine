using CustomEngine.Worlds;
using System;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using CustomEngine.Files;

namespace CustomEngine
{
    public static class Engine
    {
        public const string SettingsPath = "/Config/EngineSettings.xml";

        const float UpdateRate = 0.0f;
        const float RenderRate = 60.0f;

        public static AbstractRenderer Renderer { get { return _renderer; } set { _renderer = value; } }

        public static void Initialize()
        {
            LoadDefaults();
            Run();
        }

        private static AbstractRenderer _renderer;

        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }
        private static GlobalTimer _timer = new GlobalTimer();

        public static World TransitionWorld { get { return _transitionWorld; } set { _transitionWorld = value; } }
        private static World _transitionWorld = null;
        public static World World
        {
            get { return _currentWorld; }
            set { _currentWorld = value; }
        }
        private static World _currentWorld = null;

        public static BindingList<World> LoadedWorlds = new BindingList<World>();
        public static BindingList<GameTimer> ActiveTimers = new BindingList<GameTimer>();
        public static BindingList<PawnController> ActiveControllers = new BindingList<PawnController>();
        internal static int PhysicsSubsteps = 10;

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
        public static void LoadDefaults()
        {
            EngineSettings settings = null;
            if (!File.Exists(SettingsPath))
            {
                settings = new EngineSettings();
                settings.SaveXML(SettingsPath);
            }
            else
                settings = EngineSettings.FromXML(SettingsPath);

            _transitionWorld = new World(settings._transitionWorldPath);
            _transitionWorld.Load();
        }
        public static void Run() { _timer.Run(UpdateRate, RenderRate); }
        public static void Stop() { _timer.Stop(); }
        public static void ShowMessage(string message,  int viewport = -1)
        {
            RenderPanel panel = CurrentPanel;
            if (panel == null)
                return;
            if (viewport >= 0)
                panel.GetViewport(viewport)?.ShowMessage(message);
            else
                panel._overallHud.ShowMessage(message);
        }
        public static void Update()
        {
            foreach (GameTimer timer in ActiveTimers)
                timer.UpdateTick(UpdateDelta);
            foreach (PawnController c in ActiveControllers)
                c.Update();
            World.Update();
        }
        public static void SetCurrentWorld(World world)
        {
            if (world != null && !world.IsLoaded)
                world.Load();
            World = world;
        }
    }
}