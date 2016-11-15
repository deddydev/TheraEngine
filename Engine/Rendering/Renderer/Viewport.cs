using CustomEngine.Rendering.HUD;
using System.Drawing;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;
using System;

namespace CustomEngine.Rendering
{
    public class Viewport
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

        public Camera Camera
        {
            get { return _worldCamera; }
            set
            {
                _worldCamera = value;
                _worldCamera?.Resize(Width, Height);
            }
        }
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

            _worldCamera?.Resize(Width, Height);
            _hud.ParentResized(_region);
        }
        public void DebugPrint(string message) { _hud.DebugPrint(message); }
        private void SetTopLeft()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 0.5f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.5f;
        }
        private void SetTopRight()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 0.5f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.5f;
        }
        private void SetBottomLeft()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 0.5f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.5f;
        }
        private void SetBottomRight()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 0.5f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.5f;
        }
        private void SetTop()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 1.0f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.5f;
        }
        private void SetBottom()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 1.0f;
            _topPercentage = 0.5f;
            _bottomPercentage = 0.0f;
        }
        private void SetLeft()
        {
            _leftPercentage = 0.0f;
            _rightPercentage = 0.5f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.0f;
        }
        private void SetRight()
        {
            _leftPercentage = 0.5f;
            _rightPercentage = 1.0f;
            _topPercentage = 1.0f;
            _bottomPercentage = 0.0f;
        }
        private void SetFullScreen()
        {
            _leftPercentage = _bottomPercentage = 0.0f;
            _rightPercentage = _topPercentage = 1.0f;
        }

        public unsafe void Render(SceneProcessor scene)
        {
            if (_worldCamera == null)
                return;

            _currentlyRendering = this;
            Engine.Renderer.PushRenderArea(Region);
            Engine.Renderer.CropRenderArea(Region);

            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
            //Matrix4 m1 = _worldCamera.Matrix;
            //OpenTK.Graphics.OpenGL.GL.PushMatrix();
            //OpenTK.Graphics.OpenGL.GL.LoadMatrix((float*)&m1);
            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            //Matrix4 m2 = _worldCamera.ProjectionMatrix;
            //OpenTK.Graphics.OpenGL.GL.PushMatrix();
            //OpenTK.Graphics.OpenGL.GL.LoadMatrix((float*)&m2);
            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);

            //OpenTK.Graphics.OpenGL.GL.PopMatrix();
            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
            //OpenTK.Graphics.OpenGL.GL.PopMatrix();
            //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);

            scene.Render(_worldCamera);
            //_hud.Render();
            Engine.Renderer.PopRenderArea();
            _currentlyRendering = null;
        }

        public enum TwoPlayerViewportPreference
        {
            SplitHorizontally,
            SplitVertically,
        }
        public enum ThreePlayerViewportPreference
        {
            BlankBottomRight,
            PreferFirstPlayer,
            PreferSecondPlayer,
            PreferThirdPlayer,
        }
        public void ViewportCountChanged(int newIndex, int total, TwoPlayerViewportPreference twoPlayerPref, ThreePlayerViewportPreference threePlayerPref)
        {
            _index = newIndex;
            switch (total)
            {
                case 1:
                    SetFullScreen();
                    break;
                case 2:
                    switch (newIndex)
                    {
                        case 0:
                            if (twoPlayerPref == TwoPlayerViewportPreference.SplitHorizontally)
                                SetTop();
                            else
                                SetLeft();
                            break;
                        case 1:
                            if (twoPlayerPref == TwoPlayerViewportPreference.SplitHorizontally)
                                SetBottom();
                            else
                                SetRight();
                            break;
                    }
                    break;
                case 3:
                    switch (newIndex)
                    {
                        case 0:
                            switch (threePlayerPref)
                            {
                                case ThreePlayerViewportPreference.BlankBottomRight:
                                    SetTopLeft();
                                    break;
                                case ThreePlayerViewportPreference.PreferFirstPlayer:
                                    SetTop();
                                    break;
                                case ThreePlayerViewportPreference.PreferSecondPlayer:
                                    SetBottomLeft();
                                    break;
                                case ThreePlayerViewportPreference.PreferThirdPlayer:
                                    SetTopLeft();
                                    break;
                            }
                            break;
                        case 1:
                            switch (threePlayerPref)
                            {
                                case ThreePlayerViewportPreference.BlankBottomRight:
                                    SetTopRight();
                                    break;
                                case ThreePlayerViewportPreference.PreferFirstPlayer:
                                    SetBottomLeft();
                                    break;
                                case ThreePlayerViewportPreference.PreferSecondPlayer:
                                    SetTop();
                                    break;
                                case ThreePlayerViewportPreference.PreferThirdPlayer:
                                    SetTopRight();
                                    break;
                            }
                            break;
                        case 2:
                            switch (threePlayerPref)
                            {
                                case ThreePlayerViewportPreference.BlankBottomRight:
                                    SetBottomLeft();
                                    break;
                                case ThreePlayerViewportPreference.PreferFirstPlayer:
                                    SetBottomRight();
                                    break;
                                case ThreePlayerViewportPreference.PreferSecondPlayer:
                                    SetBottomRight();
                                    break;
                                case ThreePlayerViewportPreference.PreferThirdPlayer:
                                    SetBottom();
                                    break;
                            }
                            break;
                    }
                    break;
                case 4:
                    switch (newIndex)
                    {
                        case 0: SetTopLeft(); break;
                        case 1: SetTopRight(); break;
                        case 2: SetBottomLeft(); break;
                        case 3: SetBottomRight(); break;
                    }
                    break;
            }
        }
    }
}
