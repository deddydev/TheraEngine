using CustomEngine.Rendering.HUD;
using System.Drawing;
using System;
using CustomEngine.World;
using CustomEngine.Rendering.Cameras;

namespace CustomEngine.Rendering
{
    public class Viewport : IRenderable
    {
        public Camera _currentCamera;
        private HudManager _hud;
        private int _viewportNumber = 0;
        private RectangleF _region;

        public float _leftPercentage = 0.0f;
        public float _rightPercentage = 1.0f;
        public float _bottomPercentage = 0.0f;
        public float _topPercentage = 1.0f;

        public Camera Camera { get { return _currentCamera; } }
        public HudManager HUD { get { return _hud; } }
        public int ViewportNumber { get { return _viewportNumber; } }
        CustomGameForm Form { get { return CustomGameForm.Instance; } }

        public float Height { get { return _region.Height; } }
        public float Width { get { return _region.Width; } }
        public float X { get { return _region.X; } }
        public float Y { get { return _region.Y; } }

        public Viewport()
        {
            _hud = new HudManager(this);
        }
        public void OnResized(float width, float height)
        {
            Camera.Resize(Width, Height);
        }
        public void Render()
        {
            _hud.Render();
            Engine.World.Render();
        }
        public void ShowMessage(string message)
        {
            _hud.ShowMessage(message);
        }
    }
}
