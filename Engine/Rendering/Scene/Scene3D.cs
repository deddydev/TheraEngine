using System;
using System.Collections.Generic;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Shapes;

namespace TheraEngine.Rendering
{
    public interface IScene3D : IScene
    {
        IOctree RenderTree { get; }
        LightManager Lights { get; }
        IBLProbeGridActor IBLProbeActor { get; set; }
        IEventList<I3DRenderable> Renderables { get; }

        void UpdateShadowMaps();
        void RenderShadowMaps();
        void Voxelize(bool clearVoxelization = true);
        void Clear(BoundingBoxStruct sceneBounds);

    }
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene3D : BaseScene, IScene3D
    {
        private TMaterial _voxelizationMaterial;

        //TODO: implement octree on GPU with compute shader instead of here on CPU
        //Also implement occlusion culling along with frustum culling
        public IOctree RenderTree { get; private set; }
        //public override int Count => RenderTree.Count;
        public LightManager Lights { get; private set; }
        //public ParticleManager Particles { get; }
        public IBLProbeGridActor IBLProbeActor { get; set; }
        public IEventList<I3DRenderable> Renderables { get; }
        
        public Scene3D() : this(0.5f) { }
        public Scene3D(Vec3 boundsHalfExtents)
        {
            Renderables = new EventList<I3DRenderable>();
            Renderables.PostAnythingAdded += Renderables_PostAnythingAdded;
            Renderables.PostAnythingRemoved += Renderables_PostAnythingRemoved;

            RenderTree = new Octree(new BoundingBoxStruct(boundsHalfExtents, Vec3.Zero));
            Lights = new LightManager();
            
            Render = RenderDeferred;

            TMaterial m = new TMaterial("VoxelizeMat",
                new ShaderVar[] { },
                new BaseTexRef[]
                {
                    new TexRef3D("VoxelScene")
                    {
                        Resizable = false
                    },
                },
                new GLSLScript[]
                {

                }
            );

            m.RenderParams.CullMode = ECulling.None;
            m.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            m.RenderParams.BlendMode.Enabled = ERenderParamUsage.Disabled;

            _voxelizationMaterial = m;
        }

        private void Renderables_PostAnythingAdded(I3DRenderable item) => RenderTree.Add(item);
        private void Renderables_PostAnythingRemoved(I3DRenderable item) => RenderTree.Remove(item);

        public void UpdateShadowMaps() => Lights?.UpdateShadowMaps(this);
        public void RenderShadowMaps() => Lights?.RenderShadowMaps(this);
        public void Voxelize(bool clearVoxelization = true)
        {
            //TMaterial m = _voxelizationMaterial;
            //RenderTex3D tex = m.Textures[0].RenderTextureGeneric as RenderTex3D;
            //if (clearVoxelization)
            //{
            //    voxelTexture->Clear(clearColor);
            //}
            //Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
            //Engine.Renderer.ColorMask(false, false, false, false);
            //Engine.Renderer.PushRenderArea(new BoundingRectangle(0, 0, tex.Width, tex.Height, 0.0f, 0.0f));
            //Engine.Renderer.MaterialOverride = m;
            //{
            //    // Texture.
            //    tex.Bind();
                
            //    voxelTexture->Activate(material->program, "texture3D", 0);
            //    Engine.Renderer.BindImageTexture(0, voxelTexture->textureID, 0, GL_TRUE, 0, GL_WRITE_ONLY, GL_RGBA8);

            //    // Lighting.
            //    uploadLighting(renderingScene, material->program);

            //    // Render.
            //    renderQueue(renderingScene.renderers, material->program, true);

            //    Engine.Renderer.GenerateMipmap(ETexTarget.Texture3D);
            //}
            //Engine.Renderer.MaterialOverride = null;
            //Engine.Renderer.PopRenderArea();
            //Engine.Renderer.ColorMask(true, true, true, true);
        }
        public override void CollectVisible(RenderPasses populatingPasses, IVolume collectionVolume, ICamera camera, bool shadowPass)
        {
            RenderTree.CollectVisible(collectionVolume, populatingPasses, camera, shadowPass);
        }
        public abstract class RenderSequenceCommand
        {

        }
        public abstract class RenderSequenceViewportCommand
        {

        }
        private bool _queueRemake = false;
        public override void RegenerateTree()
        {
            _queueRemake = true;
        }
        public override void GlobalPreRender()
        {
            Voxelize();
            RenderShadowMaps();
        }
        public override void GlobalSwap()
        {
            Lights.SwapBuffers();
            RenderTree.Swap();
            //if (_queueRemake)
            //{
            //    RenderTree.Remake();
            //    _queueRemake = false;
            //}
        }
        public override void GlobalUpdate()
        {
            UpdateShadowMaps();
        }
        /// <summary>
        /// Clears all items from the scene and sets the bounds.
        /// </summary>
        /// <param name="sceneBounds">The total extent of the items in the scene.</param>
        public void Clear(BoundingBoxStruct sceneBounds)
        {
            Renderables.Clear();
            RenderTree = new Octree(sceneBounds);
            Lights = new LightManager();
        }

        private float _renderFPS;
        private RenderQuery _timeQuery = new RenderQuery();

        #region Passes
        public void RenderDeferred(RenderPasses renderingPasses, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            AbstractRenderer renderer = Engine.Renderer;
            //_timeQuery.BeginQuery(EQueryTarget.TimeElapsed);
            renderer.PushCamera(camera);
            renderer.PushCurrent3DScene(this);
            try
            {
                if (viewport != null)
                {
                    if (!viewport.FBOsInitialized)
                        viewport.InitializeFBOs();

                    viewport.PushRenderingCamera(camera);

                    //Enable internal resolution
                    renderer.PushRenderArea(viewport.InternalResolution);
                    {
                        RenderDeferredPass(viewport, renderingPasses);

                        viewport.SSAOFBO.RenderTo(viewport.SSAOBlurFBO);
                        viewport.SSAOBlurFBO.RenderTo(viewport.GBufferFBO);

                        RenderLightPass(viewport);
                        RenderForwardPass(viewport, renderingPasses);
                        RenderBloomPass(viewport);

                        if (camera.UsesAutoExposure)
                        {
                            viewport.HDRSceneTexture.CalcDotLuminance();
                        }

                        if (camera is TypicalCamera typicalCamera)
                        {
                            typicalCamera.UpdateExposure(viewport.HDRSceneTexture);

                            TMaterial postMat = typicalCamera.PostProcessMaterial?.File;
                            if (postMat != null)
                                RenderPostProcessPass(viewport, postMat);
                        }
                        else if (camera is BlendableCamera blendableCamera)
                        {
                            blendableCamera.UpdateExposure(viewport.HDRSceneTexture);
                        }
                    }
                    renderer.PopRenderArea();

                    //Full viewport resolution now
                    renderer.PushRenderArea(viewport.Region);
                    {
                        //Render the last pass to the actual screen resolution, 
                        //or the provided target FBO
                        target?.Bind(EFramebufferTarget.DrawFramebuffer);
                        viewport.PostProcessFBO.RenderFullscreen();
                        target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                    }
                    renderer.PopRenderArea();
                    viewport.PopRenderingCamera();
                }
                else
                {
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);

                    renderer.ClearDepth(1.0f);
                    renderer.EnableDepthTest(true);
                    renderer.AllowDepthWrite(true);
                    renderer.StencilMask(~0);
                    renderer.ClearStencil(0);
                    renderer.Clear(target?.TextureTypes ?? EFBOTextureType.Color | EFBOTextureType.Depth | EFBOTextureType.Stencil);

                    renderer.AllowDepthWrite(false);
                    renderingPasses.Render(ERenderPass.Background);

                    renderer.AllowDepthWrite(true);
                    renderingPasses.Render(ERenderPass.OpaqueDeferredLit);
                    renderingPasses.Render(ERenderPass.OpaqueForward);
                    renderingPasses.Render(ERenderPass.TransparentForward);

                    //Render forward on-top objects last
                    //Disable depth fail for objects on top
                    renderer.DepthFunc(EComparison.Always);
                    renderingPasses.Render(ERenderPass.OnTopForward);

                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
            }
            finally
            {
                renderer.PopCurrent3DScene();
                renderer.PopCamera();
            }
            //_renderFPS = 1.0f / (_timeQuery.EndAndGetQueryInt() * 1e-9f);
            //Engine.PrintLine(_renderMS.ToString() + " ms");
        }
        private void RenderPostProcessPass(Viewport viewport, TMaterial post)
        {
            //TODO: Apply camera post process material pass here

        }
        private void RenderDeferredPass(Viewport viewport, RenderPasses renderingPasses)
        {
            viewport.SSAOFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                Engine.Renderer.StencilMask(~0);
                Engine.Renderer.ClearStencil(0);
                Engine.Renderer.Clear(EFBOTextureType.Color | EFBOTextureType.Depth | EFBOTextureType.Stencil);
                Engine.Renderer.EnableDepthTest(true);
                Engine.Renderer.ClearDepth(1.0f);
                renderingPasses.Render(ERenderPass.OpaqueDeferredLit);
                renderingPasses.Render(ERenderPass.DeferredDecals);
                Engine.Renderer.EnableDepthTest(false);
            }
            viewport.SSAOFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void RenderForwardPass(Viewport viewport, RenderPasses renderingPasses)
        {
            viewport.ForwardPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                Engine.Renderer.EnableDepthTest(false);

                //Render the deferred pass lighting result
                viewport.LightCombineFBO.RenderFullscreen();

                Engine.Renderer.EnableDepthTest(true);
                renderingPasses.Render(ERenderPass.OpaqueForward);

                Engine.Renderer.AllowDepthWrite(false);
                renderingPasses.Render(ERenderPass.Background);
                Engine.Renderer.EnableDepthTest(true);

                //Render forward transparent objects next
                renderingPasses.Render(ERenderPass.TransparentForward);

                //Render forward on-top objects last
                renderingPasses.Render(ERenderPass.OnTopForward);
                Engine.Renderer.EnableDepthTest(false);
            }
            viewport.ForwardPassFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void RenderLightPass(Viewport viewport)
        {
            viewport.LightCombineFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                //Start with blank slate so additive blending doesn't ghost old frames
                Engine.Renderer.Clear(EFBOTextureType.Color);
                foreach (PointLightComponent c in Lights.PointLights)
                    viewport.RenderPointLight(c);
                foreach (SpotLightComponent c in Lights.SpotLights)
                    viewport.RenderSpotLight(c);
                foreach (DirectionalLightComponent c in Lights.DirectionalLights)
                    viewport.RenderDirLight(c);
            }
            viewport.LightCombineFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void BloomBlur(QuadFrameBuffer fbo, BoundingRectangle rect, int mipmap, float dir)
        {
            fbo.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                fbo.Material.Parameter<ShaderFloat>(0).Value = dir;
                fbo.Material.Parameter<ShaderInt>(1).Value = mipmap;
                fbo.RenderFullscreen();
            }
            fbo.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void BloomScaledPass(QuadFrameBuffer fbo, BoundingRectangle rect, int mipmap)
        {
            Engine.Renderer.PushRenderArea(rect);
            {
                BloomBlur(fbo, rect, mipmap, 0.0f);
                BloomBlur(fbo, rect, mipmap, 1.0f);
            }
            Engine.Renderer.PopRenderArea();
        }
        private void RenderBloomPass(Viewport viewport)
        {
            viewport.BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            viewport.ForwardPassFBO.RenderFullscreen();
            viewport.BloomBlurFBO1.Unbind(EFramebufferTarget.DrawFramebuffer);

            var tex = viewport.BloomBlurTexture.GetTexture(true);
            tex.Bind();
            tex.GenerateMipmaps();

            BloomScaledPass(viewport.BloomBlurFBO16, viewport.BloomRect16, 4);
            BloomScaledPass(viewport.BloomBlurFBO8, viewport.BloomRect8, 3);
            BloomScaledPass(viewport.BloomBlurFBO4, viewport.BloomRect4, 2);
            BloomScaledPass(viewport.BloomBlurFBO2, viewport.BloomRect2, 1);
            //Don't blur original image, barely makes a difference to result
        }
        #endregion

        public override IEnumerator<IRenderable> GetEnumerator() => Renderables.GetEnumerator();
    }
}
