using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Shapes;
using TheraEngine.Input;
using TheraEngine.Physics;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.UI;

namespace TheraEngine.Rendering
{
    public class Viewport
    {
        private static Stack<Viewport> CurrentlyRenderingViewports { get; } = new Stack<Viewport>();
        public static Viewport CurrentlyRendering => CurrentlyRenderingViewports.Peek();

        private List<LocalPlayerController> _owners = new List<LocalPlayerController>();
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private BaseRenderPanel _owningPanel;
        
        private SSAOInfo _ssaoInfo = new SSAOInfo();

        internal QuadFrameBuffer SSAOFBO;
        internal QuadFrameBuffer SSAOBlurFBO;
        internal QuadFrameBuffer GBufferFBO;
        internal QuadFrameBuffer BloomBlurFBO1;
        internal QuadFrameBuffer BloomBlurFBO2;
        internal QuadFrameBuffer BloomBlurFBO4;
        internal QuadFrameBuffer BloomBlurFBO8;
        internal QuadFrameBuffer BloomBlurFBO16;
        internal QuadFrameBuffer LightCombineFBO;
        internal QuadFrameBuffer BrightPassFBO;
        internal QuadFrameBuffer PostProcessFBO;
        internal QuadFrameBuffer HudFBO;
        internal PrimitiveManager PointLightManager;
        internal PrimitiveManager SpotLightManager;
        internal QuadFrameBuffer DirLightFBO;
        internal Camera RenderingCamera => RenderingCameras.Peek();
        internal Stack<Camera> RenderingCameras { get; } = new Stack<Camera>();
        internal TexRef2D BrdfTex;

        private BoundingRectangle _internalResolution = new BoundingRectangle();

        private float _leftPercentage = 0.0f;
        private float _rightPercentage = 1.0f;
        private float _bottomPercentage = 0.0f;
        private float _topPercentage = 1.0f;

        public Camera Camera
        {
            get => _worldCamera;
            set
            {
                _worldCamera?.Viewports?.Remove(this);
                
                _worldCamera = value;

                //Engine.PrintLine("Updated viewport " + _index + " camera: " + (_worldCamera == null ? "null" : _worldCamera.GetType().GetFriendlyName()));

                if (_worldCamera != null)
                {
                    _worldCamera.Viewports.Add(this);
                    
                    //TODO: what if the same camera is used by multiple viewports?
                    //Need to use a separate projection matrix per viewport instead of passing the width and height to the camera itself
                    _worldCamera.Resize(_internalResolution.Width, _internalResolution.Height);
                    if (_worldCamera is PerspectiveCamera p)
                        p.Aspect = Width / Height;
                }
            }
        }

        //public BaseRenderPanel OwningPanel => _owningPanel;
        public BoundingRectangle Region => _region;
        public float Height { get => _region.Height; set => _region.Height = value; }
        public float Width { get => _region.Width; set => _region.Width = value; }
        public float X { get => _region.X; set => _region.X = value; }
        public float Y { get => _region.Y; set => _region.Y = value; }
        public Vec2 Position { get => _region.OriginTranslation; set => _region.OriginTranslation = value; }
        public int Index => _index;
        
        public List<LocalPlayerController> Owners => _owners;
        //{
        //    get => _owner;
        //    set
        //    {
        //        if (_owner != null)
        //            _owner.Viewport = null;
                
        //        _owner = value;

        //        if (_owner != null)
        //        {
        //            _owner.Viewport = this;
        //            Camera = _owner.CurrentCamera;
        //        }
        //    }
        //}

        public BoundingRectangle InternalResolution => _internalResolution;

        private IUIManager _hud;
        public IUIManager HUD
        {
            get => _hud;
            set
            {
                _hud = value;
                _hud?.Resize(Region.Extents);

                //Engine.PrintLine("Updated viewport " + _index + " HUD: " + (_hud == null ? "null" : _hud.GetType().GetFriendlyName()));
            }
        }

        public BaseRenderPanel OwningPanel => _owningPanel;

