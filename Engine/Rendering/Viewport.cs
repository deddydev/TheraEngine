﻿using TheraEngine.Input;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds.Actors;
using System;
using TheraEngine.Rendering.Text;
using BulletSharp;
using TheraEngine.Rendering.Models.Materials;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using TheraEngine.Core.Shapes;
using System.Collections.Generic;

namespace TheraEngine.Rendering
{
    public delegate void DelOnRender(SceneProcessor scene);
    public class Viewport
    {
        public static Viewport CurrentlyRendering => _currentlyRendering;
        private static Viewport _currentlyRendering = null;

        public DelOnRender Render;

        private SSAOInfo _ssaoInfo = new SSAOInfo();
        private List<LocalPlayerController> _owners = new List<LocalPlayerController>();
        private HudManager _pawnHUD;
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private RenderPanel _owningPanel;
        private QuadFrameBuffer _skyBoxBuffer;
        private QuadFrameBuffer _deferredGBuffer;
        private QuadFrameBuffer _postProcessFrameBuffer;

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
            if ((Owners.Count == 0 || Camera == null) && Engine.Audio == null)
                return;

            Vec3 forward = _worldCamera.GetForwardVector();
            Vec3 up = _worldCamera.GetUpVector();
            Engine.Audio.UpdateListener(_worldCamera.WorldPoint, forward, up, Vec3.Zero, 0.5f);
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

        public const int GBufferTextureCount = 4;

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
            UpdateRender();
        }
        
