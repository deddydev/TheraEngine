using System;
using System.Collections.Generic;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Particles;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene3D : BaseScene
    {
        private LightManager _lightManager;
        private ParticleManager _particles;
        
        //TODO: implement octree on GPU with compute shader instead of here on CPU
        //Also implement occlusion culling along with frustum culling
        public Octree RenderTree { get; private set; }
        public override int Count => RenderTree.Count;
        public LightManager Lights => _lightManager;
        public ParticleManager Particles => _particles;

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
        public override void Update(RenderPasses populatingPasses, IVolume cullingVolume, Camera camera, IUIManager hud, bool shadowPass)
        {
            RenderTree.CollectVisible(cullingVolume, populatingPasses, camera, shadowPass);
            base.Update(populatingPasses, cullingVolume, camera, hud, shadowPass);
        }
        public void RenderForward(RenderPasses renderingPasses, Camera camera, Viewport viewport, IUIManager hud, MaterialFrameBuffer target)
        {
            AbstractRenderer.PushCurrentCamera(camera);
            AbstractRenderer.PushCurrent3DScene(this);
            {
                if (viewport != null)
                {
                    //Enable internal resolution
                    Engine.Renderer.PushRenderArea(viewport.InternalResolution);
                    {
                        viewport.ForwardPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                        {
                            Engine.Renderer.Clear(EBufferClear.All);

                            Engine.Renderer.AllowDepthWrite(false);
                            renderingPasses.Render(ERenderPass.Background);
                            Engine.Renderer.AllowDepthWrite(true);

                            renderingPasses.Render(ERenderPass.OpaqueForward);
                            //c.OwningComponent?.OwningWorld?.PhysicsWorld.DrawDebugWorld();
                            //RenderTree.DebugRender(c?.Frustum, true);

                            renderingPasses.Render(ERenderPass.TransparentForward);
                            Engine.Renderer.AllowDepthWrite(false);
                            renderingPasses.Render(ERenderPass.OnTopForward);
                        }
                        viewport.ForwardPassFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

                        viewport.PingPongBloomBlurFBO.Reset();
                        viewport.PingPongBloomBlurFBO.BindCurrentTarget(EFramebufferTarget.DrawFramebuffer);
                        Engine.Renderer.AllowDepthWrite(false);
                        viewport.ForwardPassFBO.RenderFullscreen();
                        Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);
                        for (int i = 0; i < 10; ++i)
                        {
                            viewport.PingPongBloomBlurFBO.BindCurrentTarget(EFramebufferTarget.DrawFramebuffer);
                            Engine.Renderer.AllowDepthWrite(false);
                            viewport.PingPongBloomBlurFBO.RenderFullscreen();
                            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);

                            viewport.PingPongBloomBlurFBO.Switch();
                        }
                    }
                    Engine.Renderer.PopRenderArea();

                    //Render the last pass to the actual screen resolution, 
                    //or the provided target FBO
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);
                    {
                        Engine.Renderer.PushRenderArea(viewport.Region);
                        {
                            Engine.Renderer.AllowDepthWrite(false);
                            viewport.PostProcessFBO.RenderFullscreen();
                            hud?.UIScene?.Render(hud.RenderPasses, hud.Camera, viewport, null, null);
                        }
                        Engine.Renderer.PopRenderArea();
                    }
                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
                else
                {
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);
                    Engine.Renderer.Clear(EBufferClear.All);

                    Engine.Renderer.AllowDepthWrite(false);
                    renderingPasses.Render(ERenderPass.Background);

                    Engine.Renderer.AllowDepthWrite(true);
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
            AbstractRenderer.PopCurrentCamera();
        }
        public void RenderDeferred(RenderPasses renderingPasses, Camera camera, Viewport viewport, IUIManager hud, MaterialFrameBuffer target)
        {
            AbstractRenderer.PushCurrentCamera(camera);
            AbstractRenderer.PushCurrent3DScene(this);
            {
                if (viewport != null)
                {
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

                        //Now render to final post process framebuffer.
                        viewport.ForwardPassFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                        {
                            //Render the deferred pass result
                            viewport.GBufferFBO.RenderFullscreen();

                            Engine.Renderer.EnableDepthTest(true);
                            //c.OwningComponent?.OwningWorld?.PhysicsWorld.DrawDebugWorld();
                            //RenderTree.DebugRender(c?.Frustum, true);

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
                        
                        viewport.PingPongBloomBlurFBO.Reset();
                        viewport.PingPongBloomBlurFBO.BindCurrentTarget(EFramebufferTarget.DrawFramebuffer);
                        viewport.ForwardPassFBO.RenderFullscreen();
                        Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);
                        for (int i = 0; i < 10; ++i)
                        {
                            viewport.PingPongBloomBlurFBO.BindCurrentTarget(EFramebufferTarget.DrawFramebuffer);
                            viewport.PingPongBloomBlurFBO.RenderFullscreen();
                            Engine.Renderer.BindFrameBuffer(EFramebufferTarget.DrawFramebuffer, 0);

                            viewport.PingPongBloomBlurFBO.Switch();
                        }

                        //TODO: Apply camera post process material pass here
                        TMaterial post = camera?.PostProcessRef?.File?.PostProcessMaterial?.File;
                        if (post != null)
                        {

                        }
                    }
                    Engine.Renderer.PopRenderArea();

                    //Render the last pass to the actual screen resolution, 
                    //or the provided target FBO
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);
                    {
                        Engine.Renderer.PushRenderArea(viewport.Region);
                        {
                            viewport.PostProcessFBO.RenderFullscreen();
                            hud?.UIScene?.Render(hud.RenderPasses, hud.Camera, viewport, null, null);
                            Engine.Renderer.EnableDepthTest(true);
                        }
                        Engine.Renderer.PopRenderArea();
                    }
                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
                else
                {
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);
                    Engine.Renderer.StencilMask(~0);
                    Engine.Renderer.ClearStencil(0);
                    Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth | EBufferClear.Stencil);

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
            AbstractRenderer.PopCurrentCamera();
        }
        public void Add(I3DRenderable obj)
        {
            if (RenderTree?.Add(obj) == true)
            {
                if (obj is I3DRenderable r && r.CullingVolume != null)
                    RegisterCullingVolume(r.CullingVolume);
                //Engine.PrintLine("Added {0} to the scene.", obj.ToString());
            }
        }
        public void Remove(I3DRenderable obj)
        {
            if (RenderTree?.Remove(obj) == true)
            {
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
            _lightManager = new LightManager();
        }
    }
}
