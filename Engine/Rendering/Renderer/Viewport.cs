using CustomEngine.Rendering.HUD;
using System.Drawing;
using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;
using System;
using OpenTK.Graphics.OpenGL;
using CustomEngine.Worlds;
using System.Linq;
using CustomEngine.Worlds.Actors.Types;
using System.Collections.Generic;
using CustomEngine.Rendering.Models;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System.Drawing.Text;

namespace CustomEngine.Rendering
{
    public class Viewport
    {
        public static Viewport CurrentlyRendering { get { return _currentlyRendering; } }
        private static Viewport _currentlyRendering = null;

        private LocalPlayerController _owner;
        private HudManager _hud;
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private GBuffer _gBuffer;
        private RenderPanel _owningPanel;
        private ScreenTextHandler _text;

        public ScreenTextHandler Text => _text;

        private float _leftPercentage = 0.0f;
        private float _rightPercentage = 1.0f;
        private float _bottomPercentage = 0.0f;
        private float _topPercentage = 1.0f;

        public Camera Camera
        {
            get => _worldCamera;
            set
            {
                _worldCamera?.Viewports.Remove(this);
                _worldCamera = value;
                if (_worldCamera != null)
                {
                    _worldCamera.Viewports.Add(this);
                    //TODO: what if the same camera is used by multiple viewports?
                    //Need to use a separate projection matrix per viewport instead of passing the width and height to the camera itself
                    _worldCamera.Resize(Width, Height);
                }
            }
        }
        public RenderPanel OwningPanel => _owningPanel;
        public GBuffer GBuffer => _gBuffer;
        public HudManager HUD
        {
            get => _hud;
            set => _hud = value ?? new HudManager(this);
        }
        public LocalPlayerController OwningPlayer => _owner;
        public BoundingRectangle Region => _region;
        public float Height => _region.Height;
        public float Width => _region.Width;
        public float X => _region.X;
        public float Y => _region.Y;
        public int Index => _index;
        public Vec2 Center => new Vec2(Width / 2.0f, Height / 2.0f);

        public Viewport(LocalPlayerController owner, RenderPanel panel, int index)
        {
            _owningPanel = panel;
            _hud = new HudManager(this);
            _index = index;
            _owner = owner;
            _owner.Viewport = this;
            if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
                _gBuffer = new GBuffer(
                    GBufferTextureType.Diffuse | 
                    GBufferTextureType.Normal |
                    GBufferTextureType.Position);
        }
        internal void Resize(float parentWidth, float parentHeight)
        {
            _region.X = _leftPercentage * parentWidth;
            _region.Y = _bottomPercentage * parentHeight;
            _region.Width = _rightPercentage * parentWidth - _region.X;
            _region.Height = _topPercentage * parentHeight - _region.Y;

            _worldCamera?.Resize(Width, Height);
            _hud?.Resize(_region);
            _gBuffer.Resize(Width, Height);
        }
        public void DebugPrint(string message)
        {
            _hud.DebugPrint(message);
        }
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
        public Vec3 ScreenToWorld(Vec2 viewportPoint, float depth)
            => _worldCamera.ScreenToWorld(viewportPoint, depth);
        public Vec3 ScreenToWorld(Vec3 viewportPoint)
            => _worldCamera.ScreenToWorld(viewportPoint);
        public Vec3 WorldToScreen(Vec3 worldPoint) 
            => _worldCamera.WorldToScreen(worldPoint);

        public Vec2 AbsoluteToRelative(Vec2 absolutePoint) => new Vec2(absolutePoint.X - _region.X, absolutePoint.Y - _region.Y);
        public Vec2 RelativeToAbsolute(Vec2 viewportPoint) => new Vec2(viewportPoint.X + _region.X, viewportPoint.Y + _region.Y);
        public float GetDepth(Vec2 viewportPoint)
        {
            Vec2 absolutePoint = RelativeToAbsolute(viewportPoint);
            return Engine.Renderer.GetDepth(absolutePoint.X, absolutePoint.Y);
        }

        public Ray GetWorldRay(Vec2 viewportPoint)
            => _worldCamera.GetWorldRay(viewportPoint);
        public Segment GetWorldSegment(Vec2 viewportPoint) 
            => _worldCamera.GetWorldSegment(viewportPoint);