        internal unsafe void UpdateRender()
        {
            int width = InternalResolution.IntWidth;
            int height = InternalResolution.IntHeight;

            _ssaoInfo.Generate();

            TextureReference depthTexture = new TextureReference("Depth", width, height,
                EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
            {
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.Clamp,
                VWrap = ETexWrapMode.Clamp,
                FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
            };
            TextureReference ssaoNoise = new TextureReference("SSAONoise",
                _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight,
                EPixelInternalFormat.Rgba16, EPixelFormat.Bgra, EPixelType.UnsignedShort,
                PixelFormat.Format64bppArgb)
            {
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.Repeat,
                VWrap = ETexWrapMode.Repeat,
                DoNotResize = true,
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
                *values++ = 0;
            }
            bmp.UnlockBits(data);
            TextureReference[] deferredRefs = new TextureReference[]
            {
                new TextureReference("AlbedoSpec", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                },
                new TextureReference("Normal", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment1,
                },
                ssaoNoise,
                depthTexture,
            };
            TextureReference[] postProcessRefs = new TextureReference[]
            {
                new TextureReference("OutputColor", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                },
                depthTexture,
            };

            ShaderVar[] postProcessParameters = new ShaderVar[]
            {

            };
            ShaderVar[] deferredParameters = new ShaderVar[]
            {

            };

            //Engine.DebugPrint(shader._source);
            Material postProcessMat =  new Material("GBufferPostProcessMaterial", postProcessParameters, postProcessRefs, GBufferShaderPostProcess())
            {
                Requirements = Material.UniformRequirements.None
            };
            Material deferredMat = new Material("GBufferDeferredMaterial", deferredParameters, deferredRefs, GBufferShaderDeferred())
            {
                Requirements = Material.UniformRequirements.NeedsLightsAndCamera,
            };

            //Don't overwrite the depth texture generated by the deferred pass (and used by the forward pass)
            //when rendering the screen-space quad that shows the deferred pass

            deferredMat.RenderParams.DepthTest.Enabled = true;
            deferredMat.RenderParams.DepthTest.UpdateDepth = false;
            deferredMat.RenderParams.DepthTest.Function = EComparison.Always;

            //postProcessMat.RenderParams.DepthTest.Enabled = true;
            //postProcessMat.RenderParams.DepthTest.UpdateDepth = false;
            //postProcessMat.RenderParams.DepthTest.Function = EComparison.Always;

            _postProcessFrameBuffer = new QuadFrameBuffer(postProcessMat);
            _postProcessFrameBuffer.SettingUniforms += _postProcessGBuffer_SettingUniforms;

            if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
            {
                _deferredGBuffer = new QuadFrameBuffer(deferredMat);
                _deferredGBuffer.SettingUniforms += _deferredGBuffer_SettingUniforms;

                Render = RenderDeferred;
            }
            else
            {
                Render = RenderForward;
            }
        }

        private void _postProcessGBuffer_SettingUniforms(int programBindingId)
        {
            _worldCamera?.PostProcessSettings.SetUniforms(programBindingId);
        }

        private void _deferredGBuffer_SettingUniforms(int programBindingId)
        {
            if (_worldCamera == null)
                return;
            Engine.Renderer.Uniform(programBindingId, "SSAOSamples", _ssaoInfo.Kernel.Select(x => (IUniformable3Float)x).ToArray());
            _worldCamera.SetUniforms(programBindingId);
            _worldCamera.PostProcessSettings.AmbientOcclusion.SetUniforms(programBindingId);
        }

        public void SetInternalResolution(float width, float height)
        {
            _internalResolution.Width = width;
            _internalResolution.Height = height;

            _worldCamera?.Resize(_internalResolution.Width, _internalResolution.Height);

            _pawnHUD.Resize(_internalResolution.Bounds);
            _deferredGBuffer?.ResizeTextures(_internalResolution.IntWidth, _internalResolution.IntHeight);
            _postProcessFrameBuffer?.ResizeTextures(_internalResolution.IntWidth, _internalResolution.IntHeight);
        }
        internal void Resize(float parentWidth, float parentHeight, bool setInternalResolution = true)
        {
            _region.X = _leftPercentage * parentWidth;
            _region.Y = _bottomPercentage * parentHeight;
            _region.Width = _rightPercentage * parentWidth - _region.X;
            _region.Height =  _topPercentage * parentHeight - _region.Y;
            
            if (setInternalResolution)
                //SetInternalResolution(1920, 1080);
                SetInternalResolution(parentWidth, parentHeight); 

            if (_worldCamera is PerspectiveCamera p)
                p.Aspect = Width / Height;

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
        public void RenderDeferred(SceneProcessor scene)
        {
            _currentlyRendering = this;

            //_skyBoxBuffer.Bind(EFramebufferTarget.Framebuffer);
            //{
            //    //Render skybox objects
            //    scene.Render(ERenderPassType3D.Skybox);
            //}
            //_skyBoxBuffer.Unbind(EFramebufferTarget.Framebuffer);
            //_skyBoxBuffer.Render();

            if (Camera != null)
            {
                //AbstractRenderer.PushCurrentCamera(Camera);
                //Engine.Scene?.RenderShadowMaps();
                //AbstractRenderer.PopCurrentCamera();

                scene.PreRender(Camera, false);

                //Enable internal resolution
                Engine.Renderer.PushRenderArea(_internalResolution);
                {
                    //Render to deferred framebuffer.
                    _deferredGBuffer.Bind(EFramebufferTarget.Framebuffer);
                    {
                        //Initial scene setup
                        Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                        Engine.Renderer.AllowDepthWrite(true);
                        
                        //Render deferred objects
                        scene.Render(ERenderPassType3D.OpaqueDeferredLit);
                    }
                    _deferredGBuffer.Unbind(EFramebufferTarget.Framebuffer);

                    //Now render to final post process framebuffer.
                    _postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                    {
                        //No need to clear anything, 
                        //color will be fully overwritten by the previous pass, 
                        //and we need depth from the previous pass

                        //Render the deferred pass result
                        _deferredGBuffer.Render();

                        Engine.Renderer.AllowDepthWrite(true);

                        //Render forward opaque objects first
                        scene.Render(ERenderPassType3D.OpaqueForward);
                        //Engine.World.PhysicsScene.DebugDrawWorld();
                        //Render forward transparent objects next
                        scene.Render(ERenderPassType3D.TransparentForward);
                        //Render forward on-top objects last
                        scene.Render(ERenderPassType3D.OnTopForward);

                        //Now render the hud over everything
                        _pawnHUD.Render();
                    }
                    _postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);
                }

                //Disable internal resolution
                Engine.Renderer.PopRenderArea();

                //Render the last pass to the actual screen resolution
                Engine.Renderer.PushRenderArea(Region);
                Engine.Renderer.CropRenderArea(Region);
                _postProcessFrameBuffer.Render();
                Engine.Renderer.PopRenderArea();

                scene.PostRender();
            }
            else
            {
                Engine.Renderer.PushRenderArea(_internalResolution);
                _postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);
                _pawnHUD.Render();
                _postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);
                Engine.Renderer.PopRenderArea();
                _postProcessFrameBuffer.Render();
            }

