using CustomEngine.Input;
using CustomEngine.Rendering;
using System;
using CustomEngine.World;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine
{
    public partial class CustomGameForm : GameWindow
    {
        public static CustomGameForm Instance { get { return _instance ?? new CustomGameForm("ERROR"); } }
        private static CustomGameForm _instance;

        public List<Viewport> _viewports = new List<Viewport>();
        public List<WorldBase> _loadedWorlds = new List<WorldBase>();
        public WorldBase _currentWorld = null;
        public List<Timer> _activeTimers = new List<Timer>();
        public List<Controller> _activeControllers = new List<Controller>();

        public CustomGameForm(string title)
        {
            Title = title;
            _instance = this;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            foreach (Timer timer in _activeTimers)
                timer.UpdateTick(e.Time);
            foreach (Controller c in _activeControllers)
                c.UpdateTick(e.Time);
            _currentWorld.UpdateTick(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (Viewport v in _viewports)
                v.RenderTick(e.Time);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }

        public void LoadWorld(WorldBase world)
        {
            if (_currentWorld != null)
                _currentWorld.OnUnload();

            _currentWorld = world;

            if (_currentWorld != null)
                _currentWorld.OnLoad();
        }
    }
}
