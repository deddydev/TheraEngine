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

        public static AbstractRenderer Renderer { get { return _renderer; } set { _renderer = value; } }
        private static AbstractRenderer _renderer;

        public static float RenderDelta { get { return (float)_timer.RenderTime; } }
        public static float UpdateDelta { get { return (float)_timer.UpdateTime; } }
        private static GlobalTimer _timer;

        public static World TransitionWorld { get { return _transitionWorld; } set { _transitionWorld = value; } }
        private static World _transitionWorld = null;
        public static World CurrentWorld { get { return _currentWorld; } set { _currentWorld = value; } }
        private static World _currentWorld = null;

        public static BindingList<World> LoadedWorlds = new BindingList<World>();
        public static BindingList<GameTimer> ActiveTimers = new BindingList<GameTimer>();
        public static BindingList<PawnController> ActiveControllers = new BindingList<PawnController>();
        
        public static World World { get { return _currentWorld; } }
        public static GamePanel CurrentPanel
        {
            get
            {
                RenderWindowContext ctx = RenderWindowContext.CurrentContext;
                if (ctx != null)
                    return ctx.Control as GamePanel;
                return null;
            }
        }

        static Engine()
        {
            _timer = new GlobalTimer();
            _timer.Run(60.0f);
            LoadDefaults();
        }
        public static void LoadDefaults()
        {
            EngineSettings s = EngineSettings.FromXML(SettingsPath);
            _transitionWorld = new World("TransitionWorld", s._transitionWorldPath);
            _transitionWorld.Load();
        }

        private static void _contentWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void _contentWatcher_Created(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void _contentWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        public static void ShowMessage(string message,  int viewport = -1)
        {
            GamePanel panel = CurrentPanel;
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
        public static void SetCurrentWorld(World path)
        {
            //if (_currentWorld != null)
            //{
            //    _currentWorld.Visible = false;
            //    Task unload = _currentWorld.Unload(path);
            //}
            //_currentWorld = world;
            //if (_currentWorld != null)
            //{
            //    _currentWorld.Load();
            //    _currentWorld.Visible = true;
            //}
        }
    }
}