using CustomEngine.Rendering.HUD;
using System.Drawing;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;

namespace CustomEngine.Rendering
{
    public class Viewport : IRenderable
    {
        private PlayerController _player;
        private HudManager _hud;
        private int _viewportNumber = 0;
        private RectangleF _region;

        public float _leftPercentage = 0.0f;
        public float _rightPercentage = 1.0f;
        public float _bottomPercentage = 0.0f;
        public float _topPercentage = 1.0f;
        
        public Camera Camera { get { return _player.CurrentCamera; } }
        public HudManager HUD { get { return _hud; } }
        public int ViewportNumber { get { return _player.Number; } }

        public float Height { get { return _region.Height; } }
        public float Width { get { return _region.Width; } }
        public float X { get { return _region.X; } }
        public float Y { get { return _region.Y; } }

        public Viewport()
        {
            _hud = new HudManager(this);
        }
        public void Resize(float parentWidth, float parentHeight)
        {
            _region.X = _leftPercentage * parentWidth;
            _region.Y = _bottomPercentage * parentHeight;
            _region.Width = _rightPercentage * parentWidth - _region.X;
            _region.Height = _topPercentage * parentHeight - _region.Y;

            Camera.Resize(Width, Height);
            _hud.Resize(Width, Width);
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
