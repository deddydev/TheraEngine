﻿using System;
using System.Drawing;
using System.IO;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Actors.Types
{
    public class IBLProbeComponent : SceneCaptureComponent, I3DRenderable
    {
        private CubeFrameBuffer _irradianceFBO;
        private CubeFrameBuffer _prefilterFBO;
        private TexRefCube _irradianceTex;
        private TexRefCube _prefilterTex;

        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);
        public Shape CullingVolume => null;
        public IOctreeNode OctreeNode { get; set; }

        public TexRefCube IrradianceTex => _irradianceTex;
        public TexRefCube PrefilterTex => _prefilterTex;

        public IBLProbeComponent() : base()
        {
            _rc = new RenderCommandDebug3D(Render);
        }

        public override void OnSpawned()
        {
            OwningScene.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            OwningScene.Remove(this);
            base.OnDespawned();
        }
        protected override void OnWorldTransformChanged()
        {
            base.OnWorldTransformChanged();
            if (IsSpawned && _envTex != null)
            {
                Capture();
                GenerateIrradianceMap();
                GeneratePrefilterMap();
            }
        }
        //protected override FrameBuffer GetFBO(int cubeExtent)
        //{
        //    _cubeTex = new TexRefCube("SceneCaptureCubeMap", cubeExtent,
        //        EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
        //    {
        //        MinFilter = ETexMinFilter.LinearMipmapLinear,
        //        MagFilter = ETexMagFilter.Linear,
        //        UWrap = ETexWrapMode.ClampToEdge,
        //        VWrap = ETexWrapMode.ClampToEdge,
        //        WWrap = ETexWrapMode.ClampToEdge,
        //    };

        //    TMaterial mat = new TMaterial("PassThruEnvMat", new BaseTexRef[] { _cubeTex }, 
        //        Engine.LoadEngineShader(Path.Combine("Common", "UnlitTexturedForward.fs"), EShaderMode.Fragment));

        //    CubeFrameBuffer f = new CubeFrameBuffer(mat, true);
        //    f.SetRenderTargets((_cubeTex, EFramebufferAttachment.ColorAttachment0, 0));
        //    return f;
        //}
        protected override void InitializeForCapture()
        {
            base.InitializeForCapture();

            _irradianceTex = new TexRefCube("IrradianceTex", _colorRes,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.Linear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
            };
            _prefilterTex = new TexRefCube("PrefilterTex", _colorRes,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                MinFilter = ETexMinFilter.LinearMipmapLinear,
                MagFilter = ETexMagFilter.Linear,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                WWrap = ETexWrapMode.ClampToEdge,
            };

            ShaderVar[] prefilterVars = new ShaderVar[]
            {
                new ShaderFloat(0.0f, "Roughness"),
                new ShaderInt(_colorRes, "CubemapDim"),
            };
            GLSLShaderFile irrShader = Engine.LoadEngineShader(Path.Combine("Scene3D", "IrradianceConvolution.fs"), EShaderMode.Fragment);
            GLSLShaderFile prefShader = Engine.LoadEngineShader(Path.Combine("Scene3D", "Prefilter.fs"), EShaderMode.Fragment);
            RenderingParameters r = new RenderingParameters();
            r.DepthTest.Enabled = ERenderParamUsage.Unchanged;
            TMaterial irrMat = new TMaterial("IrradianceMat", r, new ShaderVar[0], new TexRefCube[] { _envTex }, irrShader);
            TMaterial prefMat = new TMaterial("PrefilterMat", r, prefilterVars, new TexRefCube[] { _envTex }, prefShader);
            _irradianceFBO = new CubeFrameBuffer(irrMat, 0.1f, 3.0f, false);
            _prefilterFBO = new CubeFrameBuffer(prefMat, 0.1f, 3.0f, false);
        }
        public void GenerateIrradianceMap()
        {
            int res = _prefilterFBO.Material.Parameter<ShaderInt>(1).Value;
            for (int i = 0; i < 6; ++i)
            {
                _irradianceFBO.SetRenderTargets((_irradianceTex, EFramebufferAttachment.ColorAttachment0, 0, i));

                ECubemapFace face = ECubemapFace.PosX + i;

                Engine.Renderer.PopRenderArea();
                _irradianceFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    Engine.Renderer.Clear(EBufferClear.Color);
                    _irradianceFBO.RenderFullscreen(face);
                }
                _irradianceFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
                Engine.Renderer.PushRenderArea(new BoundingRectangle(IVec2.Zero, new IVec2(res, res)));
            }
        }
        public void GeneratePrefilterMap()
        {
            RenderTexCube cube = _prefilterTex.GetTexture(true);
            cube.Bind();
            cube.SetMipmapGenParams();
            cube.GenerateMipmaps();

            int maxMipLevels = 5;
            int res = _prefilterFBO.Material.Parameter<ShaderInt>(1).Value;
            for (int mip = 0; mip < maxMipLevels; ++mip)
            {
                int mipWidth = (int)(res * Math.Pow(0.5, mip));
                int mipHeight = (int)(res * Math.Pow(0.5, mip));
                float roughness = (float)mip / (maxMipLevels - 1);

                //_prefilterDepth.SetStorage(ERenderBufferStorage.DepthComponent16, mipWidth, mipHeight);
                _prefilterFBO.Material.Parameter<ShaderFloat>(0).Value = roughness;

                for (int i = 0; i < 6; ++i)
                {
                    _prefilterFBO.SetRenderTargets((_prefilterTex, EFramebufferAttachment.ColorAttachment0, mip, i));

                    ECubemapFace face = ECubemapFace.PosX + i;

                    Engine.Renderer.PushRenderArea(new BoundingRectangle(IVec2.Zero, new IVec2(mipWidth, mipHeight)));
                    _prefilterFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                    {
                        Engine.Renderer.Clear(EBufferClear.Color);
                        _prefilterFBO.RenderFullscreen(face);
                    }
                    _prefilterFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
                    Engine.Renderer.PopRenderArea();
                }
            }
        }
        private void Render()
        {
            Engine.Renderer.RenderPoint(WorldPoint, Color.Red);
        }
        private RenderCommandDebug3D _rc;
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(_rc, RenderInfo.RenderPass);
        }
    }
}
