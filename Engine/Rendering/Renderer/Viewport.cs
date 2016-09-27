using CustomEngine.Rendering.HUD;
using System.Drawing;
using System;
using CustomEngine.World;

namespace CustomEngine.Rendering
{
    public class Viewport : IPanel, IRenderable
    {
        public Camera _currentCamera;
        private HudManager _hud;
        private int _viewportNumber = 0;
        private RectangleF _region;

        public Camera Camera { get { return _currentCamera; } }
        public HudManager HUD { get { return _hud; } }
        public int ViewportNumber { get { return _viewportNumber; } }
        CustomGameForm Form { get { return CustomGameForm.Instance; } }

        public float Height { get { return _region.Height; } set { } }
        public float Width { get { return _region.Width; } set { } }
        public float X { get { return _region.X; } set { } }
        public float Y { get { return _region.Y; } set { } }
        public RectangleF Region { get { return _region; } set { } }

        public Viewport() { _hud = new HudManager(this); }

        public void OnResized()
        {
            Camera.SetDimensions(Width, Height);
        }

        public void Render()
        {
            
            Engine.World.Render();
        }
        internal void ShowMessage(string message)
        {
            _hud.ShowMessage(message);
        }
    }
}
