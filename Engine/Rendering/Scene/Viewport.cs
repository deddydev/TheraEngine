using Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
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
    public class Viewport : TObjectSlim
    {
        public const string SceneShaderPath = "Scene3D";
        public static Viewport CurrentlyRendering => Engine.CurrentlyRenderingViewport;

        public BoundingRectangle _region;
        public BoundingRectangle _internalResolution = new BoundingRectangle();
        public SSAOInfo _ssaoInfo = new SSAOInfo();

        public QuadFrameBuffer SSAOFBO;
        public QuadFrameBuffer SSAOBlurFBO;
        public FrameBuffer GBufferFBO;
        public QuadFrameBuffer BloomBlurFBO1;
        public QuadFrameBuffer BloomBlurFBO2;
        public QuadFrameBuffer BloomBlurFBO4;
        public QuadFrameBuffer BloomBlurFBO8;
        public QuadFrameBuffer BloomBlurFBO16;
        public QuadFrameBuffer LightCombineFBO;
        public QuadFrameBuffer ForwardPassFBO;
        public QuadFrameBuffer PostProcessFBO;
        public QuadFrameBuffer HUDFBO;
        public PrimitiveManager PointLightManager;
        public PrimitiveManager SpotLightManager;
        public PrimitiveManager DirLightManager;
        //public PrimitiveManager DecalManager;
        //public QuadFrameBuffer DirLightFBO;
        public ICamera RenderingCamera => RenderingCameras.Count > 0 ? RenderingCameras.Peek() : null;
        public Stack<ICamera> RenderingCameras { get; } = new Stack<ICamera>();

        public TexRef2D _brdfTex = null;
        public TexRef2D BrdfTex
        {
            get
            {
                if (_brdfTex is null)
                    PrecomputeBRDF();
                return _brdfTex;
            }
        }

        public float _leftPercentage = 0.0f;
        public float _rightPercentage = 1.0f;
        public float _bottomPercentage = 0.0f;
        public float _topPercentage = 1.0f;

        public IScene AttachedScene => AttachedCamera?.OwningComponent?.OwningScene;

        public IUserInterfacePawn _hud;
        public IUserInterfacePawn AttachedHUD
        {
            get => _hud;
            set
            {
                _hud = value;
                _hud?.Resize(Region.Extents);

                Engine.PrintLine("Set viewport HUD: " + (_hud?.GetTypeProxy()?.GetFriendlyName() ?? "null"));
            }
        }
        public ICamera _camera;
        public ICamera AttachedCamera
        {
            get => _camera;
            set
            {
                _camera?.Viewports?.Remove(this);

                _camera = value;

                //Engine.PrintLine("Updated viewport " + _index + " camera: " + (_worldCamera is null ? "null" : _worldCamera.GetType().GetFriendlyName()));

                if (_camera != null)
                {
                    _camera.Viewports.Add(this);
                    
                    //TODO: what if the same camera is used by multiple viewports?
                    //Need to use a separate projection matrix per viewport instead of passing the width and height to the camera itself
                    _camera.Resize(_internalResolution.Width, _internalResolution.Height);
                    if (_camera is PerspectiveCamera p)
                        p.Aspect = (float)Width / Height;
                }

                Engine.PrintLine("Set viewport camera: " + (_camera?.GetTypeProxy()?.GetFriendlyName() ?? "null"));
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

        public bool RegeneratingFBOs = false;
        public bool FBOsInitialized = false;

        public List<LocalPlayerController> Owners { get; } = new List<LocalPlayerController>();
        public BoundingRectangle InternalResolution => _internalResolution;
        public BaseRenderHandler RenderHandler { get; }

        public Viewport(BaseRenderHandler panel, int index)
        {
            if (index == 0)
            {
                Index = index;
                SetFullScreen();
            }
            else
                ViewportCountChanged(index, panel.Viewports.Count + 1, Engine.Game.TwoPlayerPref, Engine.Game.ThreePlayerPref);

            RenderHandler = panel;
            Index = index;
            _ssaoInfo.Generate();

            Resize(panel.Width, panel.Height);
        }
        public Viewport(int width, int height)
        {
            Index = 0;
            SetFullScreen();
            _ssaoInfo.Generate();

            Resize(width, height);
        }

        public BoundingRectangle BloomRect16;
        public BoundingRectangle BloomRect8;
        public BoundingRectangle BloomRect4;
        public BoundingRectangle BloomRect2;
        //public BoundingRectangle BloomRect1;

        public void SetInternalResolution(int width, int height)
        {
            _internalResolution.Width = width;
            _internalResolution.Height = height;

            BloomRect16.Width = (int)(width * 0.0625f);
            BloomRect16.Height = (int)(height * 0.0625f);
            BloomRect8.Width = (int)(width * 0.125f);
            BloomRect8.Height = (int)(height * 0.125f);
            BloomRect4.Width = (int)(width * 0.25f);
            BloomRect4.Height = (int)(height * 0.25f);
            BloomRect2.Width = (int)(width * 0.5f);
            BloomRect2.Height = (int)(height * 0.5f);
            //BloomRect1.Width = width;
            //BloomRect1.Height = height;

            //ClearFBOs();
            _camera?.Resize(width, height);

            FBOsInitialized = false;
        }

        public void PushRenderingCamera(ICamera camera) => RenderingCameras.Push(camera);
        public void PopRenderingCamera() => RenderingCameras.Pop();

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
            HUDFBO?.Destroy();
            HUDFBO = null;
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
            BloomBlurFBO1?.Generate();
            BloomBlurFBO2?.Generate();
            BloomBlurFBO4?.Generate();
            BloomBlurFBO8?.Generate();
            BloomBlurFBO16?.Generate();
            ForwardPassFBO?.Generate();
            //DirLightFBO?.Generate();
            GBufferFBO?.Generate();
            HUDFBO?.Generate();
            LightCombineFBO?.Generate();
            PostProcessFBO?.Generate();
            SSAOBlurFBO?.Generate();
            SSAOFBO?.Generate();
            TimeSpan span = DateTime.Now - start;

            Engine.PrintLine($"FBO regeneration took {span.Seconds} seconds.");
        }

        public void Resize(
            int parentWidth,
            int parentHeight,
            bool setInternalResolution = true,
            int internalResolutionWidth = -1,
            int internalResolutionHeight = -1)
        {
            float w = parentWidth.ClampMin(1);
            float h = parentHeight.ClampMin(1);
            
            _region.X = (int)(_leftPercentage * w);
            _region.Y = (int)(_bottomPercentage * h);
            _region.Width = (int)(_rightPercentage * w - _region.X);
            _region.Height = (int)(_topPercentage * h - _region.Y);
            
            if (setInternalResolution) SetInternalResolution(
                internalResolutionWidth <= 0 ? _region.Width : internalResolutionWidth,
                internalResolutionHeight <= 0 ? _region.Height : internalResolutionHeight);

            AttachedHUD?.Resize(_region.Extents);
            if (AttachedCamera is PerspectiveCamera p)
                p.Aspect = (float)_region.Width / _region.Height;
        }

        /// <summary>
        /// Associates the given local player controller with this viewport.
        /// </summary>
        public void RegisterController(LocalPlayerController controller)
        {
            if (!Owners.Contains(controller))
                Owners.Add(controller);

            //TODO: allow multiple viewports per controller?
            //Choose which viewport to use based on hovered cursor position?
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
            if (Owners.Count == 0)
                RenderHandler.Viewports.TryRemove(PlayerIndex, out _);
        }

        public void FullRender(FrameBuffer target) => FullRender(AttachedCamera, AttachedHUD, target);
        public void FullRender(ICamera camera, IUserInterfacePawn hud, FrameBuffer target)
        {
            PreRenderUpdate(camera, hud);
            PreRenderSwap(camera, hud);
            Render(camera, hud, target);
        }

        public void Render() => Render(null);
        public void Render(FrameBuffer target) => Render(AttachedCamera, AttachedHUD, target);
        public void Render(ICamera camera, IUserInterfacePawn hud, FrameBuffer target)
        {
            var scene = camera?.OwningComponent?.OwningScene;

            if (scene is null || RegeneratingFBOs || Engine.CurrentlyRenderingViewport == this)
                return;

            scene?.PreRender(this, camera);
            hud?.PreRender();

            Engine.PushRenderingViewport(this);
            OnRender(scene, camera, hud, target);
            Engine.PopRenderingViewport();
        }
        private bool _captured = false;
        private readonly RenderPasses _renderPasses = new RenderPasses();
        protected virtual void OnRender(IScene scene, ICamera camera, IUserInterfacePawn hud, FrameBuffer target)
        {
            if (!FBOsInitialized)
                InitializeFBOs();

            scene.Render(_renderPasses, camera, this, target);

            if (scene is Scene3D s3d && !_captured)
            {
                s3d.IBLProbeActor?.InitAndCaptureAll(512);
                _captured = true;
            }

            //hud may sample scene colors, render it after scene
            AttachedHUD?.RenderInScreenSpace(this, HUDFBO);
        }
        public void PreRenderSwap() => PreRenderSwap(AttachedCamera, AttachedHUD);
        public void PreRenderSwap(ICamera camera, IUserInterfacePawn hud)
        {
            camera?.OwningComponent?.OwningScene?.PreRenderSwap();
            hud?.SwapInScreenSpace();

            _renderPasses.SwapBuffers();
        }
        public void PreRenderUpdate() => PreRenderUpdate(AttachedCamera, AttachedHUD);
        public void PreRenderUpdate(ICamera camera, IUserInterfacePawn hud)
        {
            camera?.OwningComponent?.OwningScene?.PreRenderUpdate(_renderPasses, camera.Frustum, camera);
            hud?.UpdateLayout();
        }

        /// <summary>
        /// Returns the cursor position relative to the the viewport.
        /// </summary>
        public static Vec2 CursorPosition(Viewport v)
        {
            Point absolute = Cursor.Position;

            if (v is null)
                return new Vec2(absolute.X, absolute.Y);

            RenderContext ctx = v.RenderHandler.Context;
            absolute = ctx.PointToClient(absolute);
            Vec2 result = new Vec2(absolute.X, absolute.Y);
            result = v.AbsoluteToRelative(result);
            return result;
        }

        /// <summary>
        /// Returns the cursor position relative to the the viewport.
        /// </summary>
        public static Vec2 CursorPosition(Viewport v, out bool isOutOfBounds)
        {
            Point absolute = Cursor.Position;

            if (v is null)
            {
                isOutOfBounds = true;
                return new Vec2(absolute.X, absolute.Y);
            }

            RenderContext ctx = v.RenderHandler.Context;
            absolute = ctx.PointToClient(absolute);
            Vec2 result = new Vec2(absolute.X, absolute.Y);
            result = v.AbsoluteToRelative(result);

            isOutOfBounds = 
                result.X < 0 ||
                result.Y < 0 ||
                result.X > v.Region.Width ||
                result.Y > v.Region.Height;

            return result;
        }

        /// <summary>
        /// Returns the cursor position relative to the the viewport.
        /// </summary>
        public Vec2 CursorPosition() 
            => CursorPosition(this);
        
        #region Coordinate conversion
        public Vec3 ScreenToWorld(Vec2 viewportPoint, float depth)
            => _camera?.ScreenToWorld(ToInternalResCoords(viewportPoint), depth) ?? new Vec3(viewportPoint, depth);
        public Vec3 ScreenToWorld(Vec3 viewportPoint)
            => _camera?.ScreenToWorld(ToInternalResCoords(viewportPoint.Xy), viewportPoint.Z) ?? viewportPoint;
        public Vec3 WorldToScreen(Vec3 worldPoint)
        {
            if (_camera is null)
                return Vec3.Zero;
            Vec3 screenPoint = _camera.WorldToScreen(worldPoint);
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

        //internal void RenderDecal(DecalComponent c)
        //{
        //    _decalComp = c;
        //    DecalManager.Render(c.DecalRenderMatrix);
        //}
        //TODO: render using instances
        internal void RenderDirLight(DirectionalLightComponent c)
        {
            _lightComp = c;
            DirLightManager.Render(c.LightMatrix, Matrix3.Identity);
        }
        internal void RenderPointLight(PointLightComponent c)
        {
            _lightComp = c;
            PointLightManager.Render(c.LightMatrix, Matrix3.Identity);
        }
        internal void RenderSpotLight(SpotLightComponent c)
        {
            _lightComp = c;
            SpotLightManager.Render(c.LightMatrix, Matrix3.Identity);
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
            => _camera.GetWorldRay(ToInternalResCoords(viewportPoint));
        /// <summary>
        /// Returns a segment projected from the given screen location.
        /// Endpoints are located on the NearZ and FarZ planes.
        /// </summary>
        public Segment GetWorldSegment(Vec2 viewportPoint)
            => _camera.GetWorldSegment(ToInternalResCoords(viewportPoint));
        private RayTraceClosest _closestPick = new RayTraceClosest(Vec3.Zero, Vec3.Zero, 0, 0xFFFF);
        public ISceneComponent PickScene(
            Vec2 viewportPoint,
            bool testHud,
            bool interactableHudOnly,
            bool testWorld,
            out Vec3 hitNormal,
            out Vec3 hitPoint,
            out float distance,
            params TCollisionObject[] ignored)
        {
            if (testHud)
            {
                IUIComponent hudComp = AttachedHUD?.FindDeepestComponent(viewportPoint);
                bool hasHit = hudComp?.IsVisible ?? false;
                bool hitValidated = !interactableHudOnly || hudComp is UIInteractableComponent;
                if (hasHit && hitValidated)
                {
                    hitNormal = Vec3.Backward;
                    hitPoint = new Vec3(viewportPoint, 0.0f);
                    distance = 0.0f;
                    return hudComp;
                }
                //Continue on to test the world is nothing of importance in the HUD was hit
            }
            if (testWorld)
            {
                Segment cursor = GetWorldSegment(viewportPoint);

                _closestPick.StartPointWorld = cursor.StartPoint;
                _closestPick.EndPointWorld = cursor.EndPoint;
                _closestPick.Ignored = ignored;
                
                if (_closestPick.Trace(_camera?.OwningComponent?.OwningWorld))
                {
                    hitNormal = _closestPick.HitNormalWorld;
                    hitPoint = _closestPick.HitPointWorld;
                    distance = hitPoint.DistanceToFast(cursor.StartPoint);
                    TCollisionObject coll = _closestPick.CollisionObject;
                    return coll.Owner as ISceneComponent;
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
        public enum ETwoPlayerPreference
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
        public enum EThreePlayerPreference
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
        public void ViewportCountChanged(int newIndex, int total, ETwoPlayerPreference twoPlayerPref, EThreePlayerPreference threePlayerPref)
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
                            if (twoPlayerPref == ETwoPlayerPreference.SplitHorizontally)
                                SetTop();
                            else
                                SetLeft();
                            break;
                        case 1:
                            if (twoPlayerPref == ETwoPlayerPreference.SplitHorizontally)
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
                                case EThreePlayerPreference.BlankBottomRight:
                                    SetTopLeft();
                                    break;
                                case EThreePlayerPreference.PreferFirstPlayer:
                                    SetTop();
                                    break;
                                case EThreePlayerPreference.PreferSecondPlayer:
                                    SetBottomLeft();
                                    break;
                                case EThreePlayerPreference.PreferThirdPlayer:
                                    SetTopLeft();
                                    break;
                            }
                            break;
                        case 1:
                            switch (threePlayerPref)
                            {
                                case EThreePlayerPreference.BlankBottomRight:
                                    SetTopRight();
                                    break;
                                case EThreePlayerPreference.PreferFirstPlayer:
                                    SetBottomLeft();
                                    break;
                                case EThreePlayerPreference.PreferSecondPlayer:
                                    SetTop();
                                    break;
                                case EThreePlayerPreference.PreferThirdPlayer:
                                    SetTopRight();
                                    break;
                            }
                            break;
                        case 2:
                            switch (threePlayerPref)
                            {
                                case EThreePlayerPreference.BlankBottomRight:
                                    SetBottomLeft();
                                    break;
                                case EThreePlayerPreference.PreferFirstPlayer:
                                    SetBottomRight();
                                    break;
                                case EThreePlayerPreference.PreferSecondPlayer:
                                    SetBottomRight();
                                    break;
                                case EThreePlayerPreference.PreferThirdPlayer:
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
        public class SSAOInfo : TObjectSlim
        {
            public const int DefaultSamples = 64;
            const int DefaultNoiseWidth = 4, DefaultNoiseHeight = 4;
            const float DefaultMinSampleDist = 0.1f, DefaultMaxSampleDist = 1.0f;

            public Vec2[] Noise { get; private set; }
            public Vec3[] Kernel { get; private set; }
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

                Kernel = new Vec3[samples];
                Noise = new Vec2[noiseWidth * noiseHeight];

                float scale;
                Vec3 sample;
                Vec2 noise;

                for (int i = 0; i < samples; ++i)
                {
                    sample = new Vec3(
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() + 0.1f).Normalized();
                    scale = i / (float)samples;
                    sample *= Interp.Lerp(minSampleDist, maxSampleDist, scale * scale);
                    Kernel[i] = sample;
                }

                for (int i = 0; i < Noise.Length; ++i)
                {
                    noise = new Vec2((float)r.NextDouble(), (float)r.NextDouble());
                    noise.Normalize();
                    Noise[i] = noise;
                }
            }
        }
        #endregion

        #region FBOs

        public TexRef2D DepthStencilTexture { get; private set; }
        public TexRef2D HDRSceneTexture { get; private set; }
        public TexRef2D BloomBlurTexture { get; private set; }
        public TexRef2D AlbedoOpacityTexture { get; private set; }
        public TexRef2D NormalTexture { get; private set; }
        public TexRef2D RMSITexture { get; private set; }
        public TexRef2D SSAONoise { get; private set; }
        public TexRef2D SSAOTexture { get; private set; }
        public TexRef2D LightingTexture { get; private set; }
        public TexRefView2D DepthViewTexture { get; private set; }
        public TexRefView2D StencilViewTexture { get; private set; }
        public ELocalPlayerIndex PlayerIndex { get; internal set; }

        public enum EDepthStencilUse
        {
            None,
            Depth24,
            Depth32,
            Depth32f,
            Stencil8,
            Depth24Stencil8,
            Depth32Stencil8,
        }

        protected void PrecomputeBRDF(int width = 512, int height = 512)
        {
            RenderingParameters renderParams = new RenderingParameters();
            renderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            renderParams.DepthTest.Function = EComparison.Always;
            renderParams.DepthTest.UpdateDepth = false;

            _brdfTex = TexRef2D.CreateFrameBufferTexture("BRDF_LUT", width, height, EPixelInternalFormat.Rg16f, EPixelFormat.Rg, EPixelType.HalfFloat);
            _brdfTex.Resizable = true;
            _brdfTex.UWrap = ETexWrapMode.ClampToEdge;
            _brdfTex.VWrap = ETexWrapMode.ClampToEdge;
            _brdfTex.MinFilter = ETexMinFilter.Linear;
            _brdfTex.MagFilter = ETexMagFilter.Linear;
            _brdfTex.SamplerName = "BRDF";
            TexRef2D[] texRefs = new TexRef2D[] { _brdfTex };

            GLSLScript shader = Engine.Files.Shader(Path.Combine("Scene3D", "BRDF.fs"), EGLSLType.Fragment);
            TMaterial mat = new TMaterial("BRDFMat", renderParams, texRefs, shader);

            //ndc space quad, so we don't have to load any camera matrices
            VertexTriangle[] tris = VertexQuad.MakeQuad(
                    new Vec3(-1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, 1.0f, -0.5f),
                    new Vec3(-1.0f, 1.0f, -0.5f),
                    false, false).ToTriangles();

            using (MaterialFrameBuffer fbo = new MaterialFrameBuffer(mat))
            {
                fbo.SetRenderTargets((_brdfTex, EFramebufferAttachment.ColorAttachment0, 0, -1));

                using (PrimitiveData data = PrimitiveData.FromTriangles(VertexShaderDesc.PosTex(), tris))
                using (PrimitiveManager quad = new PrimitiveManager(data, mat))
                {
                    BoundingRectangle region = new BoundingRectangle(IVec2.Zero, new IVec2(width, height));

                    //Now render the texture to the FBO using the quad
                    fbo.Bind(EFramebufferTarget.DrawFramebuffer);
                    Engine.Renderer.PushRenderArea(region);
                    {
                        Engine.Renderer.Clear(EFBOTextureType.Color);
                        quad.Render();
                    }
                    Engine.Renderer.PopRenderArea();
                    fbo.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
            }
        }
        
        /// <summary>
        /// This method is called to generate all framebuffers necessary to render the final image for the viewport.
        /// </summary>
        internal protected virtual unsafe void InitializeFBOs()
        {
            RegeneratingFBOs = true;

            ClearFBOs();

            int width = InternalResolution.Width;
            int height = InternalResolution.Height;

            RenderingParameters renderParams = new RenderingParameters();
            renderParams.DepthTest.Enabled = ERenderParamUsage.Unchanged;
            renderParams.DepthTest.UpdateDepth = false;
            renderParams.DepthTest.Function = EComparison.Always;
            
            DepthStencilTexture = TexRef2D.CreateFrameBufferTexture("DepthStencil", width, height,
                EPixelInternalFormat.Depth24Stencil8, EPixelFormat.DepthStencil, EPixelType.UnsignedInt248,
                EFramebufferAttachment.DepthStencilAttachment);
            DepthStencilTexture.MinFilter = ETexMinFilter.Nearest;
            DepthStencilTexture.MagFilter = ETexMagFilter.Nearest;
            DepthStencilTexture.Resizable = false;

            DepthViewTexture = new TexRefView2D(DepthStencilTexture, 0, 1, 0, 1,
                EPixelType.UnsignedInt248, EPixelFormat.DepthStencil, EPixelInternalFormat.Depth24Stencil8)
            {
                Resizable = false,
                DepthStencilFormat = EDepthStencilFmt.Depth,
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
            };

            StencilViewTexture = new TexRefView2D(DepthStencilTexture, 0, 1, 0, 1,
                EPixelType.UnsignedInt248, EPixelFormat.DepthStencil, EPixelInternalFormat.Depth24Stencil8)
            {
                Resizable = false,
                DepthStencilFormat = EDepthStencilFmt.Stencil,
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
            };

            #region Deferred

            //If forward, we can render directly to the post process FBO.
            //If deferred, we have to render to a quad first, then render that to post process
            //if (Engine.Settings.EnableDeferredPass)
            {
                #region SSAO Noise
                SSAONoise = new TexRef2D("SSAONoise",
                    _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight,
                    EPixelInternalFormat.Rg32f, EPixelFormat.Rg, EPixelType.Float,
                    PixelFormat.Format64bppArgb)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Repeat,
                    VWrap = ETexWrapMode.Repeat,
                    Resizable = false,
                };
                Bitmap bmp = SSAONoise.Mipmaps[0].File.Bitmaps[0];
                BitmapData data = bmp.LockBits(new Rectangle(0, 0, _ssaoInfo.NoiseWidth, _ssaoInfo.NoiseHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);
                Vec2* values = (Vec2*)data.Scan0;
                Vec2[] noise = _ssaoInfo.Noise;
                foreach (Vec2 v in noise)
                    *values++ = v;
                bmp.UnlockBits(data);
                #endregion

                AlbedoOpacityTexture = TexRef2D.CreateFrameBufferTexture("AlbedoOpacity", width, height,
                    EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat);
                NormalTexture = TexRef2D.CreateFrameBufferTexture("Normal", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat);
                RMSITexture = TexRef2D.CreateFrameBufferTexture("RoughnessMetallicSpecularUnused", width, height,
                    EPixelInternalFormat.Rgba8, EPixelFormat.Rgba, EPixelType.UnsignedByte);
                SSAOTexture = TexRef2D.CreateFrameBufferTexture("OutputIntensity", width, height,
                    EPixelInternalFormat.R16f, EPixelFormat.Red, EPixelType.HalfFloat,
                    EFramebufferAttachment.ColorAttachment0);
                SSAOTexture.MinFilter = ETexMinFilter.Nearest;
                SSAOTexture.MagFilter = ETexMagFilter.Nearest;
                
                GLSLScript ssaoShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "SSAOGen.fs"), EGLSLType.Fragment);
                GLSLScript ssaoBlurShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "SSAOBlur.fs"), EGLSLType.Fragment);
                
                TexRef2D[] ssaoRefs = new TexRef2D[]
                {
                    NormalTexture,
                    SSAONoise,
                    DepthViewTexture,
                };
                TexRef2D[] ssaoBlurRefs = new TexRef2D[]
                {
                    SSAOTexture
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
                    (AlbedoOpacityTexture, EFramebufferAttachment.ColorAttachment0, 0, -1),
                    (NormalTexture, EFramebufferAttachment.ColorAttachment1, 0, -1),
                    (RMSITexture, EFramebufferAttachment.ColorAttachment2, 0, -1),
                    (DepthStencilTexture, EFramebufferAttachment.DepthStencilAttachment, 0, -1));

                SSAOBlurFBO = new QuadFrameBuffer(ssaoBlurMat);
                GBufferFBO = new FrameBuffer();
                GBufferFBO.SetRenderTargets((SSAOTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));

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
                RenderingParameters additiveRenderParams = new RenderingParameters
                {
                    //Render only the backside so that the light still shows if the camera is inside of the volume
                    //and the light does not add itself twice for the front and back faces.
                    CullMode = ECulling.Front,
                    Requirements = EUniformRequirements.Camera,
                    BlendMode = additiveBlend,
                };
                additiveRenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
                
                LightingTexture = TexRef2D.CreateFrameBufferTexture("Diffuse", width, height,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat);
                GLSLScript lightCombineShader = Engine.Files.Shader(
                    Path.Combine(SceneShaderPath, "DeferredLightCombine.fs"), EGLSLType.Fragment);
                BaseTexRef[] combineRefs = new BaseTexRef[]
                {
                    AlbedoOpacityTexture,
                    NormalTexture,
                    RMSITexture,
                    SSAOTexture,
                    DepthViewTexture,
                    LightingTexture,
                    BrdfTex,
                    //irradiance
                    //prefilter
                };
                TMaterial lightCombineMat = new TMaterial("LightCombineMat", renderParams, combineRefs, lightCombineShader);
                LightCombineFBO = new QuadFrameBuffer(lightCombineMat);
                LightCombineFBO.SetRenderTargets((LightingTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));
                LightCombineFBO.SettingUniforms += LightCombineFBO_SettingUniforms;

                TexRef2D[] lightRefs = new TexRef2D[]
                {
                    AlbedoOpacityTexture,
                    NormalTexture,
                    RMSITexture,
                    DepthViewTexture,
                    //shadow map texture
                };

                GLSLScript pointLightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "DeferredLightingPoint.fs"), EGLSLType.Fragment);
                GLSLScript spotLightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "DeferredLightingSpot.fs"), EGLSLType.Fragment);
                GLSLScript dirLightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "DeferredLightingDir.fs"), EGLSLType.Fragment);

                TMaterial pointLightMat = new TMaterial("PointLightMat", additiveRenderParams, lightRefs, pointLightShader);
                TMaterial spotLightMat = new TMaterial("SpotLightMat", additiveRenderParams, lightRefs, spotLightShader);
                TMaterial dirLightMat = new TMaterial("DirLightMat", additiveRenderParams, lightRefs, dirLightShader);

                PrimitiveData pointLightMesh = Sphere.SolidMesh(Vec3.Zero, 1.0f, 20u);
                PrimitiveData spotLightMesh = Cone.SolidMesh(Vec3.Zero, Vec3.UnitZ, 1.0f, 1.0f, 32, true);
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

            BloomBlurTexture = TexRef2D.CreateFrameBufferTexture("OutputColor", width, height,
                EPixelInternalFormat.Rgb8, EPixelFormat.Rgb, EPixelType.UnsignedByte);
            BloomBlurTexture.MagFilter = ETexMagFilter.Linear;
            BloomBlurTexture.MinFilter = ETexMinFilter.LinearMipmapLinear;
            BloomBlurTexture.UWrap = ETexWrapMode.ClampToEdge;
            BloomBlurTexture.VWrap = ETexWrapMode.ClampToEdge;
            
            HDRSceneTexture = TexRef2D.CreateFrameBufferTexture("HDRSceneColor", width, height,
                EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat,
                EFramebufferAttachment.ColorAttachment0);
            //_hdrSceneTexture.Resizeable = false;
            HDRSceneTexture.MinFilter = ETexMinFilter.Nearest;
            HDRSceneTexture.MagFilter = ETexMagFilter.Nearest;
            HDRSceneTexture.UWrap = ETexWrapMode.ClampToEdge;
            HDRSceneTexture.VWrap = ETexWrapMode.ClampToEdge;
            HDRSceneTexture.SamplerName = "HDRSceneTex";

            GLSLScript brightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "BrightPass.fs"), EGLSLType.Fragment);
            GLSLScript bloomBlurShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "BloomBlur.fs"), EGLSLType.Fragment);
            GLSLScript postProcessShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "PostProcess.fs"), EGLSLType.Fragment);
            GLSLScript hudShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "HudFBO.fs"), EGLSLType.Fragment);

            TexRef2D hudTexture = TexRef2D.CreateFrameBufferTexture("Hud", width, height,
                EPixelInternalFormat.Rgba16f, EPixelFormat.Rgba, EPixelType.HalfFloat);
            hudTexture.MinFilter = ETexMinFilter.Nearest;
            hudTexture.MagFilter = ETexMagFilter.Nearest;
            hudTexture.UWrap = ETexWrapMode.ClampToEdge;
            hudTexture.VWrap = ETexWrapMode.ClampToEdge;
            hudTexture.SamplerName = "HUDTex";

            TexRef2D[] brightRefs = new TexRef2D[]
            {
                HDRSceneTexture
            };
            TexRef2D[] blurRefs = new TexRef2D[]
            {
                BloomBlurTexture,
            };
            TexRef2D[] hudRefs = new TexRef2D[]
            {
                hudTexture,
            };
            TexRef2D[] postProcessRefs = new TexRef2D[]
            {
                HDRSceneTexture,
                BloomBlurTexture,
                DepthViewTexture,
                StencilViewTexture,
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
                (HDRSceneTexture, EFramebufferAttachment.ColorAttachment0, 0, -1),
                (DepthStencilTexture, EFramebufferAttachment.DepthStencilAttachment, 0, -1));

            BloomBlurFBO1 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO1.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));
            BloomBlurFBO2 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO2.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 1, -1));
            BloomBlurFBO4 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO4.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 2, -1));
            BloomBlurFBO8 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO8.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 3, -1));
            BloomBlurFBO16 = new QuadFrameBuffer(bloomBlurMat);
            BloomBlurFBO16.SetRenderTargets((BloomBlurTexture, EFramebufferAttachment.ColorAttachment0, 4, -1));

            PostProcessFBO = new QuadFrameBuffer(postProcessMat);
            PostProcessFBO.SettingUniforms += _postProcess_SettingUniforms;

            HUDFBO = new QuadFrameBuffer(hudMat);
            HUDFBO.SetRenderTargets((hudTexture, EFramebufferAttachment.ColorAttachment0, 0, -1));

            #endregion

            RegeneratingFBOs = false;
            FBOsInitialized = true;
        }

        private LightComponent _lightComp;
        //private DecalComponent _decalComp;

        private void LightManager_SettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            if (RenderingCamera is null)
                return;
            //RenderingCamera.PostProcessRef.File.Shadows.SetUniforms(materialProgram);
            _lightComp.SetShadowUniforms(materialProgram);
            _lightComp.SetUniforms(materialProgram, null);
        }
        private void LightCombineFBO_SettingUniforms(RenderProgram program)
        {
            if (RenderingCamera is null)
                return;

            RenderingCamera.SetUniforms(program);

            var probeActor = RenderingCamera.OwningComponent?.OwningScene3D?.IBLProbeActor;
            if (probeActor is null || probeActor.RootComponent.ChildComponents.Count == 0)
                return;

            IBLProbeComponent probe = (IBLProbeComponent)probeActor.RootComponent.ChildComponents[0];
            int baseCount = LightCombineFBO.Material.Textures.Count;

            if (probe.IrradianceTex != null)
            {
                var tex = probe.IrradianceTex.GetTexture(true);
                program.Sampler("Irradiance", tex, baseCount);
            }

            ++baseCount;

            if (probe.PrefilterTex != null)
            {
                var tex = probe.PrefilterTex.GetTexture(true);
                program.Sampler("Prefilter", tex, baseCount);
            }
        }

        //private void DecalManager_SettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        //{
        //    materialProgram.Uniform("BoxWorldMatrix", _decalComp.WorldMatrix);
        //    materialProgram.Uniform("InvBoxWorldMatrix", _decalComp.InverseWorldMatrix);
        //    materialProgram.Uniform("BoxHalfScale", _decalComp.Box.HalfExtents.Raw);
        //    materialProgram.Sampler("Texture4", _decalComp.AlbedoOpacity.RenderTextureGeneric, 4);
        //    materialProgram.Sampler("Texture5", _decalComp.Normal.RenderTextureGeneric, 5);
        //    materialProgram.Sampler("Texture6", _decalComp.RMSI.RenderTextureGeneric, 6);
        //}

        private void BrightPassFBO_SettingUniforms(RenderProgram program)
            => RenderingCamera?.SetBloomUniforms(program);
        
        private void SSAO_SetUniforms(RenderProgram program)
        {
            if (RenderingCamera is null)
                return;
            program.Uniform("NoiseScale", InternalResolution.Extents / 4.0f);
            program.Uniform("Samples", _ssaoInfo.Kernel.Select(x => (IUniformable3Float)x).ToArray());
            RenderingCamera.SetUniforms(program);
            RenderingCamera.SetAmbientOcclusionUniforms(program);
        }

        private void _postProcess_SettingUniforms(RenderProgram program)
        {
            if (RenderingCamera is null)
                return;

            RenderingCamera.SetUniforms(program);
            RenderingCamera.SetPostProcessUniforms(program);
        }

        #endregion
    }
}
