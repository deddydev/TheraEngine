using TheraEngine.Input;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.HUD;
using TheraEngine.Worlds.Actors;
using System;
using TheraEngine.Rendering.Text;
using BulletSharp;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public delegate void DelOnRender(SceneProcessor scene);
    public class Viewport
    {
        public static Viewport CurrentlyRendering => _currentlyRendering;
        private static Viewport _currentlyRendering = null;

        public DelOnRender Render;

        private SSAOInfo _ssaoInfo = new SSAOInfo();
        private LocalPlayerController _owner;
        private HudManager _pawnHUD;
        private int _index;
        private BoundingRectangle _region;
        private Camera _worldCamera;
        private RenderPanel _owningPanel;
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
                    _worldCamera.Resize(_internalResolution.Width, _internalResolution.Height);
                    if (_worldCamera is PerspectiveCamera p)
                        p.Aspect = Width / Height;

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

        public BoundingRectangle InternalResolution => _internalResolution;

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
        
        internal void UpdateRender()
        {
            int width = InternalResolution.IntWidth;
            int height = InternalResolution.IntHeight;

            TextureReference depthTexture = new TextureReference("Depth", width, height,
                EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
            {
                MinFilter = ETexMinFilter.Nearest,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.Clamp,
                VWrap = ETexWrapMode.Clamp,
                FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
            };

            TextureReference[] postProcessRefs = new TextureReference[]
            {
                new TextureReference("OutputColor", width, height,
                    EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                },
                depthTexture,
            };
            TextureReference[] deferredRefs = new TextureReference[]
            {
                new TextureReference("AlbedoSpec", width, height,
                    EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                },
                new TextureReference("Normal", width, height,
                    EPixelInternalFormat.Rgb32f, EPixelFormat.Rgb, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment1,
                },
                depthTexture,
            };
            
            ShaderVar[] postProcessParameters = new ShaderVar[]
            {

            };
            ShaderVar[] deferredParameters = new ShaderVar[]
            {

            };

            //Debug.WriteLine(shader._source);
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
            _ssaoInfo.Generate(width, height);
        }

        private void _postProcessGBuffer_SettingUniforms(int programBindingId)
        {
            _worldCamera?.PostProcessSettings.SetUniforms(programBindingId);
        }

        private void _deferredGBuffer_SettingUniforms(int programBindingId)
        {
            _worldCamera?.SetUniforms(programBindingId);
        }

        public void SetInternalResolution(float width, float height)
        {
            _internalResolution.Width = width;
            _internalResolution.Height = height;

            _worldCamera?.Resize(_internalResolution.Width, _internalResolution.Height);
            if (_worldCamera is PerspectiveCamera p)
                p.Aspect = Width / Height;
            
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
                SetInternalResolution(parentWidth, parentHeight);
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
                scene.PreRender(Camera);
                Engine.Renderer.PushRenderArea(_internalResolution);

                _deferredGBuffer.Bind(EFramebufferTarget.Framebuffer);
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);
                
                scene.Render(RenderPassType3D.OpaqueDeferredLit);
                
                _deferredGBuffer.Unbind(EFramebufferTarget.Framebuffer);
                _postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                //Engine.Renderer.Clear(EBufferClear.Color);

                _deferredGBuffer.Render();
                Engine.Renderer.AllowDepthWrite(true);

                scene.Render(RenderPassType3D.OpaqueForward);
                scene.Render(RenderPassType3D.TransparentForward);
                scene.Render(RenderPassType3D.OnTopForward);
                
                _pawnHUD.Render();

                _postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);
                Engine.Renderer.PopRenderArea();

                //Render the last pass to the actual screen resolution
                _postProcessFrameBuffer.Render();

                scene.PostRender();
            }

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
                _postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                Engine.Renderer.PushRenderArea(_internalResolution);
                
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);
                
                scene.PreRender(Camera);

                scene.Render(RenderPassType3D.OpaqueForward);
                scene.Render(RenderPassType3D.TransparentForward);
                scene.Render(RenderPassType3D.OnTopForward);
                
                Engine.Renderer.PopRenderArea();

                _postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);

                //Render quad
                _postProcessFrameBuffer.Render();

                _pawnHUD.Render();

                scene.PostRender();
            }
            
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
        private class SSAOInfo
        {
            Vec3[] _noise, _kernel;
            public const int Samples = 64;
            const int NoiseWidth = 4, NoiseHeight = 4;
            const float MinSampleDist = 0.1f, MaxSampleDist = 1.0f;

            QuadFrameBuffer _ssaoGBuffer;

            public Vec3[] Noise => _noise;
            public Vec3[] Kernel => _kernel;

            public void Generate(
                int width, int height,
                int samples = Samples,
                int noiseWidth = NoiseWidth,
                int noiseHeight = NoiseHeight,
                float minSampleDist = MinSampleDist,
                float maxSampleDist = MaxSampleDist)
            {
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
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        (float)r.NextDouble() * 2.0f - 1.0f,
                        0.0f);
                    noise.Normalize();
                    _noise[i] = noise;
                }

                ShaderVar[] vars = new ShaderVar[]
                {

                };
                TextureReference[] refs = new TextureReference[]
                {
                    new TextureReference("OutputColor", width, height,
                        EPixelInternalFormat.Rgba8, EPixelFormat.Bgra, EPixelType.UnsignedByte)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Clamp,
                        VWrap = ETexWrapMode.Clamp,
                        FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                    },
                    new TextureReference("SSAONoise", noiseWidth, noiseHeight, EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.Float)
                    {
                        MinFilter = ETexMinFilter.Nearest,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.Repeat,
                        VWrap = ETexWrapMode.Repeat,
                    },
                };

                Material ssaoMat = new Material("SSAOMat", vars, refs, null);

                _ssaoGBuffer = new QuadFrameBuffer(ssaoMat);
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
uniform sampler2D Texture2; //Depth

" + Camera.ShaderSetup() + @"
" + ShaderHelpers.LightingSetupBasic() + @"
" + ShaderHelpers.Func_WorldPosFromDepth + @"

void main()
{
    vec2 uv = FragPos.xy;
    if (uv.x > 1.0 || uv.y > 1.0)
        discard;

    vec4 AlbedoSpec = texture(Texture0, uv);
    vec3 Normal = texture(Texture1, uv).rgb;
    float Depth = texture(Texture2, uv).r;
    vec3 FragPosWS = WorldPosFromDepth(Depth, uv);

    " + ShaderHelpers.LightingCalc("totalLight", "GlobalAmbient", "Normal", "FragPosWS", "AlbedoSpec.rgb", "AlbedoSpec.a") + @"

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

void main()
{
    vec2 uv = FragPos.xy;
    vec3 hdrSceneColor = texture(Texture0, uv).rgb;
    float Depth = texture(Texture1, uv).r;

    //Color grading
    hdrSceneColor *= ColorGrade.Tint;

    //Tone mapping
    vec3 ldrSceneColor = vec3(1.0) - exp(-hdrSceneColor * ColorGrade.Exposure);
    
    //Vignette
    //float alpha = clamp(pow(distance(uv, vec2(0.5)), Vignette.Intensity), 0.0, 1.0);
    //vec4 smoothed = smoothstep(vec4(1.0), Vignette.Color, Vignette.Color * vec4(alpha));
    //ldrSceneColor = mix(ldrSceneColor, smoothed.rgb, alpha * smoothed.a);

    //Gamma-correct
    vec3 gammaCorrected = pow(ldrSceneColor, vec3(1.0 / ColorGrade.Gamma));

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
