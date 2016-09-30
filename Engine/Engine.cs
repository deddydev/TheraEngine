using CustomEngine.World;
using System;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;
using eyecm.PhysX;
using System.Threading.Tasks;

namespace CustomEngine
{
    public static class Engine
    {
        private static RenderContext _renderContext;

        private static WorldBase _transitionMap = null;

        public static BindingList<WorldBase> LoadedWorlds = new BindingList<WorldBase>();
        private static WorldBase _currentWorld = null;
        public static BindingList<Timer> ActiveTimers = new BindingList<Timer>();
        public static BindingList<PawnController> ActiveControllers = new BindingList<PawnController>();

        public static WorldBase World { get { return _currentWorld; } }
        public static CustomGameForm Form { get { return CustomGameForm.Instance; } }
        public static double RenderDelta { get { return Form.RenderTime; } }
        public static double UpdateDelta { get { return Form.UpdateTime; } }
        public static RenderContext Renderer { get { return _renderContext; } }
        public static void ShowMessage(string message, int viewport = -1)
        {
            if (viewport >= 0)
                Form.GetViewport(viewport)?.ShowMessage(message);
            else
                Form._overallHud.ShowMessage(message);
        }
        public static void Update()
        {
            foreach (Timer timer in ActiveTimers)
                timer.UpdateTick(UpdateDelta);
            foreach (PawnController c in ActiveControllers)
                c.Update();
            World.Update();
        }
        public static void LoadWorld(string path)
        {
            if (_currentWorld != null)
            {
                _currentWorld.Visible = false;
                Task unload = _currentWorld.Unload(path);
            }
            _currentWorld = world;
            if (_currentWorld != null)
            {
                _currentWorld.Load();
                _currentWorld.Visible = true;
            }
        }
    }
}