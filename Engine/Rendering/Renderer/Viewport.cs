using CustomEngine.Rendering.HUD;
using System.Drawing;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;

namespace CustomEngine.Rendering
{
    public class Viewport : IRenderable
    {
        private static Viewport _currentlyRendering = null;
        public static Viewport CurrentlyRendering { get { return _currentlyRendering; } }

        private LocalPlayerController _owner;
        private HudManager _hud;
        private int _index;
        private Rectangle _region;
        private Camera _worldCamera;

        private float _leftPercentage = 0.0f;
        private float _rightPercentage = 1.0f;
        private float _bottomPercentage = 0.0f;
        private float _topPercentage = 1.0f;

        public Camera Camera { get { return _worldCamera; } set { _worldCamera = value; } }
        public HudManager HUD { get { return _hud; } }
        public LocalPlayerController OwningPlayer { get { return _owner; } }
        public Rectangle Region { get { return _region; } }
        public int Height { get { return _region.Height; } }
        public int Width { get { return _region.Width; } }
        public int X { get { return _region.X; } }
        public int Y { get { return _region.Y; } }
        public int Index { get { return _index; } }

        public Viewport(LocalPlayerController owner, int index)
        {
            _hud = new HudManager(this);
            _worldCamera = new PerspectiveCamera();
            _index = index;
            _owner = owner;
            _owner.Viewport = this;
        }
        public void Resize(float parentWidth, float parentHeight)
        {
            _region.X = (int)(_leftPercentage * parentWidth);
            _region.Y = (int)(_bottomPercentage * parentHeight);
            _region.Width = (int)(_rightPercentage * parentWidth - _region.X);
            _region.Height = (int)(_topPercentage * parentHeight - _region.Y);

            _worldCamera.Resize(Width, Height);
            _hud.Resize(Width, Width);
        }
        public void Render(float delta)
        {
            _currentlyRendering = this;
            Engine.Renderer.PushRenderArea(_region);

            _hud.Render(delta);

            _worldCamera.SetCurrent();
            Engine.World?.Render(delta);

            Engine.Renderer.PopRenderArea();
            _currentlyRendering = null;
        }
        public void ShowMessage(string message)
        {
            _hud.ShowMessage(message);
        }
        public void ViewportCountChanged(int newIndex, int total)
        {
            _index = newIndex;
        }
    }
}
