using CustomEngine.World;
using System;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;

namespace CustomEngine
{
    public static class Engine
    {
        private static RenderContext _renderContext;

        public static BindingList<WorldBase> LoadedWorlds = new BindingList<WorldBase>();
        private static WorldBase _currentWorld = null;
        public static BindingList<Timer> ActiveTimers = new BindingList<Timer>();
        public static BindingList<Controller> ActiveControllers = new BindingList<Controller>();

        public static WorldBase World { get { return _currentWorld; } }
        public static CustomGameForm Form { get { return CustomGameForm.Instance; } }
        public static double RenderDelta { get { return Form.RenderTime; } }
        public static double UpdateDelta { get { return Form.UpdateTime; } }
        public static RenderContext Renderer { get { return _renderContext; } }
        public static void ShowMessage(string message, int viewport = -1)
        {
            if (viewport >= 0)
                Form.GetViewport(viewport)?.ShowMessage(message);
            Form._overallHud.ShowMessage(message);
        }
        internal static void Update()
        {
            foreach (Timer timer in Engine.ActiveTimers)
                timer.UpdateTick(UpdateDelta);
            foreach (Controller c in Engine.ActiveControllers)
                c.UpdateTick(UpdateDelta);
            World.Update();
        }
        public static void LoadWorld(WorldBase world)
        {
            if (_currentWorld != null)
                _currentWorld.OnUnload();
            _currentWorld = world;
            if (_currentWorld != null)
                _currentWorld.OnLoad();
        }
    }
}