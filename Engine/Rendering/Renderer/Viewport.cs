using TheraEngine.Input;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.HUD;
using TheraEngine.Rendering.Textures;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace TheraEngine.Rendering
{
    public delegate void DelOnRender(SceneProcessor scene);
    public class Viewport
    {
        public static Viewport CurrentlyRendering { get { return _currentlyRendering; } }
        private static Viewport _currentlyRendering = null;

        public DelOnRender Render;

        private LocalPlayerController _owner;
        private HudManager _hud;
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private RenderPanel _owningPanel;
        private ScreenTextHandler _text;
        private GBuffer _gBuffer;

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
                if (_worldCamera != null)
                {
                    if (_worldCamera.OwningComponent != null)
                        _worldCamera.OwningComponent.WorldTransformChanged -= CameraTransformChanged;
                    else
                        _worldCamera.TransformChanged -= CameraTransformChanged;

                    _worldCamera.OwningComponentChanged -= _worldCamera_OwningComponentChanged;

                    _worldCamera.Viewports.Remove(this);
                }
                _worldCamera = value;
                if (_worldCamera != null)
                {
                    _worldCamera.Viewports.Add(this);

                    if (_worldCamera.OwningComponent != null)
                        _worldCamera.OwningComponent.WorldTransformChanged += CameraTransformChanged;
                    else
                        _worldCamera.TransformChanged += CameraTransformChanged;

                    _worldCamera.OwningComponentChanged += _worldCamera_OwningComponentChanged;

                    //TODO: what if the same camera is used by multiple viewports?
                    //Need to use a separate projection matrix per viewport instead of passing the width and height to the camera itself
                    _worldCamera.Resize(Width, Height);
                }
            }
        }

        private void _worldCamera_OwningComponentChanged(CameraComponent previous, CameraComponent current)
        {
            if (previous != null)
                previous.WorldTransformChanged -= CameraTransformChanged;
            else
                _worldCamera.TransformChanged -= CameraTransformChanged;
            if (current != null)
                current.WorldTransformChanged += CameraTransformChanged;
            else
                _worldCamera.TransformChanged += CameraTransformChanged;
        }

        private void CameraTransformChanged()
        {
            Vec3 forward = _worldCamera.GetForwardVector();
            Vec3 up = _worldCamera.GetUpVector();
            Engine.AudioManager.UpdateListener(_owner.LocalPlayerIndex, _worldCamera.WorldPoint, forward, up, Vec3.Zero, 0.5f);
        }

        public RenderPanel OwningPanel => _owningPanel;
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
            ViewportCountChanged(index, panel._viewports.Count + 1, Engine.TwoPlayerPref, Engine.ThreePlayerPref);
            _owningPanel = panel;
            _hud = new HudManager(this);
            _index = index;
            _owner = owner;
            _owner.Viewport = this;
            Resize(panel.Width, panel.Height);
            _text = new ScreenTextHandler(this);

            _text.Add(new TextData("Hello", new Font("Arial", 20),Color.Blue, new Vec2(Region.Width / 2.0f, Region.Height / 2.0f), Vec2.Half, Vec2.One, 0.5f));

            if (Engine.Settings == null || Engine.Settings.ShadingStyle == ShadingStyle.Forward)
            {
                _gBuffer = new GBuffer(this, true);
                Render = RenderForward;
            }
            else
            {
                _gBuffer = new GBuffer(this, false);
                Render = RenderDeferred;
            }
        }
        internal void Resize(float parentWidth, float parentHeight)
        {
            _region.X = _leftPercentage * parentWidth;
            _region.Y = _bottomPercentage * parentHeight;
            _region.Width = _rightPercentage * parentWidth - _region.X;
            _region.Height = _topPercentage * parentHeight - _region.Y;

            _worldCamera?.Resize(Width, Height);
            _hud.Resize(_region.Bounds);
            _gBuffer?.Resize(_region.IntWidth, _region.IntHeight);
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
        
        public void RenderDeferred(SceneProcessor scene)
        {
            if (Camera == null)
                return;

            _currentlyRendering = this;
            Engine.Renderer.PushRenderArea(Region);
            Engine.Renderer.CropRenderArea(Region);

            if (_text.Modified)
                _text.Draw(_gBuffer.Textures[3]);

            //We want to render to GBuffer textures
            _gBuffer.Bind(EFramebufferTarget.Framebuffer);

            //Clear color and depth and allow writing to depth
            Engine.Renderer.Clear(BufferClear.Color | BufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);

            //Cull scene and retrieve renderables for each buffer
            scene.Cull(Camera);

            //Render opaque deferred items first
            scene.Render(Camera, RenderPass.OpaqueDeferred);

            //We want to render to back buffer now
            _gBuffer.Unbind(EFramebufferTarget.Framebuffer);
            
            //Render quad
            _gBuffer.Render();

            if (scene.RenderPasses.OpaqueForward.Count > 0 ||
                scene.RenderPasses.TransparentForward.Count > 0)
            {
                //Copy depth from GBuffer to main frame buffer
                Engine.Renderer.BlitFrameBuffer(
                    _gBuffer.BindingId, 0,
                    0, 0, Region.IntWidth, Region.IntHeight,
                    0, 0, Region.IntWidth, Region.IntHeight,
                    EClearBufferMask.DepthBufferBit,
                    EBlitFramebufferFilter.Nearest);

                scene.Render(Camera, RenderPass.OpaqueForward);

                Engine.Renderer.AllowDepthWrite(false);
                scene.Render(Camera, RenderPass.TransparentForward);
                Engine.Renderer.AllowDepthWrite(true);
            }

            //Render HUD on top: GBuffer is simply for the world scene so HUD is not included.
            _hud.Render();

            Engine.Renderer.PopRenderArea();
            _currentlyRendering = null;
        }
        public void RenderForward(SceneProcessor scene)
        {
            if (Camera == null)
                return;

            _currentlyRendering = this;
            Engine.Renderer.PushRenderArea(Region);
            Engine.Renderer.CropRenderArea(Region);

            if (_text.Modified)
                _text.Draw(_gBuffer.Textures[1]);

            //We want to render to GBuffer textures
            _gBuffer.Bind(EFramebufferTarget.Framebuffer);

            //Clear color and depth and allow writing to depth
            Engine.Renderer.Clear(BufferClear.Color | BufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);

            //Cull scene and retrieve renderables for each buffer
            scene.Cull(Camera);

            scene.Render(Camera, RenderPass.OpaqueForward);
            Engine.Renderer.AllowDepthWrite(false);
            scene.Render(Camera, RenderPass.TransparentForward);
            Engine.Renderer.AllowDepthWrite(true);
            
            //We want to render to back buffer now
            _gBuffer.Unbind(EFramebufferTarget.Framebuffer);

            //Render quad
            _gBuffer.Render();

            //Render HUD on top: GBuffer is simply for the world scene so HUD is not included.
            _hud.Render();

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
        
        public SceneComponent PickScene(
            Vec2 viewportPoint,
            bool mouse,
            bool testHud = true,
            bool testWorld = true,
            bool highlightActors = true)
        {
            if (testHud)
            {
                HudComponent hudComp = _hud?.FindClosestComponent(viewportPoint);
                if (hudComp != null)
                    return hudComp;
            }
            if (testWorld)
            {
#if EDITOR
                Ray cursor = GetWorldRay(viewportPoint);
                if (EditorTransformTool3D.CurrentInstance != null)
                {
                    if (EditorTransformTool3D.CurrentInstance.UpdateCursorRay(cursor, _worldCamera, false))
                        return EditorTransformTool3D.CurrentInstance.RootComponent;
                }
#endif
                float depth = GetDepth(viewportPoint);
                Vec3 worldPoint = ScreenToWorld(viewportPoint, depth);
                ThreadSafeList<IRenderable> r = Engine.Renderer.Scene.RenderTree.FindClosest(worldPoint);

            }
            return null;
        }
    }

    public class TextData
    {
        /// <summary>
        /// Constructs a new text data class.
        /// </summary>
        /// <param name="text">The text string to render.</param>
        /// <param name="font">The font to render it with.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="position">The 2D location of the text relative to the bottom left (0, 0).</param>
        /// <param name="originPercentages">The relative position of the origin of the text from the bottom left, as a UV-style coordinate (0 is left/bottom, 1 is right/top).</param>
        /// <param name="scale">The scale of the text.</param>
        /// <param name="depth">The order to render this text in. 0.0 is first, 1.0 is last.</param>
        public TextData(string text, Font font, ColorF4 color, Vec2 position, Vec2 originPercentages, Vec2 scale, float depth)
        {
            _text = text;
            _font = font;
            _brush = new SolidBrush(color);
            _position = position;
            _originPercentages = originPercentages;
            _scale = scale;
            _depth = depth;
        }

        private string _text;
        private Font _font;
        private SolidBrush _brush;
        private Vec2 _position, _originPercentages;
        private Vec2 _scale;
        private float _depth;

        internal ScreenTextHandler _parent;
        internal BoundingRectangle _bounds; //Set after being drawn
        internal List<TextData> _overlapping; //Set after being drawn

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _parent?.TextChanged(this);
            }
        }
        public Font Font
        {
            get => _font;
            set
            {
                _font = value;
                _parent?.TextChanged(this);
            }
        }
        public SolidBrush Brush
        {
            get => _brush;
            set
            {
                _brush = value;
                _parent?.TextChanged(this);
            }
        }
        public Vec2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _parent?.TextChanged(this);
            }
        }
        public Vec2 OriginPercentages
        {
            get => _originPercentages;
            set
            {
                _originPercentages = value;
                _parent?.TextChanged(this);
            }
        }
        public Vec2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _parent?.TextChanged(this);
            }
        }
        public float Depth
        {
            get => _depth;
            set
            {
                _depth = value;
                _parent?.TextChanged(this);
            }
        }
    }

    public class ScreenTextHandler
    {
        internal Viewport _viewport;
        internal SortedDictionary<float, TextData> _text;
        private LinkedList<TextData> _modified;

        public bool Modified => _modified.Count > 0;

        public ScreenTextHandler(Viewport viewport)
        {
            _text = new SortedDictionary<float, TextData>();
            _modified = new LinkedList<TextData>();
            _viewport = viewport;
        }

        public void Clear()
        {
            _text.Clear();
        }

        public void Add(TextData text)
        {
            text._parent = this;
            _text.Add(text.Depth, text);
            _modified.AddLast(text);
        }

        public unsafe void Draw(Texture texture)
        {
            Bitmap b = texture.Data.Bitmap;

            //Resize bitmap if viewport bounds do not match
            if ((IVec2)b.Size != (IVec2)_viewport.Region.Bounds)
            {
                if (b != null)
                    b.Dispose();

                if (_viewport.Region.IntWidth == 0 || _viewport.Region.IntHeight == 0)
                {
                    texture.Data.Bitmap = null;
                    return;
                }

                b = new Bitmap(_viewport.Region.IntWidth, _viewport.Region.IntHeight);
                b.MakeTransparent();
                texture.Data.Bitmap = b;
            }

            //TODO: instead of redrawing the whole image, keep track of overlapping text
            //and only redraw the previous and new regions. Repeat for any other overlapping texts.
            //Then textsubimage2d using the min and max values of all updated texts.

            //Draw text information onto the bitmap
            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                
                foreach (TextData text in _text.Values)
                {
                    SizeF textSize = g.MeasureString(text.Text, text.Font);
                    Vec2 v = text.Position;
                    text._bounds = new BoundingRectangle(v, textSize);
                    if (text._bounds.ContainedWithin(_viewport.Region) != EContainment.Disjoint)
                    {
                        g.ResetTransform();
                        g.TranslateTransform(text._position.X, text._position.Y);
                        g.ScaleTransform(text._scale.X, text._scale.Y);
                        g.DrawString(text._text, text._font, text._brush, 0.0f, 0.0f);
                    }
                }
            }
            texture.PushData();
            _modified.Clear();
        }

        internal void TextChanged(TextData textData)
        {
            _modified.AddLast(textData);
        }
    }
}
