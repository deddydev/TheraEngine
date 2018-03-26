﻿using TheraEngine.Input;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.UI;
using System;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using TheraEngine.Core.Shapes;
using System.Collections.Generic;
using TheraEngine.Components.Scene;
using TheraEngine.Components;
using TheraEngine.Actors.Types.Pawns;
using System.IO;
using TheraEngine.Physics;
using TheraEngine.Physics.RayTracing;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering
{
    public class Viewport
    {
        public const int GBufferTextureCount = 5;

        private static Stack<Viewport> CurrentlyRenderingViewports { get; } = new Stack<Viewport>();
        public static Viewport CurrentlyRendering => CurrentlyRenderingViewports.Peek();

        private List<LocalPlayerController> _owners = new List<LocalPlayerController>();
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private BaseRenderPanel _owningPanel;
        
        private SSAOInfo _ssaoInfo = new SSAOInfo();
        internal QuadFrameBuffer GBufferFBO;
        internal PingPongFrameBuffer PingPongBloomBlurFBO;
        internal QuadFrameBuffer SSAOBlurFBO;
        internal QuadFrameBuffer ForwardPassFBO;
        internal QuadFrameBuffer PostProcessFBO;

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
                if (_worldCamera != null)
                {
                    if (_worldCamera.OwningComponent != null)
                        _worldCamera.OwningComponent.WorldTransformChanged -= CameraTransformChanged;
                    else
                        _worldCamera.TransformChanged -= CameraTransformChanged;

                    _worldCamera.OwningComponentChanged -= WorldCameraOwningComponentChanged;

                    _worldCamera.Viewports.Remove(this);
                }
                _worldCamera = value;

                Engine.PrintLine("Updated viewport " + _index + " camera: " + (_worldCamera == null ? "null" : _worldCamera.GetType().GetFriendlyName()));

                if (_worldCamera != null)
                {
                    _worldCamera.Viewports.Add(this);

                    if (_worldCamera.OwningComponent != null)
                        _worldCamera.OwningComponent.WorldTransformChanged += CameraTransformChanged;
                    else
                        _worldCamera.TransformChanged += CameraTransformChanged;

                    _worldCamera.OwningComponentChanged += WorldCameraOwningComponentChanged;

                    //TODO: what if the same camera is used by multiple viewports?
                    //Need to use a separate projection matrix per viewport instead of passing the width and height to the camera itself
                    _worldCamera.Resize(_internalResolution.Width, _internalResolution.Height);
                    if (_worldCamera is PerspectiveCamera p)
                        p.Aspect = Width / Height;

                    CameraTransformChanged();
                }
            }
        }

        private void WorldCameraOwningComponentChanged(CameraComponent previous, CameraComponent current)
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
            //if ((Owners.Count == 0 || Camera == null) && Engine.Audio == null)
            //    return;

            //Vec3 forward = _worldCamera.ForwardVector;
            //Vec3 up = _worldCamera.UpVector;
            //Engine.Audio.UpdateListener(_worldCamera.WorldPoint, forward, up, Vec3.Zero, 0.5f);
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

                Engine.PrintLine("Updated viewport " + _index + " HUD: " + (_hud == null ? "null" : _hud.GetType().GetFriendlyName()));
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
            Resize(panel.Width, panel.Height);
            InitFBOs();
        }
        public Viewport(float width, float height)
        {
            _index = 0;
            SetFullScreen();
            
            Resize(width, height);
            InitFBOs();
        }
        private void _postProcessGBuffer_SettingUniforms(int programBindingId)
        {
            _worldCamera?.PostProcess.SetUniforms(programBindingId);
        }

        public void SetInternalResolution(float width, float height)
        {
            _internalResolution.Width = width;
            _internalResolution.Height = height;

            int w = _internalResolution.IntWidth;
            int h = _internalResolution.IntHeight;

            //Engine.PrintLine("Internal resolution changed: {0}x{1}", w, h);

            GBufferFBO?.ResizeTextures(w, h);
            PingPongBloomBlurFBO?.ResizeTextures(w, h);
            SSAOBlurFBO?.ResizeTextures(w, h);
            ForwardPassFBO?.ResizeTextures(w, h);
            PostProcessFBO?.ResizeTextures(w, h);

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
        public void Render(Scene scene, Camera camera, Frustum frustum, MaterialFrameBuffer target)
        {
            if (scene == null || scene.Count == 0)
                return;

            CurrentlyRenderingViewports.Push(this);
            OnRender(scene, camera, frustum, target);
            CurrentlyRenderingViewports.Pop();
        }
        protected virtual void OnRender(Scene scene, Camera camera, Frustum frustum, MaterialFrameBuffer target)
        {
            scene.CollectVisibleRenderables(frustum, false);
            scene.Render(camera, this, target);
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
            float depth = Engine.Renderer.GetDepth(viewportPoint.X, viewportPoint.Y);
            return depth;
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

                //float depth = GetDepth(viewportPoint);

                //Engine.PrintLine(depth.ToString());
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
            Vec3[] _noise, _kernel;
            public const int DefaultSamples = 64;
            const int DefaultNoiseWidth = 4, DefaultNoiseHeight = 4;
            const float DefaultMinSampleDist = 0.1f, DefaultMaxSampleDist = 1.0f;
            
            public Vec3[] Noise => _noise;
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
                _noise = new Vec3[noiseWidth * noiseHeight];

                float scale;
                Vec3 sample, noise;

                for (int i = 0; i < _kernel.Length; ++i)
                {
                    sample = new Vec3(
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble());
                    sample.NormalizeFast();
                    scale = i / (float)samples;
                    scale = Interp.Lerp(minSampleDist, maxSampleDist, scale * scale);
                    _kernel[i] = sample * scale;
                }

                for (int i = 0; i < _noise.Length; ++i)
                {
                    noise = new Vec3(
                        (float)r.NextDouble(),
                        (float)r.NextDouble(),
                        0.0f);

                    noise.Normalize();
                    _noise[i] = noise;
                }
            }
        }
        #endregion

        #region FBOs
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
        internal unsafe void InitFBOs()
        {
            int width = InternalResolution.IntWidth;
            int height = InternalResolution.IntHeight;

            _ssaoInfo.Generate();

            TexRef2D depthTexture = TexRef2D.CreateFrameBufferTexture("Depth", width, height,
                EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float,
                EFramebufferAttachment.DepthAttachment);

            InitPostFBO(width, height, depthTexture);

            //If forward, we can render directly to the post process FBO.
            //If deferred, we have to render to a quad first, then render that to post process
            if (Engine.Settings.ShadingStyle3D == ShadingStyle.Deferred)
                InitGBuffer(width, height, depthTexture);
        }
        private void InitForwardFBO(int width, int height, TexRef2D depthTexture)
        {
            TexRef2D[] forwardRefs = new TexRef2D[]
            {
                TexRef2D.CreateFrameBufferTexture("OutputColor", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment0),
                depthTexture
            };
            TMaterial forwardMat = new TMaterial(
                "ForwardMat",
                forwardRefs,
                Engine.LoadEngineShader("PostProcess.fs", ShaderMode.Fragment));

            //postProcessMat.RenderParams.DepthTest.Enabled = true;
            //postProcessMat.RenderParams.DepthTest.UpdateDepth = false;
            //postProcessMat.RenderParams.DepthTest.Function = EComparison.Always;

            //PostProcessFBO = new QuadFrameBuffer(postProcessMat);
            //PostProcessFBO.SettingUniforms += _postProcessGBuffer_SettingUniforms;
        }
        private void InitPostFBO(int width, int height, TexRef2D depthTexture)
        {
            TexRef2D[] postProcessRefs = new TexRef2D[]
            {
                TexRef2D.CreateFrameBufferTexture("OutputColor", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment0),
                depthTexture
            };
            TMaterial postProcessMat = new TMaterial(
                "PostProcessMat",
                postProcessRefs,
                Engine.LoadEngineShader("PostProcess.fs", ShaderMode.Fragment));

            postProcessMat.RenderParams.DepthTest.Enabled = true;
            postProcessMat.RenderParams.DepthTest.UpdateDepth = false;
            postProcessMat.RenderParams.DepthTest.Function = EComparison.Always;

            PostProcessFBO = new QuadFrameBuffer(postProcessMat);
            PostProcessFBO.SettingUniforms += _postProcessGBuffer_SettingUniforms;
        }
        private unsafe void InitGBuffer(int width, int height, TexRef2D depthTexture)
        {
            #region SSAO Noise
            TexRef2D ssaoNoise = new TexRef2D("SSAONoise",
                _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight,
                EPixelInternalFormat.Rgb16, EPixelFormat.Bgr, EPixelType.UnsignedShort,
                PixelFormat.Format64bppArgb)
            {
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.Repeat,
                VWrap = ETexWrapMode.Repeat,
                ResizingDisabled = true,
            };
            Bitmap bmp = ssaoNoise.Mipmaps[0].File.Bitmaps[0];
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);
            ushort* values = (ushort*)data.Scan0;
            Vec3[] noise = _ssaoInfo.Noise;
            foreach (Vec3 v in noise)
            {
                *values++ = (ushort)(v.X * ushort.MaxValue);
                *values++ = (ushort)(v.Y * ushort.MaxValue);
                *values++ = (ushort)(v.Z * ushort.MaxValue);
                //*values++ = 0;
            }
            bmp.UnlockBits(data);
            #endregion

            TexRef2D[] deferredRefs = new TexRef2D[]
            {
                TexRef2D.CreateFrameBufferTexture("AlbedoOpacity", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment0),
                TexRef2D.CreateFrameBufferTexture("Normal", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment1),
                TexRef2D.CreateFrameBufferTexture("RoughnessMetallicSpecularUnused", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgba, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment2),
                //CreateFrameBufferTexture("Velocity", width, height,
                //    EPixelInternalFormat.Rg16f, EPixelFormat.Rg, EPixelType.HalfFloat,
                //    EFramebufferAttachment.ColorAttachment3),
                ssaoNoise,
                depthTexture,
            };
            TMaterial deferredMat = new TMaterial(
                "DeferredLightingMaterial", deferredRefs, 
                Engine.LoadEngineShader("DeferredLighting.fs", ShaderMode.Fragment))
            {
                Requirements = TMaterial.UniformRequirements.NeedsLightsAndCamera,
            };

            //Don't overwrite the depth texture generated by the deferred pass (and used by the forward pass)
            //when rendering the screen-space quad that shows the deferred pass
            deferredMat.RenderParamsRef.File.DepthTest.Enabled = true;
            deferredMat.RenderParamsRef.File.DepthTest.UpdateDepth = false;
            deferredMat.RenderParamsRef.File.DepthTest.Function = EComparison.Always;

            GBufferFBO = new QuadFrameBuffer(deferredMat);
            GBufferFBO.SettingUniforms += GBuffer_SetUniforms;
        }
        private void GBuffer_SetUniforms(int programBindingId)
        {
            if (_worldCamera == null)
                return;
            Engine.Renderer.Uniform(programBindingId, "SSAOSamples", _ssaoInfo.Kernel.Select(x => (IUniformable3Float)x).ToArray());
            _worldCamera.SetUniforms(programBindingId);
            _worldCamera.PostProcess.AmbientOcclusion.SetUniforms(programBindingId);
        }
        #endregion
    }
}
