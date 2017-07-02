using TheraEngine.Input;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Drawing;
using TheraEngine.Worlds.Actors.Types;
using TheraEngine.Rendering.Text;
using System.Diagnostics;
using TheraEngine.Rendering.Textures;
using System.Drawing.Imaging;
using BulletSharp;

namespace TheraEngine.Rendering
{
    public delegate void DelOnRender(SceneProcessor scene);
    public class Viewport
    {
        public static Viewport CurrentlyRendering => _currentlyRendering;
        private static Viewport _currentlyRendering = null;

        public DelOnRender Render;

        private LocalPlayerController _owner;
        private HudManager _pawnHUD;
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private RenderPanel _owningPanel;
        private TextDrawer _text;
        internal GBuffer _gBuffer;

        public TextDrawer Text => _text;

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

                    CameraTransformChanged();
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
            if (Owner == null || Camera == null || Engine.Audio == null)
                return;
            Vec3 forward = _worldCamera.GetForwardVector();
            Vec3 up = _worldCamera.GetUpVector();
            Engine.Audio.UpdateListener(Owner.LocalPlayerIndex, _worldCamera.WorldPoint, forward, up, Vec3.Zero, 0.5f);
        }

        public RenderPanel OwningPanel => _owningPanel;
        public HudManager PawnHUD
        {
            get => _pawnHUD;
            set
            {
                _pawnHUD = value ?? new HudManager();
                _pawnHUD.Resize(Region.Bounds);
            }
        }
        public BoundingRectangle Region => _region;
        public float Height => _region.Height;
        public float Width => _region.Width;
        public float X => _region.X;
        public float Y => _region.Y;
        public int Index => _index;
        public Vec2 Center => new Vec2(Width / 2.0f, Height / 2.0f);

        public LocalPlayerController Owner
        {
            get => _owner;
            set
            {
                if (_owner != null)
                    _owner.Viewport = null;
                
                _owner = value;

                if (_owner != null)
                {
                    _owner.Viewport = this;
                    Camera = _owner.CurrentCamera;
                }
            }
        }

        public Viewport(RenderPanel panel, int index)
        {
            if (index == 0)
            {
                _index = index;
                SetFullScreen();
            }
            else
                ViewportCountChanged(index, panel._viewports.Count + 1, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);

            _owningPanel = panel;
            _pawnHUD = new HudManager();
            _index = index;
            Resize(panel.Width, panel.Height);
            _text = new TextDrawer();
            UpdateRender();
        }

        internal void UpdateRender()
        {
            if (Engine.Settings.ShadingStyle == ShadingStyle.Forward)
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
        internal void Resize(float parentWidth, float parentHeight, bool resizeGBuffer = true)
        {
            _region.X = _leftPercentage * parentWidth;
            _region.Y = _bottomPercentage * parentHeight;
            _region.Width = _rightPercentage * parentWidth - _region.X;
            _region.Height = _topPercentage * parentHeight - _region.Y;

            _worldCamera?.Resize(Width, Height);
            _pawnHUD.Resize(_region.Bounds);
            if (resizeGBuffer)
                _gBuffer?.Resize(_region.IntWidth, _region.IntHeight);
        }
        internal void ResizeGBuffer()
        {
            _gBuffer?.Resize(_region.IntWidth, _region.IntHeight);
        }
        public void DebugPrint(string message)
        {
            _pawnHUD.DebugPrint(message);
        }
        public void RenderDeferred(SceneProcessor scene)
        {
            _currentlyRendering = this;
            Engine.Renderer.PushRenderArea(Region);
            Engine.Renderer.CropRenderArea(Region);

            if (Camera != null)
            {
                if (_text.Modified)
                    _text.Draw(_gBuffer.Textures[3]);

                //We want to render to GBuffer textures
                _gBuffer.Bind(EFramebufferTarget.Framebuffer);

                //Clear color and depth and allow writing to depth
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);

                //Cull scene and retrieve renderables for each buffer
                scene.PreRender(Camera);

                //Render opaque deferred items first
                scene.Render(RenderPass.OpaqueDeferred);

#if DEBUG
                Engine.World.PhysicsScene.DebugDrawWorld();
#endif

                //We want to render to back buffer now
                _gBuffer.Unbind(EFramebufferTarget.Framebuffer);

                //Render deferred pass
                _gBuffer.Render();

                Engine.Renderer.Clear(EBufferClear.Depth);

                //Copy depth from GBuffer to main frame buffer
                _gBuffer.Bind(EFramebufferTarget.ReadFramebuffer);
                Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);
                Engine.Renderer.BlitFrameBuffer(
                    0, 0, Region.IntWidth, Region.IntHeight,
                    0, 0, Region.IntWidth, Region.IntHeight,
                    EClearBufferMask.DepthBufferBit,
                    EBlitFramebufferFilter.Nearest);
                _gBuffer.Unbind(EFramebufferTarget.ReadFramebuffer);
                Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);