        public unsafe void Render(SceneProcessor scene)
        {
            if (_worldCamera == null)
                return;

            _currentlyRendering = this;
            GL.Viewport(Region.IntX, Region.IntY, Region.IntWidth, Region.IntHeight);
            
            //Engine.Renderer.PushRenderArea(Region);
            //Engine.Renderer.CropRenderArea(Region);
            
            scene.Render(_worldCamera);
            _hud.Render();

            //Engine.Renderer.PopRenderArea();
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
        
        public IActor PickScene(
            Vec2 viewportPoint,
            bool mouse,
            bool testHud = true,
            bool testWorld = true,
            bool highlightActors = true)
        {
            if (testHud)
            {
                HudComponent hudComp = _hud.FindComponent(viewportPoint);
                if (hudComp != null)
                    return hudComp;
            }
            if (testWorld)
            {
#if EDITOR
                Ray cursor = GetWorldRay(viewportPoint);
                if (EditorTransformTool.CurrentInstance != null)
                {
                    if (EditorTransformTool.CurrentInstance.UpdateCursorRay(cursor, _worldCamera, false))
                        return EditorTransformTool.CurrentInstance;
                }
#endif
                float depth = 0.0f; //GetDepth(viewportPoint);
                Vec3 worldPoint = ScreenToWorld(viewportPoint, depth);
                List<I3DBoundable> r = Engine.Renderer.Scene.RenderTree.FindClosest(worldPoint);

            }
            return null;
        }
    }

    public class ScreenTextHandler
    {
        PrimitiveManager _manager;
        Texture _texture;
        
        internal class TextData
        {
            public string _string;
            public List<Vec3> _positions = new List<Vec3>();
        }
        internal static int _fontSize = 12;
        internal static readonly Font _textFont = new Font("Arial", _fontSize);
        internal Viewport _viewport;
        internal Dictionary<string, TextData> _text = new Dictionary<string, TextData>();
        public int Count { get { return _text.Count; } }

        internal IVec2 _size = new IVec2();
        internal Bitmap _bitmap = null;
        internal int _texId = -1;
        Matrix4 _transform = Matrix4.Identity;

        public Vec3 this[string text]
        {
            set
            {
                if (!_text.ContainsKey(text))
                    _text.Add(text, new TextData() { _string = text, _positions = new List<Vec3>() { value } });
                else
                    _text[text]._positions.Add(value);
            }
        }

        public ScreenTextHandler(Viewport viewport)
        {
            _text = new Dictionary<string, TextData>();
            _viewport = viewport;
            TextureReference texRef = new TextureReference("Text", (int)_viewport.Width, (int)_viewport.Height);
            _manager = new PrimitiveManager(PrimitiveData.FromQuads(Culling.Back, new PrimitiveBufferInfo(), VertexQuad.ZUpQuad(_viewport.Width, _viewport.Height)), Material.GetBasicTextureMaterial(texRef));
            _texture = _manager.Program.Textures[0];
        }

        public void Clear() => _text.Clear();

        public unsafe void Draw()
        {
            Bitmap b = _texture.Data.Bitmap;

            //Resize bitmap if viewport bounds have changed
            if (_size != (IVec2)_viewport.Region.Bounds ||
                _viewport.Region.Bounds.X.IsZero() ||
                _viewport.Region.Bounds.Y.IsZero())
            {
                _size = (IVec2)_viewport.Region.Bounds;
                _transform = Matrix4.CreateTranslation(_viewport.Width / 2.0f, _viewport.Height / 2.0f, 0.0f);

                if (b != null)
                    b.Dispose();

                if (_size.X == 0 || _size.Y == 0)
                    return;

                //Create a texture over the whole model panel
                b = new Bitmap(_size.X, _size.Y);
                b.MakeTransparent();
                _texture.Data.Bitmap = b;
            }

            //Draw text information onto the bitmap
            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.AntiAlias;

                List<Vec2> _used = new List<Vec2>();

                foreach (TextData d in _text.Values)
                    foreach (Vec3 v in d._positions)
                        if (v.X + d._string.Length * 10.0f > 0.0f && v.X < _viewport.Width &&
                            v.Y > -10.0f && v.Y < _viewport.Height &&
                            v.Z > 0.0f && v.Z < 1.0f && //near and far depth values
                            !_used.Contains(new Vec2(v.X, v.Y)))
                        {
                            g.DrawString(d._string, _textFont, Brushes.Black, new PointF(v.X, v.Y));
                            _used.Add(new Vec2(v.X, v.Y));
                        }
            }

            _texture.PushData();
            _manager.Render(_transform, Matrix3.Identity);
        }
    }
}
