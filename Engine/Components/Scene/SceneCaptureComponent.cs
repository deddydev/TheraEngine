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
        protected PerspectiveCamera[] _cameras;
        protected FrameBuffer _renderFBO;
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

            _cameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f,  90.0f, 0.0f, RotationOrder.YPR), //+X right
                new Rotator(0.0f, -90.0f, 0.0f, RotationOrder.YPR), //-X left
                new Rotator(90.0f,  0.0f, 0.0f, RotationOrder.YPR), //+Y up
                new Rotator(-90.0f, 0.0f, 0.0f, RotationOrder.YPR), //-Y down
                new Rotator(0.0f, 180.0f, 0.0f, RotationOrder.YPR), //+Z backward
                new Rotator(0.0f,   0.0f, 0.0f, RotationOrder.YPR), //-Z forward
            };

            PerspectiveCamera c;
            for (int i = 0; i < 6; ++i)
            {
                c = new PerspectiveCamera(Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f);
                c.LocalPoint.Raw = WorldPoint;
                //c.Resize(_viewport.InternalResolution.Width, _viewport.InternalResolution.Height);
                c.PostProcessRef.File.ColorGrading.AutoExposure = true;
                c.PostProcessRef.File.ColorGrading.Exposure = 1.0f;
                _cameras[i] = c;
            }

            _envTex = new TexRefCube("SceneCaptureCubeMap", _colorRes,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.NearestMipmapLinear,
                MagFilter = ETexMagFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
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
                };

            _tempDepth = new RenderBuffer();
            _tempDepth.SetStorage(ERenderBufferStorage.DepthComponent16, _colorRes, _colorRes);
            _renderFBO = new FrameBuffer();
        }
        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            if (_cameras != null)
                foreach (Camera c in _cameras)
                    c.LocalPoint.Raw = WorldPoint;
        }
        /// <summary>
        /// Renders the scene to the ResultTexture cubemap.
        /// </summary>
        public void Capture()
        {
            //_cubeTex = new TexRefCube("", 512, new CubeMipmap(Engine.LoadEngineTexture2D("skybox.png")));
            
            if (_renderFBO == null)
                SetCaptureResolution(512);

            Scene3D scene = OwningScene;
            scene.UpdateShadowMaps();
            scene.Lights.SwapBuffers();
            scene.RenderShadowMaps();

            for (int i = 0; i < 6; ++i)
            {
                Camera camera = _cameras[i];
                _viewport.Update(scene, camera, camera.Frustum);
                _viewport.SwapBuffers(scene);
                if (CaptureDepthCubeMap)
                {
                    _renderFBO.SetRenderTargets(
                        (_envTex, EFramebufferAttachment.ColorAttachment0, 0, i),
                        (_envDepthTex, EFramebufferAttachment.DepthAttachment, 0, i));
                }
                else
                {
                    _renderFBO.SetRenderTargets(
                        (_envTex, EFramebufferAttachment.ColorAttachment0, 0, i),
                        (_tempDepth, EFramebufferAttachment.DepthAttachment, 0, -1));

                }
                _viewport.Render(scene, camera, _renderFBO);
            }

            BaseRenderTexture tex = _envTex.RenderTextureGeneric;
            tex.Bind();
            tex.SetMipmapGenParams();
            tex.GenerateMipmaps();
        }
    }
}
