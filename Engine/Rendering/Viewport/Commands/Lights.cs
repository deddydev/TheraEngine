using System;
using System.IO;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class LightPassVPRC : ViewportRenderCommand
    {
        private MeshRenderer _pointLightRenderer;
        public MeshRenderer PointLightRenderer
        {
            get => _pointLightRenderer;
            private set
            {
                if (_pointLightRenderer != null)
                {
                    _pointLightRenderer.SettingUniforms -= LightManager_SettingUniforms;
                    _pointLightRenderer.Dispose();
                }

                _pointLightRenderer = value;

                if (_pointLightRenderer != null)
                    _pointLightRenderer.SettingUniforms += LightManager_SettingUniforms;
            }
        }

        private MeshRenderer _spotLightRenderer;
        public MeshRenderer SpotLightRenderer
        {
            get => _spotLightRenderer;
            private set
            {
                if (_spotLightRenderer != null)
                {
                    _spotLightRenderer.SettingUniforms -= LightManager_SettingUniforms;
                    _spotLightRenderer.Dispose();
                }

                _spotLightRenderer = value;

                if (_spotLightRenderer != null)
                    _spotLightRenderer.SettingUniforms += LightManager_SettingUniforms;
            }
        }

        private MeshRenderer _dirLightRenderer;
        public MeshRenderer DirLightRenderer
        {
            get => _dirLightRenderer;
            private set
            {
                if (_dirLightRenderer != null)
                {
                    _dirLightRenderer.SettingUniforms -= LightManager_SettingUniforms;
                    _dirLightRenderer.Dispose();
                }

                _dirLightRenderer = value;

                if (_dirLightRenderer != null)
                    _dirLightRenderer.SettingUniforms += LightManager_SettingUniforms;
            }
        }

        private QuadFrameBuffer _lightCombineFBO;
        public QuadFrameBuffer LightCombineFBO
        {
            get => _lightCombineFBO;
            private set
            {
                _lightCombineFBO?.Dispose();
                _lightCombineFBO = value;
            }
        }

        private ICamera _renderingCamera;
        private LightComponent _lightComp;

        public override void DestroyFBOs()
        {
            LightCombineFBO = null;
            PointLightRenderer = null;
            SpotLightRenderer = null;
            DirLightRenderer = null;
        }
        public override void GenerateFBOs(Viewport viewport)
        {
            TexRef2D normalTex = viewport.Pipeline.GetTexture<TexRef2D>("Normal");
            TexRefView2D depthViewTex = viewport.Pipeline.GetTexture<TexRefView2D>("DepthView");
            TexRef2D albedoTex = viewport.Pipeline.GetTexture<TexRef2D>("AlbedoOpacity");
            TexRef2D rmsiTex = viewport.Pipeline.GetTexture<TexRef2D>("RMSI");
            TexRef2D depthStencilTex = viewport.Pipeline.GetTexture<TexRef2D>("DepthStencil");
            TexRef2D ssaoTex = viewport.Pipeline.GetTexture<TexRef2D>("SSAO");

            int width = viewport.InternalResolution.Width;
            int height = viewport.InternalResolution.Height;

            TexRef2D brdfTex = PrecomputeBRDF(viewport);

            RenderingParameters renderParams = new RenderingParameters()
            {
                DepthTest =
                {
                    Enabled = ERenderParamUsage.Unchanged,
                    UpdateDepth = false,
                    Function = EComparison.Always,
                }
            };

            RenderingParameters additiveRenderParams = new RenderingParameters
            {
                //Render only the backside so that the light still shows if the camera is inside of the volume
                //and the light does not add itself twice for the front and back faces.
                CullMode = ECulling.Front,
                Requirements = EUniformRequirements.Camera,
                BlendMode =
                {
                    //Add the previous and current light colors together using FuncAdd with each mesh render
                    Enabled = ERenderParamUsage.Enabled,
                    RgbDstFactor = EBlendingFactor.One,
                    AlphaDstFactor = EBlendingFactor.One,
                    RgbSrcFactor = EBlendingFactor.One,
                    AlphaSrcFactor = EBlendingFactor.One,
                    RgbEquation = EBlendEquationMode.FuncAdd,
                    AlphaEquation = EBlendEquationMode.FuncAdd,
                },
                DepthTest =
                {
                    Enabled = ERenderParamUsage.Disabled
                }
            };

            TexRef2D lightingTex = TexRef2D.CreateFrameBufferTexture(
                "Diffuse",
                width,
                height,
                EPixelInternalFormat.Rgb16f,
                EPixelFormat.Rgb,
                EPixelType.HalfFloat);

            GLSLScript lightCombineShader = Engine.Files.Shader(
                Path.Combine(SceneShaderPath, "DeferredLightCombine.fs"), EGLSLType.Fragment);

            BaseTexRef[] combineRefs = new BaseTexRef[]
            {
                albedoTex,
                normalTex,
                rmsiTex,
                ssaoTex,
                depthViewTex,
                lightingTex,
                brdfTex,
                //irradiance
                //prefilter
            };

            LightCombineFBO = new QuadFrameBuffer(new TMaterial("LightCombineMat", renderParams, combineRefs, lightCombineShader));
            LightCombineFBO.SetRenderTargets((lightingTex, EFramebufferAttachment.ColorAttachment0, 0, -1));
            LightCombineFBO.SettingUniforms += LightCombineFBO_SettingUniforms;
            viewport.Pipeline.SetFBO("LightCombine", LightCombineFBO);

            TexRef2D[] lightRefs = new TexRef2D[]
            {
                albedoTex,
                normalTex,
                rmsiTex,
                depthViewTex,
                //shadow map texture
            };

            GLSLScript pointLightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "DeferredLightingPoint.fs"), EGLSLType.Fragment);
            GLSLScript spotLightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "DeferredLightingSpot.fs"), EGLSLType.Fragment);
            GLSLScript dirLightShader = Engine.Files.Shader(Path.Combine(SceneShaderPath, "DeferredLightingDir.fs"), EGLSLType.Fragment);

            TMaterial pointLightMat = new TMaterial("PointLightMat", additiveRenderParams, lightRefs, pointLightShader);
            TMaterial spotLightMat = new TMaterial("SpotLightMat", additiveRenderParams, lightRefs, spotLightShader);
            TMaterial dirLightMat = new TMaterial("DirLightMat", additiveRenderParams, lightRefs, dirLightShader);

            TMesh pointLightMesh = Sphere.SolidMesh(Vec3.Zero, 1.0f, 20u);
            TMesh spotLightMesh = Cone.SolidMesh(Vec3.Zero, Vec3.UnitZ, 1.0f, 1.0f, 32, true);
            TMesh dirLightMesh = BoundingBox.SolidMesh(-Vec3.Half, Vec3.Half);

            PointLightRenderer = new MeshRenderer(pointLightMesh, pointLightMat);
            SpotLightRenderer = new MeshRenderer(spotLightMesh, spotLightMat);
            DirLightRenderer = new MeshRenderer(dirLightMesh, dirLightMat);
        }

        protected TexRef2D PrecomputeBRDF(Viewport viewport, int width = 2048, int height = 2048)
        {
            RenderingParameters renderParams = new RenderingParameters()
            {
                DepthTest =
                {
                    Enabled = ERenderParamUsage.Disabled,
                    Function = EComparison.Always,
                    UpdateDepth = false,
                }
            };

            TexRef2D brdfTex = TexRef2D.CreateFrameBufferTexture("BRDF", width, height, EPixelInternalFormat.Rg16f, EPixelFormat.Rg, EPixelType.HalfFloat);
            brdfTex.Resizable = true;
            brdfTex.UWrap = ETexWrapMode.ClampToEdge;
            brdfTex.VWrap = ETexWrapMode.ClampToEdge;
            brdfTex.MinFilter = ETexMinFilter.Linear;
            brdfTex.MagFilter = ETexMagFilter.Linear;
            brdfTex.SamplerName = "BRDF";
            viewport.Pipeline.SetTexture(brdfTex);

            TMaterial mat = new TMaterial(
                "BRDFMat",
                renderParams,
                new[] { brdfTex },
                Engine.Files.Shader(Path.Combine(SceneShaderPath, "BRDF.fs"), EGLSLType.Fragment));

            //ndc space quad, so we don't have to load any camera matrices
            VertexTriangle[] tris = VertexQuad.MakeQuad(
                    new Vec3(-1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, -1.0f, -0.5f),
                    new Vec3(1.0f, 1.0f, -0.5f),
                    new Vec3(-1.0f, 1.0f, -0.5f),
                    false, false).ToTriangles();

            using MaterialFrameBuffer fbo = new MaterialFrameBuffer(mat);
            fbo.SetRenderTargets((brdfTex, EFramebufferAttachment.ColorAttachment0, 0, -1));

            using TMesh data = TMesh.Create(VertexShaderDesc.PosTex(), tris);
            using MeshRenderer quad = new MeshRenderer(data, mat);
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

            return brdfTex;
        }

        public override void Execute(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            _renderingCamera = viewport.RenderingCamera;

            //Viewport light combine fbo
            LightCombineFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                //Start with blank slate so additive blending doesn't ghost old frames
                Engine.Renderer.Clear(EFBOTextureType.Color);

                //TODO: only run this entire pass if the scene requests lights (2D or 3D)
                if (scene is Scene3D s3d)
                {
                    foreach (PointLightComponent c in s3d.Lights.PointLights)
                        RenderPointLight(c);

                    foreach (SpotLightComponent c in s3d.Lights.SpotLights)
                        RenderSpotLight(c);

                    foreach (DirectionalLightComponent c in s3d.Lights.DirectionalLights)
                        RenderDirLight(c);
                }
            }
            LightCombineFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

            _renderingCamera = null;
        }

        internal void RenderDirLight(DirectionalLightComponent c)
            => RenderLight(DirLightRenderer, c);
        internal void RenderPointLight(PointLightComponent c)
            => RenderLight(PointLightRenderer, c);
        internal void RenderSpotLight(SpotLightComponent c)
            => RenderLight(SpotLightRenderer, c);
        private void RenderLight(MeshRenderer renderer, LightComponent comp)
        {
            _lightComp = comp;
            renderer.Render(comp.LightMatrix, Matrix3.Identity);
            _lightComp = null;
        }

        private void LightManager_SettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            if (_renderingCamera is null)
                return;

            //RenderingCamera.PostProcessRef.File.Shadows.SetUniforms(materialProgram);
            _lightComp.SetShadowUniforms(materialProgram);
            _lightComp.SetUniforms(materialProgram, null);
        }
        private void LightCombineFBO_SettingUniforms(RenderProgram program)
        {
            if (_renderingCamera is null)
                return;

            _renderingCamera.SetUniforms(program);

            var probeActor = _renderingCamera.OwningComponent?.OwningScene3D?.IBLProbeActor;
            if (probeActor is null || probeActor.RootComponent.ChildComponents.Count == 0)
                return;

            IBLProbeComponent probe = (IBLProbeComponent)probeActor.RootComponent.ChildComponents[0];
            int baseCount = LightCombineFBO.Material.Textures.Count;

            program.Sampler("Irradiance", probe.IrradianceTex?.GetTexture(true), baseCount);
            program.Sampler("Prefilter", probe.PrefilterTex?.GetTexture(true), ++baseCount);
        }
    }
}
