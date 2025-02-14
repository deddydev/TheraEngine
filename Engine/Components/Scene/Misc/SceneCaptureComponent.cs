﻿using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Components.Scene
{
    public class SceneCaptureComponent : TransformComponent
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

            _envTex = new TexRefCube("SceneCaptureCubeMap", _colorRes,
                EPixelInternalFormat.Rgb8, EPixelFormat.Rgb, EPixelType.UnsignedByte)
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
            _tempDepth.SetStorage(ERenderBufferStorage.DepthComponent32f, _colorRes, _colorRes);

            RenderFBO = new CubeFrameBuffer(null, 0.1f, 10000.0f, true);
            ChildSockets.Clear();

            //RenderFBO.SetTransform(WorldPoint);

            //float aspect = _viewport.InternalResolution.Width / _viewport.InternalResolution.Height;
            foreach (TransformableCamera cam in RenderFBO.Cameras)
            {
                ChildSockets.Add(new CameraComponent(cam));

                cam.PostProcessRef.File.ColorGrading.AutoExposure = false;
                cam.PostProcessRef.File.ColorGrading.Exposure = 1.0f;
            }
        }
        //protected override void OnWorldTransformChanged()
        //{
        //    base.OnWorldTransformChanged();
        //    //RenderFBO?.SetTransform(WorldPoint);
        //}
        /// <summary>
        /// Renders the scene to the ResultTexture cubemap.
        /// </summary>
        public void Capture()
        {
            //_cubeTex = new TexRefCube("", 512, new CubeMipmap(Engine.LoadEngineTexture2D("skybox.png")));
            
            if (RenderFBO is null)
                SetCaptureResolution(512);

            IScene3D scene = OwningScene3D;
            if (scene is null)
                return;

            scene.CollectShadowMaps();
            scene.Lights.SwapBuffers();
            scene.RenderShadowMaps();

            int depthLayer;
            IFrameBufferAttachement depthAttachment;

            for (int i = 0; i < 6; ++i)
            {
                if (CaptureDepthCubeMap)
                {
                    depthLayer = i;
                    depthAttachment = _envDepthTex;
                }
                else
                {
                    depthLayer = 0;
                    depthAttachment = _tempDepth;
                }

                RenderFBO.SetRenderTargets(
                    (_envTex, EFramebufferAttachment.ColorAttachment0, 0, i),
                    (depthAttachment, EFramebufferAttachment.DepthAttachment, 0, depthLayer));

                _viewport.AttachedCamera = RenderFBO.Cameras[i];
                _viewport.RenderWithLayoutUpdate(RenderFBO);
            }

            BaseRenderTexture tex = _envTex.RenderTextureGeneric;
            tex.Bind();
            tex.SetMipmapGenParams();
            tex.GenerateMipmaps();
        }
    }
}
