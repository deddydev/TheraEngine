using System;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Particles;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene3D : BaseScene
    {
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
            //TMaterial m = new TMaterial("VoxelizeMat",
            //    new ShaderVar[]
            //    {

            //    },
            //    new BaseTexRef[]
            //    {
            //        new TexRef3D("VoxelScene"),
            //    },
            //    new Shader[]
            //    {

            //    }
            //);

            //m.RenderParams.CullMode = Culling.None;
            //m.RenderParams.DepthTest.Enabled = false;
            //m.RenderParams.BlendMode.Enabled = false;

            //_voxelizationMaterial = m;
        }

        public void UpdateShadowMaps() => Lights?.UpdateShadowMaps(this);
        public void RenderShadowMaps() => Lights?.RenderShadowMaps(this);
        public void Voxelize()
        {
            //TMaterial m = _voxelizationMaterial.File;
            //RenderTex3D tex = m.Textures[0].GetTextureGeneric() as RenderTex3D;

            //Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
            //Engine.Renderer.ColorMask(false, false, false, false);
            //Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, tex.Width, tex.Height, 0.0f, 0.0f));
            //Engine.Renderer.MaterialOverride = m;
            //{
            //    // Texture.
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
            AbstractRenderer.PushCamera(camera);
            AbstractRenderer.PushCurrent3DScene(this);
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
                            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth | EBufferClear.Stencil);
                            Engine.Renderer.EnableDepthTest(true);
                            Engine.Renderer.ClearDepth(1.0f);
                            renderingPasses.Render(ERenderPass.OpaqueDeferredLit);
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
                    Engine.Renderer.StencilMask(~0);
                    Engine.Renderer.ClearStencil(0);
                    //Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth | EBufferClear.Stencil);

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
            AbstractRenderer.PopCurrent3DScene();
            AbstractRenderer.PopCamera();
        }

        
        public void Add(I3DRenderable obj)
        {
            if (RenderTree?.Add(obj) == true)
            {
                obj.RenderInfo.Scene = this;
                if (obj is I3DRenderable r && r.CullingVolume != null)
                    RegisterCullingVolume(r.CullingVolume);
                //Engine.PrintLine("Added {0} to the scene.", obj.ToString());
            }
        }
        public void Remove(I3DRenderable obj)
        {
            if (RenderTree?.Remove(obj) == true)
            {
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
                Engine.Renderer.Clear(EBufferClear.Color);

                foreach (PointLightComponent c in Lights.PointLights)
                {
                    viewport.RenderPointLight(c);
                }
                foreach (SpotLightComponent c in Lights.SpotLights)
                {
                    viewport.RenderSpotLight(c);
                }
                foreach (DirectionalLightComponent c in Lights.DirectionalLights)
                {
                    viewport.RenderDirLight(c);
                }
            }
            viewport.LightCombineFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        }
        private void RenderBloom(Viewport viewport)
        {
            viewport.BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            viewport.ForwardPassFBO.RenderFullscreen();
            viewport.BloomBlurFBO1.Unbind(EFramebufferTarget.DrawFramebuffer);

            var tex = viewport._bloomBlurTexture.GetTexture(true);
            tex.Bind();
            tex.GenerateMipmaps();

            Engine.Renderer.PushRenderArea(new BoundingRectangle(0, 0,
                (int)(viewport.InternalResolution.Width * 0.0625f),
                (int)(viewport.InternalResolution.Height * 0.0625f)));
            {
                viewport.BloomBlurFBO16.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO16.Material.Parameter<ShaderFloat>(0).Value = 0.0f;
                    viewport.BloomBlurFBO16.Material.Parameter<ShaderInt>(1).Value = 4;
                    viewport.BloomBlurFBO16.RenderFullscreen();
                }
                viewport.BloomBlurFBO16.Unbind(EFramebufferTarget.DrawFramebuffer);

                viewport.BloomBlurFBO16.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO16.Material.Parameter<ShaderFloat>(0).Value = 1.0f;
                    viewport.BloomBlurFBO16.Material.Parameter<ShaderInt>(1).Value = 4;
                    viewport.BloomBlurFBO16.RenderFullscreen();
                }
                viewport.BloomBlurFBO16.Unbind(EFramebufferTarget.DrawFramebuffer);
            }
            Engine.Renderer.PopRenderArea();

            Engine.Renderer.PushRenderArea(new BoundingRectangle(0, 0,
                (int)(viewport.InternalResolution.Width * 0.125f),
                (int)(viewport.InternalResolution.Height * 0.125f)));
            {
                viewport.BloomBlurFBO8.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO8.Material.Parameter<ShaderFloat>(0).Value = 0.0f;
                    viewport.BloomBlurFBO8.Material.Parameter<ShaderInt>(1).Value = 3;
                    viewport.BloomBlurFBO8.RenderFullscreen();
                }
                viewport.BloomBlurFBO8.Unbind(EFramebufferTarget.DrawFramebuffer);

                viewport.BloomBlurFBO8.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO8.Material.Parameter<ShaderFloat>(0).Value = 1.0f;
                    viewport.BloomBlurFBO8.Material.Parameter<ShaderInt>(1).Value = 3;
                    viewport.BloomBlurFBO8.RenderFullscreen();
                }
                viewport.BloomBlurFBO8.Unbind(EFramebufferTarget.DrawFramebuffer);
            }
            Engine.Renderer.PopRenderArea();

            Engine.Renderer.PushRenderArea(new BoundingRectangle(0, 0,
                (int)(viewport.InternalResolution.Width * 0.25f),
                (int)(viewport.InternalResolution.Height * 0.25f)));
            {
                viewport.BloomBlurFBO4.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO4.Material.Parameter<ShaderFloat>(0).Value = 0.0f;
                    viewport.BloomBlurFBO4.Material.Parameter<ShaderInt>(1).Value = 2;
                    viewport.BloomBlurFBO4.RenderFullscreen();
                }
                viewport.BloomBlurFBO4.Unbind(EFramebufferTarget.DrawFramebuffer);

                viewport.BloomBlurFBO4.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO4.Material.Parameter<ShaderFloat>(0).Value = 1.0f;
                    viewport.BloomBlurFBO4.Material.Parameter<ShaderInt>(1).Value = 2;
                    viewport.BloomBlurFBO4.RenderFullscreen();
                }
                viewport.BloomBlurFBO4.Unbind(EFramebufferTarget.DrawFramebuffer);
            }
            Engine.Renderer.PopRenderArea();

            Engine.Renderer.PushRenderArea(new BoundingRectangle(0, 0,
                (int)(viewport.InternalResolution.Width * 0.5f),
                (int)(viewport.InternalResolution.Height * 0.5f)));
            {
                viewport.BloomBlurFBO2.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO2.Material.Parameter<ShaderFloat>(0).Value = 0.0f;
                    viewport.BloomBlurFBO2.Material.Parameter<ShaderInt>(1).Value = 1;
                    viewport.BloomBlurFBO2.RenderFullscreen();
                }
                viewport.BloomBlurFBO2.Unbind(EFramebufferTarget.DrawFramebuffer);

                viewport.BloomBlurFBO2.Bind(EFramebufferTarget.DrawFramebuffer);
                {
                    viewport.BloomBlurFBO2.Material.Parameter<ShaderFloat>(0).Value = 1.0f;
                    viewport.BloomBlurFBO2.Material.Parameter<ShaderInt>(1).Value = 1;
                    viewport.BloomBlurFBO2.RenderFullscreen();
                }
                viewport.BloomBlurFBO2.Unbind(EFramebufferTarget.DrawFramebuffer);
            }
            Engine.Renderer.PopRenderArea();

            //Don't blur original image, barely makes a difference to result

            //Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f,
            //    viewport.InternalResolution.Width, viewport.InternalResolution.Height));
            //{
            //    viewport.BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            //    {
            //        viewport.BloomBlurFBO1.Material.Parameter<ShaderFloat>(0).Value = 0.0f;
            //        viewport.BloomBlurFBO1.Material.Parameter<ShaderInt>(1).Value = 0;
            //        viewport.BloomBlurFBO1.RenderFullscreen();
            //    }
            //    viewport.BloomBlurFBO1.Unbind(EFramebufferTarget.DrawFramebuffer);

            //    viewport.BloomBlurFBO1.Bind(EFramebufferTarget.DrawFramebuffer);
            //    {
            //        viewport.BloomBlurFBO1.Material.Parameter<ShaderFloat>(0).Value = 1.0f;
            //        viewport.BloomBlurFBO1.Material.Parameter<ShaderInt>(1).Value = 0;
            //        viewport.BloomBlurFBO1.RenderFullscreen();
            //    }
            //    viewport.BloomBlurFBO1.Unbind(EFramebufferTarget.DrawFramebuffer);
            //}
            //Engine.Renderer.PopRenderArea();
        }
    }
}
