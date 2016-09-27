using CustomEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering;
using CustomEngine.Input;
using System.ComponentModel;

namespace CustomEngine
{
    public static class Engine
    {
        private static RenderContext _renderContext;

        public static BindingList<WorldBase> LoadedWorlds = new BindingList<WorldBase>();
        public static WorldBase World = null;
        public static BindingList<Timer> ActiveTimers = new BindingList<Timer>();
        public static BindingList<Controller> ActiveControllers = new BindingList<Controller>();

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

    }
}