            _currentlyRendering = null;
        }
        public void RenderForward(SceneProcessor scene)
        {
            _currentlyRendering = this;

            if (Camera != null)
            {
                scene.PreRender(Camera, false);
                Engine.Renderer.PushRenderArea(_internalResolution);

                _postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                {
                    //Initial scene setup
                    Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                    Engine.Renderer.AllowDepthWrite(true);

                    //Render forward opaque objects first
                    scene.Render(ERenderPassType3D.OpaqueForward);
                    //Render forward transparent objects next
                    scene.Render(ERenderPassType3D.TransparentForward);

                    //Disable depth fail for objects on top
                    Engine.Renderer.DepthFunc(EComparison.Always);

                    //Render forward on-top objects last
                    scene.Render(ERenderPassType3D.OnTopForward);

                    _pawnHUD.Render();
                }
                _postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);

                Engine.Renderer.PopRenderArea();

                //Render quad
                Engine.Renderer.PushRenderArea(Region);
                Engine.Renderer.CropRenderArea(Region);
                _postProcessFrameBuffer.Render();
                Engine.Renderer.PopRenderArea();

                scene.PostRender();
            }
            
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

        /// <summary>
        /// Converts a viewport point relative to actual screen resolution
        /// to a point relative to the internal resolution.
        /// </summary>
        public Vec2 ToInternalResCoords(Vec2 viewportPoint) => viewportPoint * (InternalResolution.Bounds / _region.Bounds);
        /// <summary>
        /// Converts a viewport point relative to the internal resolution
        /// to a point relative to the actual screen resolution.
        /// </summary>
        public Vec2 FromInternalResCoords(Vec2 viewportPoint) => viewportPoint * (InternalResolution.Bounds / _region.Bounds);

        public unsafe float GetDepth(Vec2 viewportPoint)
        {
            throw new NotImplementedException();
            Vec2 absolutePoint = viewportPoint;//RelativeToAbsolute(viewportPoint);
            _deferredGBuffer.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.SetReadBuffer(EDrawBuffersAttachment.None);
            //var depthTex = _gBuffer.Textures[4];
            //depthTex.Bind();
            //if (viewportPoint.Y >= depthTex.Height || viewportPoint.Y < 0 || viewportPoint.X >= depthTex.Width || viewportPoint.X < 0)
            //    return 0;
            //BitmapData bmd = depthTex.LockBits(new Rectangle(0, 0, depthTex.Width, depthTex.Height), ImageLockMode.ReadWrite, depthTex.PixelFormat);
            //float depth = *(float*)((byte*)bmd.Scan0 + ((int)viewportPoint.Y * bmd.Stride + (int)viewportPoint.X * 4));
            //depthTex.UnlockBits(bmd);
            float depth = Engine.Renderer.GetDepth(absolutePoint.X, absolutePoint.Y);
            _deferredGBuffer.Unbind(EFramebufferTarget.Framebuffer);
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
            params CollisionObject[] ignored)
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
                ClosestRayResultCallback c = ignored != null && ignored.Length > 0 ?
                    Engine.RaycastClosestExcept(cursor, ignored) :
                    Engine.RaycastClosest(cursor);
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
                    scale = CustomMath.Lerp(minSampleDist, maxSampleDist, scale * scale);
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
        internal static Shader GBufferShaderDeferred()
        {
            string source = @"
#version 450
//GBUFFER FRAG SHADER

layout (location = 0) out vec4 OutColor;
in vec3 FragPos;

uniform sampler2D Texture0; //AlbedoSpec
uniform sampler2D Texture1; //Normal
uniform sampler2D Texture2; //SSAO Noise
uniform sampler2D Texture3; //Depth

uniform vec3 SSAOSamples[64];
uniform float SSAORadius = 0.75;
uniform float SSAOPower = 4.0;

" + Camera.ShaderDecl() + @"
" + ShaderHelpers.LightingDeclBasic() + @"
" + ShaderHelpers.Func_ViewPosFromDepth + @"

void main()
{
    vec2 uv = FragPos.xy;
    if (uv.x > 1.0 || uv.y > 1.0)
        discard;

    vec4 AlbedoSpec = texture(Texture0, uv);
    vec3 Normal = texture(Texture1, uv).rgb;
    float Depth = texture(Texture3, uv).r;

    vec3 FragPosVS = ViewPosFromDepth(Depth, uv);
    vec3 FragPosWS = (CameraToWorldSpaceMatrix * vec4(FragPosVS, 1.0)).xyz;

    ivec2 res = textureSize(Texture0, 0);
    vec2 noiseScale = vec2(res.x * 0.25f, res.y * 0.25f);

    vec3 randomVec = vec3(texture(Texture2, uv * noiseScale).rg * 2.0f - 1.0f, 0.0f);
    vec3 n = normalize(vec3(WorldToCameraSpaceMatrix * vec4(Normal, 0.0)));
    vec3 tangent = normalize(randomVec - n * dot(randomVec, n));
    vec3 bitangent = cross(n, tangent);
    mat3 TBN = mat3(tangent, bitangent, n); 

    int kernelSize = 64;
    float bias = 0.025;

    float occlusion = 0.0f;
    for (int i = 0; i < kernelSize; ++i)
    {
        vec3 noiseSample = TBN * SSAOSamples[i];
        noiseSample = FragPosVS + noiseSample * SSAORadius;

        vec4 offset = ProjMatrix * vec4(noiseSample, 1.0f);
        offset.xyz /= offset.w;
        offset.xyz = offset.xyz * 0.5f + 0.5f;

        float sampleDepth = ViewPosFromDepth(texture(Texture3, offset.xy).r, offset.xy).z;

        float rangeCheck = smoothstep(0.0, 1.0, SSAORadius / abs(FragPosVS.z - sampleDepth));
        occlusion += (sampleDepth >= noiseSample.z + bias ? 1.0 : 0.0) * rangeCheck;  
    } 

    occlusion = pow(1.0 - (occlusion / kernelSize), SSAOPower);

    " + ShaderHelpers.LightingCalcBasic("totalLight", "GlobalAmbient", "Normal", "FragPosWS", "AlbedoSpec.rgb", "AlbedoSpec.a", "occlusion") + @"

    OutColor = vec4(AlbedoSpec.rgb * totalLight, 1.0);
}";

            return new Shader(ShaderMode.Fragment, source);
        }
        internal static Shader GBufferShaderPostProcess()
        {
            string source = @"
#version 450
//POST PROCESSING FRAG SHADER

out vec4 OutColor;
in vec3 FragPos;

uniform sampler2D Texture0; //HDR Scene Color
uniform sampler2D Texture1; //Depth

" + PostProcessSettings.ShaderSetup() + @"
" + ShaderHelpers.Func_RGBtoHSV + @"
" + ShaderHelpers.Func_HSVtoRGB + @"

float rand(vec2 co)
{
    return fract(sin(dot(co.xy, vec2(12.9898,78.233))) * 43758.5453);
}

void main()
{
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    float Depth = texture(Texture1, uv).r;

    //Color grading
    hdrSceneColor *= ColorGrade.Tint;
    vec3 hsv = RGBtoHSV(hdrSceneColor);
    hsv.x *= ColorGrade.Hue;
    hsv.y *= ColorGrade.Saturation;
    hsv.z *= ColorGrade.Brightness;
    hdrSceneColor = HSVtoRGB(hsv);
    hdrSceneColor = (hdrSceneColor - 0.5) * ColorGrade.Contrast + 0.5;

    //Tone mapping
    vec3 ldrSceneColor = vec3(1.0) - exp(-hdrSceneColor * ColorGrade.Exposure);
    
    //Vignette
    uv *= 1.0 - uv.yx;
    float vig = clamp(pow(uv.x * uv.y * Vignette.Intensity, Vignette.Power), 0.0, 1.0);
    ldrSceneColor = mix(Vignette.Color, ldrSceneColor, vig);

    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(1.0 / ColorGrade.Gamma));

    //Fix subtle banding by applying fine noise
    gammaCorrected += mix(-0.5/255.0, 0.5/255.0, rand(uv));

    OutColor = vec4(gammaCorrected, 1.0);
}";
            return new Shader(ShaderMode.Fragment, source);
        }
        /// <summary>
        /// Takes the HDR scene color, tone maps to LDR and performs post processing.
        /// </summary>
        private static string PostProcessPart()
        {
            return @"";
        }
    }
}
