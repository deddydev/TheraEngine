﻿using System;
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

        private BoundingRectangle _region;
        private Camera _worldCamera;
        private SSAOInfo _ssaoInfo = new SSAOInfo();

        internal QuadFrameBuffer SSAOFBO;
        internal QuadFrameBuffer SSAOBlurFBO;
        internal FrameBuffer GBufferFBO;
        internal QuadFrameBuffer BloomBlurFBO1;
        internal QuadFrameBuffer BloomBlurFBO2;
        internal QuadFrameBuffer BloomBlurFBO4;
        internal QuadFrameBuffer BloomBlurFBO8;
        internal QuadFrameBuffer BloomBlurFBO16;
        internal QuadFrameBuffer LightCombineFBO;
        internal QuadFrameBuffer ForwardPassFBO;
        internal QuadFrameBuffer PostProcessFBO;
        internal QuadFrameBuffer HudFBO;
        internal PrimitiveManager PointLightManager;
        internal PrimitiveManager SpotLightManager;
        internal PrimitiveManager DirLightManager;
        //internal QuadFrameBuffer DirLightFBO;
        internal Camera RenderingCamera => RenderingCameras.Peek();
        internal Stack<Camera> RenderingCameras { get; } = new Stack<Camera>();
        internal TexRef2D BrdfTex = null;
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
                        p.Aspect = (float)Width / Height;
                }
            }
        }

        //public BaseRenderPanel OwningPanel => _owningPanel;
        public BoundingRectangle Region => _region;
        public int Height { get => _region.Height; set => _region.Height = value; }
        public int Width { get => _region.Width; set => _region.Width = value; }
        public int X { get => _region.X; set => _region.X = value; }
        public int Y { get => _region.Y; set => _region.Y = value; }
        public IVec2 Position { get => _region.OriginTranslation; set => _region.OriginTranslation = value; }
        public int Index { get; private set; }

        internal bool RegeneratingFBOs = false;

        public List<LocalPlayerController> Owners { get; } = new List<LocalPlayerController>();

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

        public BaseRenderPanel OwningPanel { get; }

        public Viewport(BaseRenderPanel panel, int index)
        {
            if (index == 0)
            {
                Index = index;
                SetFullScreen();
            }
            else
                ViewportCountChanged(index, panel.Viewports.Count + 1, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);

            OwningPanel = panel;
            Index = index;
            _ssaoInfo.Generate();
            PrecomputeBRDF();
            Resize(panel.Width, panel.Height);
        }
        public Viewport(int width, int height)
        {
            Index = 0;
            SetFullScreen();
            _ssaoInfo.Generate();
            PrecomputeBRDF();
            Resize(width, height);
        }
        public void SetInternalResolution(int width, int height)
        {
            _internalResolution.Width = width;
            _internalResolution.Height = height;
            
            InitFBOs();

            _worldCamera?.Resize(width, height);
        }

        private void ClearFBOs()
        {
            BloomBlurFBO1?.Destroy();
            BloomBlurFBO1 = null;
            BloomBlurFBO2?.Destroy();
            BloomBlurFBO2 = null;
            BloomBlurFBO4?.Destroy();
            BloomBlurFBO4 = null;
            BloomBlurFBO8?.Destroy();
            BloomBlurFBO8 = null;
            BloomBlurFBO16?.Destroy();
            BloomBlurFBO16 = null;
            ForwardPassFBO?.Destroy();
            ForwardPassFBO = null;
            //DirLightFBO?.Destroy();
            //DirLightFBO = null;
            GBufferFBO?.Destroy();
            GBufferFBO = null;
            HudFBO?.Destroy();
            HudFBO = null;
            LightCombineFBO?.Destroy();
            LightCombineFBO = null;
            PostProcessFBO?.Destroy();
            PostProcessFBO = null;
            SSAOBlurFBO?.Destroy();
            SSAOBlurFBO = null;
            SSAOFBO?.Destroy();
            SSAOFBO = null;
        }
        internal void GenerateFBOs()
        {
            DateTime start = DateTime.Now;
            //RenderQuery query = new RenderQuery();
            //query.BeginQuery(EQueryTarget.TimeElapsed);
            BloomBlurFBO1?.Generate();
            BloomBlurFBO2?.Generate();
            BloomBlurFBO4?.Generate();
            BloomBlurFBO8?.Generate();
            BloomBlurFBO16?.Generate();
            ForwardPassFBO?.Generate();
            //DirLightFBO?.Generate();
            GBufferFBO?.Generate();
            HudFBO?.Generate();
            LightCombineFBO?.Generate();
            PostProcessFBO?.Generate();
            SSAOBlurFBO?.Generate();
            SSAOFBO?.Generate();
            //query.EndQuery(EQueryTarget.TimeElapsed);
            //int time = query.GetQueryObjectInt(EGetQueryObject.QueryResult);
            TimeSpan span = DateTime.Now - start;
            Engine.PrintLine($"FBO regeneration took {span.Seconds} seconds.");
        }

        public void Resize(
            int parentWidth,
            int parentHeight,
            bool setInternalResolution = true,
            float internalResolutionWidthScale = 1.0f,
            float internalResolutionHeightScale = 1.0f)
        {
            float w = parentWidth.ClampMin(1);
            float h = parentHeight.ClampMin(1);

            _region.X = (int)(_leftPercentage * w);
            _region.Y = (int)(_bottomPercentage * h);
            _region.Width = (int)(_rightPercentage * w - _region.X);
            _region.Height = (int)(_topPercentage * h - _region.Y);
            
            if (setInternalResolution) SetInternalResolution(
                (int)(_region.Width * internalResolutionWidthScale),
                (int)(_region.Height * internalResolutionHeightScale));

            HUD?.Resize(_region.Extents);
            if (Camera is PerspectiveCamera p)
                p.Aspect = (float)_region.Width / _region.Height;
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
            if (scene == null || scene.Count == 0 || RegeneratingFBOs)
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
        internal protected virtual void SwapBuffers(BaseScene scene)
        {
            _renderPasses.SwapBuffers();
            scene?.PreRenderSwap();
            HUD?.RenderPasses.SwapBuffers();
        }
        public void Update(BaseScene scene, Camera camera, Frustum frustum)
        {
            scene?.Update(_renderPasses, frustum, camera, false);
            HUD?.UIScene?.Update(HUD.RenderPasses, null, HUD.Camera, false);
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

        internal void RenderDirLight(DirectionalLightComponent c)
        {
            _lightComp = c;
            DirLightManager.Render(c.LightMatrix);
        }
        internal void RenderPointLight(PointLightComponent c)
        {
            _lightComp = c;
            PointLightManager.Render(c.LightMatrix);
        }
        internal void RenderSpotLight(SpotLightComponent c)
        {
            _lightComp = c;
            SpotLightManager.Render(c.LightMatrix);
        }

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
            Index = newIndex;
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

        private void PrecomputeBRDF(int width = 512, int height = 512)
        {
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action<int, int>)PrecomputeBRDF, BaseRenderPanel.PanelType.Rendering, width, height))
                return;

            RenderingParameters renderParams = new RenderingParameters();
            renderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            renderParams.DepthTest.Function = EComparison.Always;
            renderParams.DepthTest.UpdateDepth = false;

            BrdfTex = TexRef2D.CreateFrameBufferTexture("BRDF_LUT", width, height, EPixelInternalFormat.Rg16f, EPixelFormat.Rg, EPixelType.HalfFloat);
            BrdfTex.Resizeable = true;
            BrdfTex.UWrap = ETexWrapMode.ClampToEdge;
            BrdfTex.VWrap = ETexWrapMode.ClampToEdge;
            BrdfTex.MinFilter = ETexMinFilter.Linear;
            BrdfTex.MagFilter = ETexMagFilter.Linear;
            TexRef2D[] texRefs = new TexRef2D[] { BrdfTex };

            GLSLShaderFile shader = Engine.LoadEngineShader(Path.Combine("Scene3D", "BRDF.fs"), EShaderMode.Fragment);
            TMaterial mat = new TMaterial("BRDFMat", renderParams, texRefs, shader);
            MaterialFrameBuffer fbo = new MaterialFrameBuffer(mat);
            fbo.SetRenderTargets((BrdfTex, EFramebufferAttachment.ColorAttachment0, 0, -1));

            PrimitiveData data = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), 
                VertexQuad.MakeQuad( //ndc space quad, so we don't have to load any camera matrices
                    new Vec3(-1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, 1.0f, -0.5f),
                    new Vec3(-1.0f, 1.0f, -0.5f),
                    false, false).ToTriangles());
            PrimitiveManager quad = new PrimitiveManager(data, mat);

            BoundingRectangle region = new BoundingRectangle(IVec2.Zero, new IVec2(width, height));
            
            //Now render the texture to the FBO using the quad
            fbo.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(region);
            {
                Engine.Renderer.Clear(EBufferClear.Color);
                quad.Render();
            }
            Engine.Renderer.PopRenderArea();
            fbo.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        
        internal unsafe void InitFBOs()
        {
            RegeneratingFBOs = true;
            if (BaseRenderPanel.ThreadSafeBlockingInvoke((Action)InitFBOs, BaseRenderPanel.PanelType.Rendering))
                return;

            ClearFBOs();

            if (BrdfTex == null)
                PrecomputeBRDF();
            
            int width = InternalResolution.Width;
            int height = InternalResolution.Height;
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
                Resizeable = false,
                DepthStencilFormat = EDepthStencilFmt.Depth,
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
            };

            TexRefView2D stencilViewTexture = new TexRefView2D(_depthStencilTexture, 0, 1, 0, 1,
                EPixelType.UnsignedInt248, EPixelFormat.DepthStencil, EPixelInternalFormat.Depth24Stencil8)
            {
                Resizeable = false,
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
                    Resizeable = false,
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
                //TexRef2D[] deferredLightingRefs = new TexRef2D[]
                //{
                //    albedoOpacityTexture,
                //    normalTexture,
                //    rmsiTexture,
                //    ssaoTexture,
                //    depthViewTexture,
                //};

                TMaterial ssaoMat = new TMaterial("SSAOMat", renderParams, ssaoRefs, ssaoShader);
                TMaterial ssaoBlurMat = new TMaterial("SSAOBlurMat", renderParams, ssaoBlurRefs, ssaoBlurShader);
                
                SSAOFBO = new QuadFrameBuffer(ssaoMat);
                SSAOFBO.SettingUniforms += SSAO_SetUniforms;
                SSAOFBO.SetRenderTargets(
                    (albedoOpacityTexture, EFramebufferAttachment.ColorAttachment0, 0, -1),
                    (normalTexture, EFramebufferAttachment.ColorAttachment1, 0, -1),
                    (rmsiTexture, EFramebufferAttachment.ColorAttachment2, 0, -1),
                    (_depthStencilTexture, EFramebufferAttachment.DepthStencilAttachment, 0, -1));

                SSAOBlurFBO = new QuadFrameBuffer(ssaoBlurMat);
                GBufferFBO = new FrameBuffer();
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
                    CullMode = ECulling.Front,
                    Requirements = EUniformRequirements.Camera,
                    BlendMode = additiveBlend,
                };
                lightRenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                //RenderingParameters dirLightRenderParams = new RenderingParameters
                //{
                //    //Render only the front of the quad that shows over the whole screen
                //    CullMode = ECulling.Back,
                //    Requirements = EUniformRequirements.Camera,
                //    BlendMode = additiveBlend,
                //};
                //dirLightRenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;

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

                GLSLShaderFile pointLightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLightingPoint.fs"), EShaderMode.Fragment);
                GLSLShaderFile spotLightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLightingSpot.fs"), EShaderMode.Fragment);
                GLSLShaderFile dirLightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "DeferredLightingDir.fs"), EShaderMode.Fragment);

                TMaterial pointLightMat = new TMaterial("PointLightMat", lightRenderParams, lightRefs, pointLightShader);
                TMaterial spotLightMat = new TMaterial("SpotLightMat", lightRenderParams, lightRefs, spotLightShader);
                TMaterial dirLightMat = new TMaterial("DirLightMat", lightRenderParams, lightRefs, dirLightShader);

                PrimitiveData pointLightMesh = Sphere.SolidMesh(Vec3.Zero, 1.0f, 20u);
                PrimitiveData spotLightMesh = BaseCone.SolidMesh(Vec3.Zero, Vec3.UnitZ, 1.0f, 1.0f, 32, true);
                PrimitiveData dirLightMesh = BoundingBox.SolidMesh(-Vec3.Half, Vec3.Half);

                PointLightManager = new PrimitiveManager(pointLightMesh, pointLightMat);
                PointLightManager.SettingUniforms += LightManager_SettingUniforms;

                SpotLightManager = new PrimitiveManager(spotLightMesh, spotLightMat);
                SpotLightManager.SettingUniforms += LightManager_SettingUniforms;
                
                DirLightManager = new PrimitiveManager(dirLightMesh, dirLightMat);
                DirLightManager.SettingUniforms += LightManager_SettingUniforms;

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
                EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat,
                EFramebufferAttachment.ColorAttachment0);
            //_hdrSceneTexture.Resizeable = false;
            _hdrSceneTexture.MinFilter = ETexMinFilter.Nearest;
            _hdrSceneTexture.MagFilter = ETexMagFilter.Nearest;
            _hdrSceneTexture.UWrap = ETexWrapMode.ClampToEdge;
            _hdrSceneTexture.VWrap = ETexWrapMode.ClampToEdge;

            GLSLShaderFile brightShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "BrightPass.fs"), EShaderMode.Fragment);
            GLSLShaderFile bloomBlurShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "BloomBlur.fs"), EShaderMode.Fragment);
            GLSLShaderFile postProcessShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "PostProcess.fs"), EShaderMode.Fragment);
            GLSLShaderFile hudShader = Engine.LoadEngineShader(Path.Combine(SceneShaderPath, "HudFBO.fs"), EShaderMode.Fragment);

            TexRef2D hudTexture = TexRef2D.CreateFrameBufferTexture("Hud", width, height,
                EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat, 
                EFramebufferAttachment.ColorAttachment0);
            hudTexture.MinFilter = ETexMinFilter.Nearest;
            hudTexture.MagFilter = ETexMagFilter.Nearest;
            hudTexture.UWrap = ETexWrapMode.ClampToEdge;
            hudTexture.VWrap = ETexWrapMode.ClampToEdge;

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
                hudTexture,
            };
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
            TMaterial hudMat = new TMaterial("HudMat", renderParams, hudRefs, hudShader);

            ForwardPassFBO = new QuadFrameBuffer(brightMat);
            ForwardPassFBO.SettingUniforms += BrightPassFBO_SettingUniforms;
            ForwardPassFBO.SetRenderTargets(
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

            RegeneratingFBOs = false;
        }

        private LightComponent _lightComp;

        private void LightManager_SettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            if (RenderingCamera == null)
                return;
            RenderingCamera.PostProcessRef.File.Shadows.SetUniforms(materialProgram);
            _lightComp.SetUniforms(materialProgram);
        }
        private void LightCombineFBO_SettingUniforms(RenderProgram program)
        {
            if (RenderingCamera == null)
                return;

            RenderingCamera.SetUniforms(program);

            var probeActor = RenderingCamera.OwningComponent?.OwningScene?.IBLProbeActor;
            if (probeActor == null)
                return;

            IBLProbeComponent probe = (IBLProbeComponent)probeActor.RootComponent.ChildComponents[0];
            int baseCount = LightCombineFBO.Material.Textures.Length;

            if (probe.IrradianceTex != null)
                TMaterialBase.SetTextureUniform(probe.IrradianceTex.GetTexture(true),
                    baseCount, "Texture" + baseCount.ToString(), program);
            ++baseCount;
            if (probe.PrefilterTex != null)
                TMaterialBase.SetTextureUniform(probe.PrefilterTex.GetTexture(true),
                    baseCount, "Texture" + baseCount.ToString(), program);
        }

        private void BrightPassFBO_SettingUniforms(RenderProgram program)
            => RenderingCamera?.PostProcessRef.File.Bloom.SetUniforms(program);
        
        private void SSAO_SetUniforms(RenderProgram program)
        {
            if (RenderingCamera == null)
                return;
            program.Uniform("NoiseScale", InternalResolution.Extents / 4.0f);
            program.Uniform("Samples", _ssaoInfo.Kernel.Select(x => (IUniformable3Float)x).ToArray());
            RenderingCamera.SetUniforms(program);
            RenderingCamera.PostProcessRef.File.AmbientOcclusion.SetUniforms(program);
        }

        private void _postProcess_SettingUniforms(RenderProgram program)
        {
            if (RenderingCamera == null)
                return;

            RenderingCamera.SetUniforms(program);
            RenderingCamera.PostProcessRef.File.ColorGrading.UpdateExposure(_hdrSceneTexture);
            RenderingCamera.PostProcessRef.File.SetUniforms(program);
        }

        #endregion
    }
}
