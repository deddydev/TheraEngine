using System;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Rendering.Models;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering;

namespace TheraEngine.Actors.Types
{
    public class SceneCaptureComponent : TranslationComponent
    {
        private PerspectiveCamera[] _cameras;
        private PrimitiveManager _cube;
        private MaterialFrameBuffer _shadowMap;
        private int _shadowResolution;
        private Viewport _viewport = new Viewport(1.0f, 1.0f);

        public SceneCaptureComponent()
        {
           

            PrimitiveData cubeData = BoundingBox.SolidMesh(-1.0f, 1.0f, true);
            TexRefCube cubeMap = new TexRefCube("SceneCaptureCubeMap", 512,
                EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
            {
                FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
            };
            BaseTexRef[] textures = new BaseTexRef[] { cubeMap };
            Engine.LoadEngineShader("SceneCaptureCube.fs", EShaderMode.Fragment);
            TMaterial mat = new TMaterial("SceneCaptureMat", textures);
            _cube = new PrimitiveManager(cubeData, mat);
        }
        /// <summary>
        /// This is to set special uniforms each time something is rendered with the shadow depth shader.
        /// </summary>
        private void SetShadowDepthUniforms(int programBindingId)
        {
            //Engine.Renderer.Uniform(programBindingId, "FarPlaneDist", Radius);
            Engine.Renderer.Uniform(programBindingId, "LightPos", WorldPoint);
            for (int i = 0; i < _cameras.Length; ++i)
                Engine.Renderer.Uniform(programBindingId, string.Format("ShadowMatrices[{0}]", i), _cameras[i].WorldToCameraProjSpaceMatrix);
        }
        public void SetShadowMapResolution(int resolution)
        {
            _shadowResolution = resolution;
            if (_shadowMap == null)
                Initialize(resolution);
            else
                _shadowMap.ResizeTextures(resolution, resolution);
        }
        private void Initialize(int resolution)
        {
            _cameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f,   0.0f, 0.0f, RotationOrder.YPR), //forward
                new Rotator(0.0f, 180.0f, 0.0f, RotationOrder.YPR), //backward
                new Rotator(0.0f, -90.0f, 0.0f, RotationOrder.YPR), //left
                new Rotator(0.0f,  90.0f, 0.0f, RotationOrder.YPR), //right
                new Rotator(90.0f,  0.0f, 0.0f, RotationOrder.YPR), //up
                new Rotator(-90.0f, 0.0f, 0.0f, RotationOrder.YPR), //down
            };

            for (int i = 0; i < 6; ++i)
                _cameras[i] = new PerspectiveCamera(
                    Vec3.Zero, rotations[i], 1.0f, 10000.0f, 90.0f, 1.0f);

            _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(resolution));
            _shadowMap.Material.SettingUniforms += SetShadowDepthUniforms;

            _shadowMap.Generate();
        }
        private static TMaterial GetShadowMapMaterial(int cubeExtent)
        {
            //These are listed in order of appearance in the shader
            TexRefCube[] refs = new TexRefCube[]
            {
                new TexRefCube("SceneCaptureCubeMap", cubeExtent,
                    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    WWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                },
            };
            ShaderFile fragShader = Engine.LoadEngineShader("SceneCaptureCube.fs", EShaderMode.Fragment);
            ShaderFile geomShader = Engine.LoadEngineShader("PointLightShadowDepth.gs", EShaderMode.Geometry);
            TMaterial mat = new TMaterial("SceneCaptureMat", new ShaderVar[0], refs, fragShader, geomShader);
            mat.RenderParams.CullMode = Culling.None;
            return mat;
        }

        public void Capture() => Capture(OwningWorld.Settings.Bounds.HalfExtents.LengthFast);
        public void Capture(float distance)
        {
            BaseScene scene = OwningScene;
            //_viewport.Render(scene, Camera, Camera.Frustum, _shadowMap);

            //prefilterShader.use();
            //prefilterShader.setInt("environmentMap", 0);
            //glActiveTexture(GL_TEXTURE0);
            //glBindTexture(GL_TEXTURE_CUBE_MAP, envCubemap);

            //glBindFramebuffer(GL_FRAMEBUFFER, captureFBO);
            //unsigned int maxMipLevels = 5;
            //for (unsigned int mip = 0; mip < maxMipLevels; ++mip)
            //{
            //    // resize framebuffer according to mip-level size.
            //    unsigned int mipWidth = 128 * std::pow(0.5, mip);
            //    unsigned int mipHeight = 128 * std::pow(0.5, mip);
            //    glBindRenderbuffer(GL_RENDERBUFFER, captureRBO);
            //    glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT24, mipWidth, mipHeight);
            //    glViewport(0, 0, mipWidth, mipHeight);

            //    float roughness = (float)mip / (float)(maxMipLevels - 1);
            //    prefilterShader.setFloat("roughness", roughness);
            //    for (unsigned int i = 0; i < 6; ++i)
            //    {
            //        prefilterShader.setMat4("projection", captureProjection);
            //        prefilterShader.setMat4("view", captureViews[i]);
            //        glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, prefilterMap, mip);

            //        glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);
            //        renderCube();
            //    }
            //}
            //glBindFramebuffer(GL_FRAMEBUFFER, 0);

            //TODO: render to each of the sky texture's sides
            //Engine.Scene.Render(cam, cam.Frustum, v, false);
            foreach (PerspectiveCamera cam in _cameras)
            {
                
            }
        }
    }
}
