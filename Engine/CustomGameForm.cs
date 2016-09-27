using CustomEngine.Rendering;
using System;
using CustomEngine.World;
using OpenTK;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using CustomEngine.Rendering.HUD;

namespace CustomEngine
{
    public partial class CustomGameForm : GameWindow
    {
        public static CustomGameForm Instance { get { return _instance ?? new CustomGameForm("ERROR"); } }
        private static CustomGameForm _instance;

        public HudManager _overallHud;
        public List<Viewport> _viewports = new List<Viewport>();

        public CustomGameForm(string title)
        {
            Title = title;
            _instance = this;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            Engine.Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (Viewport v in _viewports)
                v.Render();
            _overallHud.Render();
        }

        public Viewport GetViewport(int viewport)
        {
            return _viewports[viewport.Clamp(0, _viewports.Count - 1)];
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
        }
    }
}