                //Render other passes
                if (scene.RenderPasses.OpaqueForward.Count > 0 || scene.RenderPasses.TransparentForward.Count > 0)
                {
                    scene.Render(RenderPass.OpaqueForward);
                    Engine.Renderer.AllowDepthWrite(false);
                    scene.Render(RenderPass.TransparentForward);
                    Engine.Renderer.AllowDepthWrite(true);
                }

                scene.PostRender();
            }

            //Render HUD on top: GBuffer is simply for the world scene so HUD is not included.
            _pawnHUD.Render();

            Engine.Renderer.PopRenderArea();
            _currentlyRendering = null;
        }
        public void RenderForward(SceneProcessor scene)
        {
            _currentlyRendering = this;
            Engine.Renderer.PushRenderArea(Region);
            Engine.Renderer.CropRenderArea(Region);

            if (Camera != null)
            {
                if (_text.Modified)
                    _text.Draw(_gBuffer.Textures[1]);

                //We want to render to GBuffer textures
                _gBuffer.Bind(EFramebufferTarget.Framebuffer);

                //Clear color and depth and allow writing to depth
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);

                //Cull scene and retrieve renderables for each buffer
                scene.PreRender(Camera);

                scene.Render(RenderPass.OpaqueForward);
                Engine.Renderer.AllowDepthWrite(false);
                scene.Render(RenderPass.TransparentForward);
                Engine.Renderer.AllowDepthWrite(true);

                //We want to render to back buffer now
                _gBuffer.Unbind(EFramebufferTarget.Framebuffer);

                //Render quad
                _gBuffer.Render();

                scene.PostRender();
            }

            //Render HUD on top: GBuffer is simply for the world scene so HUD is not included.
            _pawnHUD.Render();

            Engine.Renderer.PopRenderArea();
            _currentlyRendering = null;
        }
        public Vec3 ScreenToWorld(Vec2 viewportPoint, float depth)
            => _worldCamera.ScreenToWorld(viewportPoint, depth);
        public Vec3 ScreenToWorld(Vec3 viewportPoint)
            => _worldCamera.ScreenToWorld(viewportPoint);
        public Vec3 WorldToScreen(Vec3 worldPoint)
            => _worldCamera.WorldToScreen(worldPoint);
        public Vec2 AbsoluteToRelative(Vec2 absolutePoint) => new Vec2(absolutePoint.X - _region.X, absolutePoint.Y - _region.Y);
        public Vec2 RelativeToAbsolute(Vec2 viewportPoint) => new Vec2(viewportPoint.X + _region.X, viewportPoint.Y + _region.Y);
        public unsafe float GetDepth(Vec2 viewportPoint)
        {
            Vec2 absolutePoint = viewportPoint;//RelativeToAbsolute(viewportPoint);
            _gBuffer.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.SetReadBuffer(DrawBuffersAttachment.None);
            var depthTex = _gBuffer.Textures[4];
            //depthTex.Bind();
            //if (viewportPoint.Y >= depthTex.Height || viewportPoint.Y < 0 || viewportPoint.X >= depthTex.Width || viewportPoint.X < 0)
            //    return 0;
            //BitmapData bmd = depthTex.LockBits(new Rectangle(0, 0, depthTex.Width, depthTex.Height), ImageLockMode.ReadWrite, depthTex.PixelFormat);
            //float depth = *(float*)((byte*)bmd.Scan0 + ((int)viewportPoint.Y * bmd.Stride + (int)viewportPoint.X * 4));
            //depthTex.UnlockBits(bmd);
            float depth = Engine.Renderer.GetDepth(absolutePoint.X, absolutePoint.Y);
            _gBuffer.Unbind(EFramebufferTarget.Framebuffer);
            return depth;
        }
        public Ray GetWorldRay(Vec2 viewportPoint)
            => _worldCamera.GetWorldRay(viewportPoint);
        public Segment GetWorldSegment(Vec2 viewportPoint)
            => _worldCamera.GetWorldSegment(viewportPoint);
        public SceneComponent PickScene(
            Vec2 viewportPoint,
            bool testHud,
            bool testWorld,
            out Vec3 hitNormal,
            out Vec3 hitPoint,
            out float distance)
        {
            if (testHud)
            {
                HudComponent hudComp = _pawnHUD.FindClosestComponent(viewportPoint);
                if (hudComp != null)
                {
                    hitNormal = Vec3.Backward;
                    hitPoint = new Vec3(viewportPoint, 0.0f);
                    distance = 0.0f;
                    return hudComp;
                }
            }
            if (testWorld)
            {
                Segment cursor = GetWorldSegment(viewportPoint);
                
                ClosestRayResultCallback c = Engine.RaycastClosest(cursor);
                if (c.HasHit)
                {
                    hitNormal = c.HitNormalWorld;
                    hitPoint = c.HitPointWorld;
                    distance = hitPoint.DistanceToFast(cursor.StartPoint);
                    CollisionObject coll = c.CollisionObject;
                    PhysicsDriver d = coll.UserObject as PhysicsDriver;
                    return d.Owner as SceneComponent;
                }

                //float depth = GetDepth(viewportPoint);

                //Vec3 worldPoint = ScreenToWorld(viewportPoint, depth);
                //ThreadSafeList<I3DRenderable> r = Engine.Scene.RenderTree.FindClosest(worldPoint);

            }
            hitNormal = Vec3.Zero;
            hitPoint = Vec3.Zero;
            distance = 0.0f;
            return null;
        }
        /// <summary>
        /// Viewport layout preference for when only two people are playing.
        /// </summary>
        public enum TwoPlayerPreference
        {
            /// <summary>
            /// 1st player is on the top of the screen, 2nd player is on bottom.
            /// </summary>
            SplitHorizontally,
            /// <summary>
            /// 1st player is on the left side of the screen, 2nd player is on the right side.
            /// </summary>
            SplitVertically,
        }
        /// <summary>
        /// Viewport layout preference for when only three people are playing.
        /// </summary>
        public enum ThreePlayerPreference
        {
            /// <summary>
            /// Top left, top right, and bottom left quadrants of the screen are used for viewports.
            /// The bottom right is blank (can be drawn in using global hud; for example, a world map)
            /// </summary>
            BlankBottomRight,
            /// <summary>
            /// First player has a wide screen on top (two quadrants), and the remaining two players have smaller screens in the bottom two quadrants.
            /// </summary>
            PreferFirstPlayer,
            /// <summary>
            /// Second player has a wide screen on top (two quadrants), and the remaining two players have smaller screens in the bottom two quadrants.
            /// </summary>
            PreferSecondPlayer,
            /// <summary>
            /// Third player has a wide screen on top (two quadrants), and the remaining two players have smaller screens in the bottom two quadrants.
            /// </summary>
            PreferThirdPlayer,
        }
        public void ViewportCountChanged(int newIndex, int total, TwoPlayerPreference twoPlayerPref, ThreePlayerPreference threePlayerPref)
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
                            if (twoPlayerPref == TwoPlayerPreference.SplitHorizontally)
                                SetTop();
                            else
                                SetLeft();
                            break;
                        case 1:
                            if (twoPlayerPref == TwoPlayerPreference.SplitHorizontally)
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
                                case ThreePlayerPreference.BlankBottomRight:
                                    SetTopLeft();
                                    break;
                                case ThreePlayerPreference.PreferFirstPlayer:
                                    SetTop();
                                    break;
                                case ThreePlayerPreference.PreferSecondPlayer:
                                    SetBottomLeft();
                                    break;
                                case ThreePlayerPreference.PreferThirdPlayer:
                                    SetTopLeft();
                                    break;
                            }
                            break;
                        case 1:
                            switch (threePlayerPref)
                            {
                                case ThreePlayerPreference.BlankBottomRight:
                                    SetTopRight();
                                    break;
                                case ThreePlayerPreference.PreferFirstPlayer:
                                    SetBottomLeft();
                                    break;
                                case ThreePlayerPreference.PreferSecondPlayer:
                                    SetTop();
                                    break;
                                case ThreePlayerPreference.PreferThirdPlayer:
                                    SetTopRight();
                                    break;
                            }
                            break;
                        case 2:
                            switch (threePlayerPref)
                            {
                                case ThreePlayerPreference.BlankBottomRight:
                                    SetBottomLeft();
                                    break;
                                case ThreePlayerPreference.PreferFirstPlayer:
                                    SetBottomRight();
                                    break;
                                case ThreePlayerPreference.PreferSecondPlayer:
                                    SetBottomRight();
                                    break;
                                case ThreePlayerPreference.PreferThirdPlayer:
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
    }
}
