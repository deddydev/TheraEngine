using System;
using System.Collections.Concurrent;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Particles;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene3D : BaseScene
    {
        private TMaterial _voxelizationMaterial;

        //TODO: implement octree on GPU with compute shader instead of here on CPU
        //Also implement occlusion culling along with frustum culling
        public Octree RenderTree { get; private set; }
        public override int Count => RenderTree.Count;
        public LightManager Lights { get; private set; }
        public ParticleManager Particles { get; }
        public IBLProbeGridActor IBLProbeActor { get; set; }
        //private GlobalFileRef<TMaterial> _voxelizationMaterial;

        public Scene3D() : this(0.5f) { }
        public Scene3D(Vec3 boundsHalfExtents)
        {
            Clear(new BoundingBox(boundsHalfExtents, Vec3.Zero));
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
                new GLSLShaderFile[]
                {

                }
            );

            m.RenderParams.CullMode = ECulling.None;
            m.RenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            m.RenderParams.BlendMode.Enabled = ERenderParamUsage.Disabled;

            _voxelizationMaterial = m;
        }

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
        public override void CollectVisible(RenderPasses populatingPasses, IVolume collectionVolume, Camera camera, bool shadowPass)
        {
            RenderTree.CollectVisible(collectionVolume, populatingPasses, camera, shadowPass);
        }
        //public void RenderForward(RenderPasses renderingPasses, Camera camera, Viewport viewport, FrameBuffer target)
        //{
        //    AbstractRenderer.PushCamera(camera);
        //    AbstractRenderer.PushCurrent3DScene(this);
        //    {
        //        if (viewport != null)
        //        {
        //            //Enable internal resolution
        //            Engine.Renderer.PushRenderArea(viewport.InternalResolution);
        //            {
        //                viewport.ForwardPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
        //                {
        //                    Engine.Renderer.Clear(EBufferClear.All);

        //                    Engine.Renderer.AllowDepthWrite(false);
        //                    renderingPasses.Render(ERenderPass.Background);
        //                    Engine.Renderer.AllowDepthWrite(true);

        //                    renderingPasses.Render(ERenderPass.OpaqueForward);
        //                    //c.OwningComponent?.OwningWorld?.PhysicsWorld.DrawDebugWorld();
        //                    //RenderTree.DebugRender(camera?.Frustum, true);

        //                    renderingPasses.Render(ERenderPass.TransparentForward);
        //                    Engine.Renderer.AllowDepthWrite(false);
        //                    renderingPasses.Render(ERenderPass.OnTopForward);
        //                }
        //                viewport.ForwardPassFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

        //                //viewport.PingPongBloomBlurFBO.Reset();
        //                //viewport.PingPongBloomBlurFBO.BindCurrentTarget(EFramebufferTarget.DrawFramebuffer);
        //                //Engine.Renderer.AllowDepthWrite(false);
        //                //viewport.BrightPassFBO.RenderFullscreen();
        //                //Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);

        //                //var t = viewport._bloomBlurTexture.GetTexture(true);
        //                //t.Bind();
        //                //t.GenerateMipmaps();

        //                //for (int i = 0; i < 10; ++i)
        //                //{
        //                //    viewport.PingPongBloomBlurFBO.BindCurrentTarget(EFramebufferTarget.DrawFramebuffer);
        //                //    Engine.Renderer.AllowDepthWrite(false);
        //                //    viewport.PingPongBloomBlurFBO.RenderFullscreen();
        //                //    Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);

        //                //    viewport.PingPongBloomBlurFBO.Switch();
        //                //}
        //            }
        //            Engine.Renderer.PopRenderArea();

        //            //Render the last pass to the actual screen resolution, 
        //            //or the provided target FBO
        //            target?.Bind(EFramebufferTarget.DrawFramebuffer);
        //            {
        //                Engine.Renderer.PushRenderArea(viewport.Region);
        //                viewport.PostProcessFBO.RenderFullscreen();
        //                Engine.Renderer.PopRenderArea();
        //            }
        //            target?.Unbind(EFramebufferTarget.DrawFramebuffer);
        //        }
        //        else
        //        {
        //            target?.Bind(EFramebufferTarget.DrawFramebuffer);
        //            Engine.Renderer.Clear(EBufferClear.All);

        //            Engine.Renderer.AllowDepthWrite(false);
        //            renderingPasses.Render(ERenderPass.Background);

        //            Engine.Renderer.AllowDepthWrite(true);
        //            renderingPasses.Render(ERenderPass.OpaqueForward);
        //            renderingPasses.Render(ERenderPass.TransparentForward);

        //            //Render forward on-top objects last
        //            //Disable depth fail for objects on top
        //            Engine.Renderer.DepthFunc(EComparison.Always);
        //            renderingPasses.Render(ERenderPass.OnTopForward);
        //            target?.Unbind(EFramebufferTarget.DrawFramebuffer);
        //        }
        //    }
        //    AbstractRenderer.PopCurrent3DScene();
        //    AbstractRenderer.PopCamera();
        //}
        public void RenderDeferred(RenderPasses renderingPasses, Camera camera, Viewport viewport, FrameBuffer target)
        {
            //_timeQuery.BeginQuery(EQueryTarget.TimeElapsed);
            Engine.Renderer.PushCamera(camera);
            Engine.Renderer.PushCurrent3DScene(this);
            {
                if (viewport != null)
                {
                    viewport.RenderingCameras.Push(camera);

                    //Enable internal resolution
                    Engine.Renderer.PushRenderArea(viewport.InternalResolution);
                    {
                        //Render to deferred framebuffer.
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

                        viewport.SSAOBlurFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                        viewport.SSAOFBO.RenderFullscreen();
                        viewport.SSAOBlurFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

                        viewport.GBufferFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                        viewport.SSAOBlurFBO.RenderFullscreen();
                        viewport.GBufferFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

                        RenderLights(viewport);
                        RenderForwardPass(viewport, renderingPasses);
                        RenderBloom(viewport);

                        camera.PostProcessRef.File.ColorGrading.UpdateExposure(viewport.HDRSceneTexture);

                        //TODO: Apply camera post process material pass here
                        TMaterial post = camera?.PostProcessRef?.File?.PostProcessMaterial?.File;
                        if (post != null)
                        {

                        }
                    }
                    Engine.Renderer.PopRenderArea();

                    //Full viewport resolution now
                    Engine.Renderer.PushRenderArea(viewport.Region);
                    {
                        //Render the last pass to the actual screen resolution, 
                        //or the provided target FBO
                        target?.Bind(EFramebufferTarget.DrawFramebuffer);
                        viewport.PostProcessFBO.RenderFullscreen();
                        target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                    }
                    Engine.Renderer.PopRenderArea();
                    viewport.RenderingCameras.Pop();
                }
                else
                {
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);

                    Engine.Renderer.ClearDepth(1.0f);
                    Engine.Renderer.EnableDepthTest(true);
                    Engine.Renderer.AllowDepthWrite(true);
                    Engine.Renderer.StencilMask(~0);
                    Engine.Renderer.ClearStencil(0);
                    Engine.Renderer.Clear(target?.TextureTypes ?? EFBOTextureType.Color | EFBOTextureType.Depth | EFBOTextureType.Stencil);

                    Engine.Renderer.AllowDepthWrite(false);
                    renderingPasses.Render(ERenderPass.Background);
                    
                    Engine.Renderer.AllowDepthWrite(true);
                    renderingPasses.Render(ERenderPass.OpaqueDeferredLit);
                    renderingPasses.Render(ERenderPass.OpaqueForward);
                    renderingPasses.Render(ERenderPass.TransparentForward);

                    //Render forward on-top objects last
                    //Disable depth fail for objects on top
                    Engine.Renderer.DepthFunc(EComparison.Always);
                    renderingPasses.Render(ERenderPass.OnTopForward);

                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
            }
            Engine.Renderer.PopCurrent3DScene();
            Engine.Renderer.PopCamera();
            //_renderFPS = 1.0f / (_timeQuery.EndAndGetQueryInt() * 1e-9f);
            //Engine.PrintLine(_renderMS.ToString() + " ms");
        }
        public override void Add(IRenderable obj) => Add(obj as I3DRenderable);
        public override void Remove(IRenderable obj) => Remove(obj as I3DRenderable);
        public void Add(I3DRenderable obj)
        {
            if (obj != null && RenderTree?.Add(obj) == true)
            {
                obj.RenderInfo.Owner = obj;
                obj.RenderInfo.Scene = this;
                if (obj is I3DRenderable r && r.CullingVolume != null)
                    RegisterCullingVolume(r.CullingVolume);
                //Engine.PrintLine("Added {0} to the scene.", obj.ToString());
            }
        }
        public void Remove(I3DRenderable obj)
        {
            if (obj != null && RenderTree?.Remove(obj) == true)
            {
                obj.RenderInfo.Owner = null;
                obj.RenderInfo.Scene = null;
                if (obj is I3DRenderable r && r.CullingVolume != null)
                    UnregisterCullingVolume(r.CullingVolume);
                //Engine.PrintLine("Removed {0} from the scene.", obj.ToString());
            }
        }
        private void RegisterCullingVolume(Shape cullingVolume)
        {

        }
        private void UnregisterCullingVolume(Shape cullingVolume)
        {

        }

        /// <summary>
        /// Clears all items from the scene and sets the bounds.
        /// </summary>
        /// <param name="sceneBounds">The total extent of the items in the scene.</param>
        public void Clear(BoundingBox sceneBounds)
        {
            RenderTree = new Octree(sceneBounds);
            Lights = new LightManager();
        }

        private float _renderFPS;
        private RenderQuery _timeQuery = new RenderQuery();
        private void RenderForwardPass(Viewport viewport, RenderPasses renderingPasses)
        {
            viewport.ForwardPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
            {
                Engine.Renderer.EnableDepthTest(false);
                //Render the deferred pass result
                viewport.LightCombineFBO.RenderFullscreen();

                Engine.Renderer.EnableDepthTest(true);
                //viewport.RenderingCamera.OwningComponent?.OwningWorld?.PhysicsWorld.DrawDebugWorld();
                //RenderTree.DebugRender(viewport.RenderingCamera.Frustum, true);

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
        private void RenderLights(Viewport viewport)
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
        //private void RenderDeferredDecals(Viewport viewport)
        //{
        //    foreach (DecalComponent c in Decals)
        //        viewport.RenderDecal(c);
        //}
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
        private void BloomPass(QuadFrameBuffer fbo, BoundingRectangle rect, int mipmap)
        {
            Engine.Renderer.PushRenderArea(rect);
            {
                BloomBlur(fbo, rect, mipmap, 0.0f);
                BloomBlur(fbo, rect, mipmap, 1.0f);
            }
            Engine.Renderer.PopRenderArea();
        }
        private void RenderBloom(Viewport viewport)
        {
            viewport.BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            viewport.ForwardPassFBO.RenderFullscreen();
            viewport.BloomBlurFBO1.Unbind(EFramebufferTarget.DrawFramebuffer);

            var tex = viewport.BloomBlurTexture.GetTexture(true);
            tex.Bind();
            tex.GenerateMipmaps();

            BloomPass(viewport.BloomBlurFBO16, viewport.BloomRect16, 4);
            BloomPass(viewport.BloomBlurFBO8, viewport.BloomRect8, 3);
            BloomPass(viewport.BloomBlurFBO4, viewport.BloomRect4, 2);
            BloomPass(viewport.BloomBlurFBO2, viewport.BloomRect2, 1);
            //Don't blur original image, barely makes a difference to result
        }

        public override void RegenerateTree()
        {
            RenderTree.Remake();
        }

        public override void GlobalPreRender()
        {
            Voxelize();
            RenderShadowMaps();
        }
        public override void GlobalSwap()
        {
            Lights.SwapBuffers();
        }
        public override void GlobalUpdate()
        {
            UpdateShadowMaps();
        }
    }
}