        public Viewport(BaseRenderPanel panel, int index)
        {
            if (index == 0)
            {
                _index = index;
                SetFullScreen();
            }
            else
                ViewportCountChanged(index, panel.Viewports.Count + 1, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);

            _owningPanel = panel;
            _index = index;
            _ssaoInfo.Generate();
            PrecomputeBRDF();
            Resize(panel.Width, panel.Height);
        }
        public Viewport(float width, float height)
        {
            _index = 0;
            SetFullScreen();
            _ssaoInfo.Generate();
            PrecomputeBRDF();
            Resize(width, height);
        }
        public void SetInternalResolution(float width, float height)
        {
            _internalResolution.Width = width;
            _internalResolution.Height = height;

            int w = _internalResolution.IntWidth;
            int h = _internalResolution.IntHeight;

            //Engine.PrintLine("Internal resolution changed: {0}x{1}", w, h);
            
            InitFBOs();

             _worldCamera?.Resize(w, h);
        }

        public void Resize(
            float parentWidth,
            float parentHeight,
            bool setInternalResolution = true,
            float internalResolutionWidthScale = 1.0f,
            float internalResolutionHeightScale = 1.0f)
        {
            float w = parentWidth.ClampMin(1.0f);
            float h = parentHeight.ClampMin(1.0f);

            _region.X = _leftPercentage * w;
            _region.Y = _bottomPercentage * h;
            _region.Width = _rightPercentage * w - _region.X;
            _region.Height =  _topPercentage * h - _region.Y;
            
            if (setInternalResolution) SetInternalResolution(
                _region.Width * internalResolutionWidthScale, 
                _region.Height * internalResolutionHeightScale);

            HUD?.Resize(_region.Extents);
            if (Camera is PerspectiveCamera p)
                p.Aspect = _region.Width / _region.Height;
        }

        /// <summary>
        /// Associates the given local player controller with this viewport.
        /// </summary>
        public void RegisterController(LocalPlayerController controller)
        {
            if (!Owners.Contains(controller))
                Owners.Add(controller);
            controller.Viewport = this;
        }
        /// <summary>
        /// Disassociates the given local player controller with this viewport.
        /// </summary>
        public void UnregisterController(LocalPlayerController controller)
        {
            if (controller.Viewport != this)
                return;
            controller.Viewport = null;
            if (Owners.Contains(controller))
                Owners.Remove(controller);
        }
        
        /// <summary>
        /// Renders the viewport using the given scene, camera, frustum, and optional render target FBO.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="camera"></param>
        /// <param name="frustum"></param>
        /// <param name="target"></param>
        public void Render(BaseScene scene, Camera camera, FrameBuffer target)
        {
            if (scene == null || scene.Count == 0)
                return;

            CurrentlyRenderingViewports.Push(this);
            OnRender(scene, camera, target);
            CurrentlyRenderingViewports.Pop();
        }
        private RenderPasses _renderPasses = new RenderPasses();
        protected virtual void OnRender(BaseScene scene, Camera camera, FrameBuffer target)
        {
            scene.Render(_renderPasses, camera, this, HUD, target);
        }
        internal protected virtual void SwapBuffers()
        {
            _renderPasses.SwapBuffers();
            HUD?.RenderPasses.SwapBuffers();
        }
        public void Update(BaseScene scene, Camera camera, Frustum frustum)
        {
            scene?.Update(_renderPasses, frustum, camera, HUD, false);
        }

        #region Coordinate conversion
        public Vec3 ScreenToWorld(Vec2 viewportPoint, float depth)
            => _worldCamera.ScreenToWorld(ToInternalResCoords(viewportPoint), depth);
        public Vec3 ScreenToWorld(Vec3 viewportPoint)
            => _worldCamera.ScreenToWorld(ToInternalResCoords(viewportPoint.Xy), viewportPoint.Z);
        public Vec3 WorldToScreen(Vec3 worldPoint)
        {
            Vec3 screenPoint = _worldCamera.WorldToScreen(worldPoint);
            screenPoint.Xy = FromInternalResCoords(screenPoint.Xy);
            return screenPoint;
        }
        public Vec2 AbsoluteToRelative(Vec2 absolutePoint) => new Vec2(absolutePoint.X - _region.X, absolutePoint.Y - _region.Y);
        public Vec2 RelativeToAbsolute(Vec2 viewportPoint) => new Vec2(viewportPoint.X + _region.X, viewportPoint.Y + _region.Y);
        /// <summary>
        /// Converts a viewport point relative to actual screen resolution
        /// to a point relative to the internal resolution.
        /// </summary>
        public Vec2 ToInternalResCoords(Vec2 viewportPoint) => viewportPoint * (InternalResolution.Extents / _region.Extents);
        /// <summary>
        /// Converts a viewport point relative to the internal resolution
        /// to a point relative to the actual screen resolution.
        /// </summary>
        public Vec2 FromInternalResCoords(Vec2 viewportPoint) => viewportPoint * (InternalResolution.Extents / _region.Extents);
        #endregion

