using System;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Actors.Types
{
    public class SceneCaptureComponent : TranslationComponent
    {
        protected int _colorRes;
        protected int _depthRes;
        protected Viewport _viewport;
        protected TexRefCube _envTex;
        protected TexRefCube _envDepthTex;
        protected RenderBuffer _tempDepth;

        private bool _captureDepthCubeMap = true;
        public bool CaptureDepthCubeMap
        {
            get => _captureDepthCubeMap;
            set
            {
                if (_captureDepthCubeMap == value)
                    return;
                _captureDepthCubeMap = value;
                if (_captureDepthCubeMap)
                {
                    _envDepthTex = new TexRefCube("SceneCaptureDepthCubeMap", _depthRes,
                        EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
                    {
                        MinFilter = ETexMinFilter.NearestMipmapLinear,
                        MagFilter = ETexMagFilter.Nearest,
                        UWrap = ETexWrapMode.ClampToEdge,
                        VWrap = ETexWrapMode.ClampToEdge,
                        WWrap = ETexWrapMode.ClampToEdge,
                    };
                }
                else
                    _envDepthTex = null;
            }
        }
        
        public TexRefCube ResultTexture
        {
            get => _envTex;
            set => _envTex = value;
        }
        public TexRefCube ResultDepthTexture
        {
            get => _envDepthTex;
            set => _envDepthTex = value;
        }

        public PerspectiveCamera[] Cameras { get; set; }
        protected CubeFrameBuffer RenderFBO { get; set; }

        public SceneCaptureComponent() { }
        
        public void SetCaptureResolution(int colorResolution, bool captureDepth = false, int depthResolution = 1)
        {
            _colorRes = colorResolution;
            _depthRes = depthResolution;
            _captureDepthCubeMap = captureDepth;
            InitializeForCapture();
        }

        protected virtual void InitializeForCapture()
        {
            _viewport = new Viewport(_colorRes, _colorRes);

            Cameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(  0.0f,  90.0f,   0.0f), //+X
                new Rotator(  0.0f, -90.0f,   0.0f), //-X
                new Rotator(-90.0f,   0.0f, 180.0f), //+Y
                new Rotator( 90.0f,   0.0f, 180.0f), //-Y
                new Rotator(  0.0f, 180.0f,   0.0f), //+Z
                new Rotator(  0.0f,   0.0f,   0.0f), //-Z

                //new Rotator(  0.0f, -90.0f, 180.0f), //+X
                //new Rotator(  0.0f,  90.0f, 180.0f), //-X
                //new Rotator( 90.0f,   0.0f,   0.0f), //+Y
                //new Rotator(-90.0f,   0.0f,   0.0f), //-Y
                //new Rotator(  0.0f, 180.0f, 180.0f), //+Z
                //new Rotator(  0.0f,   0.0f, 180.0f), //-Z
            };

            PerspectiveCamera c;
            for (int i = 0; i < 6; ++i)
            {
                c = new PerspectiveCamera(Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f);
                c.LocalPoint.Raw = WorldPoint;
                c.Resize(_viewport.InternalResolution.Width, _viewport.InternalResolution.Height);
                c.PostProcessRef.File.ColorGrading.AutoExposure = false;
                c.PostProcessRef.File.ColorGrading.Exposure = 1.0f;
                Cameras[i] = c;
            }

            _envTex = new TexRefCube("SceneCaptureCubeMap", _colorRes,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.NearestMipmapLinear,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
                SamplerName = "SceneTex"
            };

            if (CaptureDepthCubeMap)
                _envDepthTex = new TexRefCube("SceneCaptureDepthCubeMap", _depthRes,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
                {
                    MinFilter = ETexMinFilter.NearestMipmapLinear,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    WWrap = ETexWrapMode.ClampToEdge,
                    SamplerName = "SceneDepthTex"
                };

            _tempDepth = new RenderBuffer();
            _tempDepth.SetStorage(ERenderBufferStorage.DepthComponent16, _colorRes, _colorRes);
            RenderFBO = new CubeFrameBuffer(null, 0.1f, 10000.0f, true);
        }
        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            if (Cameras != null)
                foreach (Camera c in Cameras)
                    c.LocalPoint.Raw = WorldPoint;
        }
        /// <summary>
        /// Renders the scene to the ResultTexture cubemap.
        /// </summary>
        public void Capture()
        {
            //_cubeTex = new TexRefCube("", 512, new CubeMipmap(Engine.LoadEngineTexture2D("skybox.png")));
            
            if (RenderFBO == null)
                SetCaptureResolution(512);

            Scene3D scene = OwningScene3D;
            if (scene == null)
                return;

            //scene.UpdateShadowMaps();
            //scene.Lights.SwapBuffers();
            //scene.RenderShadowMaps();

            for (int i = 0; i < 6; ++i)
            {
                Camera camera = Cameras[i];

                _viewport.Update(scene, camera, camera.Frustum);

                scene.PreRenderSwap();
                _viewport.SwapBuffers();

                if (CaptureDepthCubeMap)
                {
                    RenderFBO.SetRenderTargets(
                        (_envTex, EFramebufferAttachment.ColorAttachment0, 0, i),
                        (_envDepthTex, EFramebufferAttachment.DepthAttachment, 0, i));
                }
                else
                {
                    RenderFBO.SetRenderTargets(
                        (_envTex, EFramebufferAttachment.ColorAttachment0, 0, i),
                        (_tempDepth, EFramebufferAttachment.DepthAttachment, 0, -1));
                }

                _viewport.HUD?.ScreenSpaceUIScene?.PreRender(_viewport, _viewport.HUD.ScreenOverlayCamera);
                scene.PreRender(_viewport, camera);

                _viewport.Render(scene, camera, RenderFBO);
            }

            BaseRenderTexture tex = _envTex.RenderTextureGeneric;
            tex.Bind();
            tex.SetMipmapGenParams();
            tex.GenerateMipmaps();
        }
    }
}