        #region Picking
        public float GetDepth(Vec2 viewportPoint)
        {
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.ReadFramebuffer, 0);
            Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);
            return Engine.Renderer.GetDepth(viewportPoint.X, viewportPoint.Y);
        }
        public byte GetStencil(Vec2 viewportPoint)
        {
            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.ReadFramebuffer, 0);
            Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);
            return Engine.Renderer.GetStencilIndex(viewportPoint.X, viewportPoint.Y);
        }
        /// <summary>
        /// Returns a ray projected from the given screen location.
        /// </summary>
        public Ray GetWorldRay(Vec2 viewportPoint)
            => _worldCamera.GetWorldRay(ToInternalResCoords(viewportPoint));
        /// <summary>
        /// Returns a segment projected from the given screen location.
        /// Endpoints are located on the NearZ and FarZ planes.
        /// </summary>
        public Segment GetWorldSegment(Vec2 viewportPoint)
            => _worldCamera.GetWorldSegment(ToInternalResCoords(viewportPoint));
        public SceneComponent PickScene(
            Vec2 viewportPoint,
            bool testHud,
            bool testWorld,
            out Vec3 hitNormal,
            out Vec3 hitPoint,
            out float distance,
            params TCollisionObject[] ignored)
        {
            if (testHud)
            {
                if (HUD?.FindDeepestComponent(viewportPoint) is UIComponent hudComp && hudComp.IsVisible)
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

                RayTraceClosest c = new RayTraceClosest(cursor.StartPoint, cursor.EndPoint, 0, 0xFFFF, ignored);
                if (c.Trace())
                {
                    hitNormal = c.HitNormalWorld;
                    hitPoint = c.HitPointWorld;
                    distance = hitPoint.DistanceToFast(cursor.StartPoint);
                    TCollisionObject coll = c.CollisionObject;
                    return coll.Owner as SceneComponent;
                }

                //Vec3 worldPoint = ScreenToWorld(viewportPoint, depth);
                //ThreadSafeList<I3DRenderable> r = Engine.Scene.RenderTree.FindClosest(worldPoint);
            }
            hitNormal = Vec3.Zero;
            hitPoint = Vec3.Zero;
            distance = 0.0f;
            return null;
        }
        #endregion

        #region Viewport Resizing
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
        #endregion

        #region SSAO
        private class SSAOInfo
        {
            Vec2[] _noise;
            Vec3[] _kernel;
            public const int DefaultSamples = 64;
            const int DefaultNoiseWidth = 4, DefaultNoiseHeight = 4;
            const float DefaultMinSampleDist = 0.1f, DefaultMaxSampleDist = 1.0f;
            
            public Vec2[] Noise => _noise;
            public Vec3[] Kernel => _kernel;

            public int Samples { get; private set; }
            public int NoiseWidth { get; private set; }
            public int NoiseHeight { get; private set; }
            public float MinSampleDist { get; private set; }
            public float MaxSampleDist { get; private set; }
            
            public void Generate(
                //int width, int height,
                int samples = DefaultSamples,
                int noiseWidth = DefaultNoiseWidth,
                int noiseHeight = DefaultNoiseHeight,
                float minSampleDist = DefaultMinSampleDist,
                float maxSampleDist = DefaultMaxSampleDist)
            {
                Samples = samples;
                NoiseWidth = noiseWidth;
                NoiseHeight = noiseHeight;
                MinSampleDist = minSampleDist;
                MaxSampleDist = maxSampleDist;

                Random r = new Random();

                _kernel = new Vec3[samples];
                _noise = new Vec2[noiseWidth * noiseHeight];

                float scale;
                Vec3 sample;
                Vec2 noise;

                for (int i = 0; i < samples; ++i)
                {
                    sample = new Vec3(
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble()).Normalized();
                    scale = i / (float)samples;
                    sample *= Interp.Lerp(minSampleDist, maxSampleDist, scale * scale);
                    _kernel[i] = sample;
                }

                for (int i = 0; i < _noise.Length; ++i)
                {
                    noise = new Vec2((float)r.NextDouble(), (float)r.NextDouble());
                    noise.Normalize();
                    _noise[i] = noise;
                }
            }
        }
        #endregion

        #region FBOs
        internal TexRef2D _depthStencilTexture;
        internal TexRef2D _hdrSceneTexture;
        internal TexRef2D _bloomBlurTexture;
        public enum DepthStencilUse
        {
            None,
            Depth24,
            Depth32,
            Depth32f,
            Stencil8,
            Depth24Stencil8,
            Depth32Stencil8,
        }

        private void PrecomputeBRDF()
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)PrecomputeBRDF, BaseRenderPanel.PanelType.Rendering))
                return;

            RenderingParameters renderParams = new RenderingParameters();
            renderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            renderParams.DepthTest.Function = EComparison.Always;
            renderParams.DepthTest.UpdateDepth = false;
            GLSLShaderFile shader = Engine.LoadEngineShader(Path.Combine("Scene3D", "BRDF.fs"), EShaderMode.Fragment);
            BrdfTex = TexRef2D.CreateFrameBufferTexture("BRDF_LUT", 512, 512, EPixelInternalFormat.Rg16f, EPixelFormat.Rg, EPixelType.HalfFloat);
            BrdfTex.Resizeable = true;
            BrdfTex.UWrap = ETexWrapMode.ClampToEdge;
            BrdfTex.VWrap = ETexWrapMode.ClampToEdge;
            BrdfTex.MinFilter = ETexMinFilter.Linear;
            BrdfTex.MagFilter = ETexMagFilter.Linear;

            TexRef2D[] brdfRefs = new TexRef2D[] { BrdfTex };
            TMaterial brdfMat = new TMaterial("BRDFMat", renderParams, brdfRefs, shader);
            MaterialFrameBuffer fbo = new MaterialFrameBuffer(brdfMat);
            fbo.SetRenderTargets((BrdfTex, EFramebufferAttachment.ColorAttachment0, 0, -1));

            PrimitiveData data = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), 
                VertexQuad.MakeQuad( //ndc space quad, so we don't have to load any camera matrices
                    new Vec3(-1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, 1.0f, -0.5f),
                    new Vec3(-1.0f, 1.0f, -0.5f),
                    false, false).ToTriangles());
            PrimitiveManager quad = new PrimitiveManager(data, brdfMat);
            BoundingRectangle region = new BoundingRectangle(Vec2.Zero, new Vec2(512.0f, 512.0f));
            
            fbo.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(region);
            {
                Engine.Renderer.Clear(EBufferClear.Color);
                quad.Render();
            }
            Engine.Renderer.PopRenderArea();
            fbo.Unbind(EFramebufferTarget.DrawFramebuffer);
        }

        //private TexRef2D[] _fboTextures;
        internal unsafe void InitFBOs()
        {
            //ForwardPassFBO?.Destroy();
            //GBufferFBO?.Destroy();
            //PingPongBloomBlurFBO?.Destroy();
            //PostProcessFBO?.Destroy();
            //SSAOBlurFBO?.Destroy();
            //SSAOFBO?.Destroy();

            int width = InternalResolution.IntWidth;
            int height = InternalResolution.IntHeight;
            const string SceneShaderPath = "Scene3D";

            RenderingParameters renderParams = new RenderingParameters();
            renderParams.DepthTest.Enabled = ERenderParamUsage.Unchanged;
            renderParams.DepthTest.UpdateDepth = false;
            renderParams.DepthTest.Function = EComparison.Always;
            
            _depthStencilTexture = TexRef2D.CreateFrameBufferTexture("DepthStencil", width, height,
                EPixelInternalFormat.Depth24Stencil8, EPixelFormat.DepthStencil, EPixelType.UnsignedInt248,
                EFramebufferAttachment.DepthStencilAttachment);
            _depthStencilTexture.MinFilter = ETexMinFilter.Nearest;
            _depthStencilTexture.MagFilter = ETexMagFilter.Nearest;
            _depthStencilTexture.Resizeable = false;

            TexRefView2D depthViewTexture = new TexRefView2D(_depthStencilTexture, 0, 1, 0, 1,
                EPixelType.UnsignedInt248, EPixelFormat.DepthStencil, EPixelInternalFormat.Depth24Stencil8)
            {
                //Resizeable = false,
                DepthStencilFormat = EDepthStencilFmt.Depth,
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
            };

            TexRefView2D stencilViewTexture = new TexRefView2D(_depthStencilTexture, 0, 1, 0, 1,
                EPixelType.UnsignedInt248, EPixelFormat.DepthStencil, EPixelInternalFormat.Depth24Stencil8)
            {
                //Resizeable = false,
                DepthStencilFormat = EDepthStencilFmt.Stencil,
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
            };

            #region Deferred

            //If forward, we can render directly to the post process FBO.
            //If deferred, we have to render to a quad first, then render that to post process
            if (Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred)
            {
                #region SSAO Noise
                TexRef2D ssaoNoise = new TexRef2D("SSAONoise",
                    _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight,
                    EPixelInternalFormat.Rg32f, EPixelFormat.Rg, EPixelType.Float,
                    PixelFormat.Format64bppArgb)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Repeat,
                    VWrap = ETexWrapMode.Repeat,
                    Resizeable = true,
                };
                Bitmap bmp = ssaoNoise.Mipmaps[0].File.Bitmaps[0];
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);
                Vec2* values = (Vec2*)data.Scan0;
                Vec2[] noise = _ssaoInfo.Noise;
                foreach (Vec2 v in noise)
                    *values++ = v;
                bmp.UnlockBits(data);
                #endregion

                TexRef2D albedoOpacityTexture = TexRef2D.CreateFrameBufferTexture("AlbedoOpacity", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat);
                TexRef2D normalTexture = TexRef2D.CreateFrameBufferTexture("Normal", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat);
                TexRef2D rmsiTexture = TexRef2D.CreateFrameBufferTexture("RoughnessMetallicSpecularUnused", width, height,
                    EPixelInternalFormat.Rgba8, EPixelFormat.Rgba, EPixelType.UnsignedByte);
                TexRef2D ssaoTexture = TexRef2D.CreateFrameBufferTexture("OutputIntensity", width, height,
                    EPixelInternalFormat.R16f, EPixelFormat.Red, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment0);
                ssaoTexture.MinFilter = ETexMinFilter.Linear;
                ssaoTexture.MagFilter = ETexMagFilter.Linear;
                
                GLSLShaderFile ssaoShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "SSAOGen.fs"), EShaderMode.Fragment);
                GLSLShaderFile ssaoBlurShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "SSAOBlur.fs"), EShaderMode.Fragment);
                GLSLShaderFile deferredShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLighting.fs"), EShaderMode.Fragment);
                
                TexRef2D[] ssaoRefs = new TexRef2D[]
                {
                    normalTexture,
                    ssaoNoise,
                    depthViewTexture,
                };
                TexRef2D[] ssaoBlurRefs = new TexRef2D[]
                {
                    ssaoTexture
                };
                TexRef2D[] deferredLightingRefs = new TexRef2D[]
                {
                    albedoOpacityTexture,
                    normalTexture,
                    rmsiTexture,
                    ssaoTexture,
                    depthViewTexture,
                };

                TMaterial ssaoMat = new TMaterial("SSAOMat", renderParams, ssaoRefs, ssaoShader);
                TMaterial ssaoBlurMat = new TMaterial("SSAOBlurMat", renderParams, ssaoBlurRefs, ssaoBlurShader);
                TMaterial deferredMat = new TMaterial("DeferredLightingMaterial", TMaterial.UniformRequirements.NeedsLights,
                    renderParams, deferredLightingRefs, deferredShader);

                SSAOFBO = new QuadFrameBuffer(ssaoMat);
                SSAOFBO.SettingUniforms += SSAO_SetUniforms;
                SSAOFBO.SetRenderTargets(
                    (albedoOpacityTexture, EFramebufferAttachment.ColorAttachment0, 0, -1),
                    (normalTexture, EFramebufferAttachment.ColorAttachment1, 0, -1),
                    (rmsiTexture, EFramebufferAttachment.ColorAttachment2, 0, -1),
                    (_depthStencilTexture, EFramebufferAttachment.DepthStencilAttachment, 0, -1));

                SSAOBlurFBO = new QuadFrameBuffer(ssaoBlurMat);
                GBufferFBO = new QuadFrameBuffer(deferredMat);
                //GBufferFBO.SettingUniforms += GBuffer_SetUniforms;
                GBufferFBO.SetRenderTargets((ssaoTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));

                #region Light Meshes

                BlendMode additiveBlend = new BlendMode()
                {
                    //Add the previous and current light colors together using FuncAdd with each mesh render
                    Enabled = ERenderParamUsage.Enabled,
                    RgbDstFactor = EBlendingFactor.One,
                    AlphaDstFactor = EBlendingFactor.One,
                    RgbSrcFactor = EBlendingFactor.One,
                    AlphaSrcFactor = EBlendingFactor.One,
                    RgbEquation = EBlendEquationMode.FuncAdd,
                    AlphaEquation = EBlendEquationMode.FuncAdd,
                };
                RenderingParameters lightRenderParams = new RenderingParameters
                {
                    //Render only the backside so that the light still shows if the camera is inside of the volume
                    //and the light does not add itself twice for the front and back faces.
                    CullMode = Culling.Front,

                    BlendMode = additiveBlend,
                };
                lightRenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                RenderingParameters dirLightRenderParams = new RenderingParameters
                {
                    //Render only the front of the quad that shows over the whole screen
                    CullMode = Culling.Back,

                    BlendMode = additiveBlend,
                };
                dirLightRenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;

                TexRef2D diffuseTex = TexRef2D.CreateFrameBufferTexture("Diffuse", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat);
                GLSLShaderFile lightCombineShader = Engine.LoadEngineShader(
                    Path.Combine(SceneShaderPath, "DeferredLightCombine.fs"), EShaderMode.Fragment);
                BaseTexRef[] combineRefs = new BaseTexRef[]
                {
                    albedoOpacityTexture,
                    normalTexture,
                    rmsiTexture,
                    ssaoTexture,
                    depthViewTexture,
                    diffuseTex,
                    BrdfTex,
                    //irradiance
                    //prefilter
                };
                TMaterial lightCombineMat = new TMaterial("LightCombineMat", renderParams, combineRefs, lightCombineShader);
                LightCombineFBO = new QuadFrameBuffer(lightCombineMat);
                LightCombineFBO.SetRenderTargets((diffuseTex, EFramebufferAttachment.ColorAttachment0, 0, -1));
                LightCombineFBO.SettingUniforms += LightCombineFBO_SettingUniforms;

                TexRef2D[] lightRefs = new TexRef2D[]
                {
                    albedoOpacityTexture,
                    normalTexture,
                    rmsiTexture,
                    depthViewTexture,
                    //shadow map texture
                };

                GLSLShaderFile dirLightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLightingDir.fs"), EShaderMode.Fragment);
                GLSLShaderFile pointLightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLightingPoint.fs"), EShaderMode.Fragment);
                GLSLShaderFile spotLightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLightingSpot.fs"), EShaderMode.Fragment);

                TMaterial pointLightMat = new TMaterial("PointLightMat", TMaterial.UniformRequirements.NeedsCamera, lightRenderParams, lightRefs, pointLightShader);
                PrimitiveData pointLightMesh = Sphere.SolidMesh(Vec3.Zero, 1.0f, 20u);
                PointLightManager = new PrimitiveManager(pointLightMesh, pointLightMat);
                PointLightManager.SettingUniforms += LightManager_SettingUniforms;

                TMaterial spotLightMat = new TMaterial("SpotLightMat", TMaterial.UniformRequirements.NeedsCamera, lightRenderParams, lightRefs, spotLightShader);
                PrimitiveData spotLightMesh = BaseCone.SolidMesh(Vec3.Zero, Vec3.UnitZ, 1.0f, 1.0f, 32, true);
                SpotLightManager = new PrimitiveManager(spotLightMesh, spotLightMat);
                SpotLightManager.SettingUniforms += LightManager_SettingUniforms;
                
                TMaterial dirLightMat = new TMaterial("DirLightMat", TMaterial.UniformRequirements.NeedsCamera, dirLightRenderParams, lightRefs, dirLightShader);
                DirLightFBO = new QuadFrameBuffer(dirLightMat);
                DirLightFBO.FullScreenTriangle.SettingUniforms += LightManager_SettingUniforms;

                #endregion
            }

            #endregion

            #region Forward
            
            _bloomBlurTexture = TexRef2D.CreateFrameBufferTexture("OutputColor", width, height,
                EPixelInternalFormat.Rgb8, EPixelFormat.Rgb, EPixelType.UnsignedByte);
            _bloomBlurTexture.MagFilter = ETexMagFilter.Linear;
            _bloomBlurTexture.MinFilter = ETexMinFilter.LinearMipmapLinear;
            _bloomBlurTexture.UWrap = ETexWrapMode.ClampToEdge;
            _bloomBlurTexture.VWrap = ETexWrapMode.ClampToEdge;
            
            _hdrSceneTexture = TexRef2D.CreateFrameBufferTexture("HDRSceneColor", width, height,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat,
                EFramebufferAttachment.ColorAttachment0);
            //_hdrSceneTexture.Resizeable = false;
            _hdrSceneTexture.UWrap = ETexWrapMode.ClampToEdge;
            _hdrSceneTexture.VWrap = ETexWrapMode.ClampToEdge;
            _hdrSceneTexture.MinFilter = ETexMinFilter.Nearest;
            _hdrSceneTexture.MagFilter = ETexMagFilter.Nearest;

            GLSLShaderFile brightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "BrightPass.fs"), EShaderMode.Fragment);
            GLSLShaderFile bloomBlurShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "BloomBlur.fs"), EShaderMode.Fragment);
            GLSLShaderFile postProcessShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "PostProcess.fs"), EShaderMode.Fragment);
            GLSLShaderFile hudShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "HudFBO.fs"), EShaderMode.Fragment);
            
            TexRef2D[] brightRefs = new TexRef2D[]
            {
                _hdrSceneTexture
            };
            TexRef2D[] blurRefs = new TexRef2D[]
            {
                _bloomBlurTexture,
            };
            TexRef2D[] postProcessRefs = new TexRef2D[]
            {
                _hdrSceneTexture,
                _bloomBlurTexture,
                depthViewTexture,
                stencilViewTexture,
            };
            TexRef2D hudTexture = TexRef2D.CreateFrameBufferTexture("Hud", width, height,
                EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat, EFramebufferAttachment.ColorAttachment0);
            hudTexture.MinFilter = ETexMinFilter.Nearest;
            hudTexture.MagFilter = ETexMagFilter.Nearest;
            hudTexture.UWrap = ETexWrapMode.ClampToEdge;
            hudTexture.VWrap = ETexWrapMode.ClampToEdge;
            TexRef2D[] hudRefs = new TexRef2D[]
            {
                hudTexture,
            };
            ShaderVar[] blurVars = new ShaderVar[]
            {
                new ShaderFloat(0.0f, "Ping"),
                new ShaderInt(0, "LOD"),
            };
            TMaterial brightMat = new TMaterial("BrightPassMat", renderParams, brightRefs, brightShader);
            TMaterial bloomBlurMat = new TMaterial("BloomBlurMat", renderParams, blurVars, blurRefs, bloomBlurShader);
            TMaterial postProcessMat = new TMaterial("PostProcessMat", renderParams, postProcessRefs, postProcessShader);
            TMaterial hudMat = new TMaterial("HudMat", renderParams, postProcessRefs, hudShader);

            BrightPassFBO = new QuadFrameBuffer(brightMat);
            BrightPassFBO.SettingUniforms += BrightPassFBO_SettingUniforms;
            BrightPassFBO.SetRenderTargets(
                (_hdrSceneTexture, EFramebufferAttachment.ColorAttachment0, 0, -1),
                (_depthStencilTexture, EFramebufferAttachment.DepthStencilAttachment, 0, -1));

            BloomBlurFBO1 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO1.SetRenderTargets((_bloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));
            BloomBlurFBO2 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO2.SetRenderTargets((_bloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 1, -1));
            BloomBlurFBO4 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO4.SetRenderTargets((_bloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 2, -1));
            BloomBlurFBO8 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO8.SetRenderTargets((_bloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 3, -1));
            BloomBlurFBO16 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO16.SetRenderTargets((_bloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 4, -1));

            PostProcessFBO = new QuadFrameBuffer(postProcessMat);
            PostProcessFBO.SettingUniforms += _postProcess_SettingUniforms;

            HudFBO = new QuadFrameBuffer(hudMat);

            #endregion
        }

        internal LightComponent _lightComp;

        private void LightManager_SettingUniforms(int vertexBindingId, int fragGeomBindingId)
        {
            if (RenderingCamera == null)
                return;
            RenderingCamera.SetUniforms(fragGeomBindingId);
            RenderingCamera.PostProcessRef.File.Shadows.SetUniforms(fragGeomBindingId);
            _lightComp.SetUniforms(fragGeomBindingId);
        }
        private void LightCombineFBO_SettingUniforms(int programBindingId)
        {
            if (RenderingCamera == null)
                return;

            RenderingCamera.SetUniforms(programBindingId);

            var probeActor = RenderingCamera.OwningComponent?.OwningScene?.IBLProbeActor;
            if (probeActor == null)
                return;

            IBLProbeComponent probe = (IBLProbeComponent)probeActor.RootComponent.ChildComponents[0];
            int baseCount = LightCombineFBO.Material.Textures.Length;

            if (probe.IrradianceTex != null)
                TMaterialBase.SetTextureUniform(probe.IrradianceTex.GetTexture(true),
                    baseCount, "Texture" + baseCount.ToString(), programBindingId);
            ++baseCount;
            if (probe.PrefilterTex != null)
                TMaterialBase.SetTextureUniform(probe.PrefilterTex.GetTexture(true),
                    baseCount, "Texture" + baseCount.ToString(), programBindingId);
        }

        private void BrightPassFBO_SettingUniforms(int programBindingId)
            => RenderingCamera?.PostProcessRef.File.Bloom.SetUniforms(programBindingId);

        //private void GBuffer_SetUniforms(int programBindingId)
        //{
        //    if (RenderingCamera == null)
        //        return;

        //    RenderingCamera.SetUniforms(programBindingId);
        //    //RenderingCamera.PostProcessRef.File.Shadows.SetUniforms(programBindingId);

        //    //var probeActor = _worldCamera.OwningComponent?.OwningScene?.IBLProbeActor;
        //    //if (probeActor == null)
        //    //    return;

        //    //IBLProbeComponent probe = (IBLProbeComponent)probeActor.RootComponent.ChildComponents[0];
        //    //int baseCount = GBufferFBO.Material.Textures.Length;

        //    //TMaterialBase.SetTextureUniform(_brdfTex.GetTexture(true),
        //    //    baseCount, "Texture" + baseCount.ToString(), programBindingId);
        //    //++baseCount;
        //    //if (probe.IrradianceTex != null)
        //    //    TMaterialBase.SetTextureUniform(probe.IrradianceTex.GetTexture(true),
        //    //        baseCount, "Texture" + baseCount.ToString(), programBindingId);
        //    //++baseCount;
        //    //if (probe.PrefilterTex != null)
        //    //    TMaterialBase.SetTextureUniform(probe.PrefilterTex.GetTexture(true),
        //    //        baseCount, "Texture" + baseCount.ToString(), programBindingId);
        //}
        
        private void SSAO_SetUniforms(int programBindingId)
        {
            if (RenderingCamera == null)
                return;
            Engine.Renderer.Uniform(programBindingId, "NoiseScale", InternalResolution.Extents / 4.0f);
            Engine.Renderer.Uniform(programBindingId, "Samples", _ssaoInfo.Kernel.Select(x => (IUniformable3Float)x).ToArray());
            RenderingCamera.SetUniforms(programBindingId);
            RenderingCamera.PostProcessRef.File.AmbientOcclusion.SetUniforms(programBindingId);
        }

        private void _postProcess_SettingUniforms(int programBindingId)
        {
            if (RenderingCamera == null)
                return;

            RenderingCamera.SetUniforms(programBindingId);
            RenderingCamera.PostProcessRef.File.ColorGrading.UpdateExposure(_hdrSceneTexture);
            RenderingCamera.PostProcessRef.File.SetUniforms(programBindingId);

            //var probeActor = _worldCamera.OwningComponent?.OwningScene?.IBLProbeActor;
            //if (probeActor == null)
            //    return;

            //IBLProbeComponent probe = (IBLProbeComponent)probeActor.RootComponent.ChildComponents[0];

            //TMaterialBase.SetTextureUniform(_brdfTex.GetTexture(true),
            //    4, "Texture4", programBindingId);

            //if (probe.ResultTexture != null)
            //    TMaterialBase.SetTextureUniform(probe.ResultTexture.GetTexture(true),
            //        5, "Texture5", programBindingId);

            //if (probe.PrefilterTex != null)
            //    TMaterialBase.SetTextureUniform(probe.PrefilterTex.GetTexture(true),
            //        6, "Texture6", programBindingId);
        }

        #endregion
    }
}
