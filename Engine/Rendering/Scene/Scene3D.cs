﻿using System;
using System.Collections.Generic;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Shapes;

namespace TheraEngine.Rendering
{
    public interface IScene3D : IScene
    {
        IOctree RenderTree { get; }
        IBLProbeGridActor IBLProbeActor { get; set; }
        IEventList<I3DRenderable> Renderables { get; }

        void CollectShadowMaps();
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
        //public ParticleManager Particles { get; }
        public IBLProbeGridActor IBLProbeActor { get; set; }
        public IEventList<I3DRenderable> Renderables { get; }
        
        public Scene3D() : this(0.5f) { }
        public Scene3D(Vec3 boundsHalfExtents) : base()
        {
            Renderables = new EventList<I3DRenderable>();
            Renderables.PostAnythingAdded += Renderables_PostAnythingAdded;
            Renderables.PostAnythingRemoved += Renderables_PostAnythingRemoved;

            RenderTree = new Octree(new BoundingBoxStruct(boundsHalfExtents, Vec3.Zero));
            
            Render = RenderDeferred;

            TMaterial voxMat = new TMaterial("VoxelizeMat",
                new ShaderVar[] { },
                new BaseTexRef[]
                {
                    new TexRef3D("VoxelScene")
                    {
                        SamplerName = "texture3D",
                        Resizable = false,
                        MagFilter = ETexMagFilter.Nearest,
                        MinFilter = ETexMinFilter.LinearMipmapLinear,
                        UWrap = ETexWrapMode.ClampToBorder,
                        VWrap = ETexWrapMode.ClampToBorder,
                        WWrap = ETexWrapMode.ClampToBorder,
                        InternalFormat = EPixelInternalFormat.Rgba8,
                        PixelFormat = EPixelFormat.Rgba,
                        PixelType = EPixelType.Float,
                    },
                },
                new GLSLScript[]
                {

                }
            );

            var rp = voxMat.RenderParams;
            rp.CullMode = ECulling.None;
            rp.DepthTest.Enabled = ERenderParamUsage.Disabled;
            rp.BlendMode.Enabled = ERenderParamUsage.Disabled;
            rp.WriteRed = rp.WriteGreen = rp.WriteBlue = rp.WriteAlpha = false;

            _voxelizationMaterial = voxMat;
        }

        private void Renderables_PostAnythingAdded(I3DRenderable item) => RenderTree.Add(item);
        private void Renderables_PostAnythingRemoved(I3DRenderable item) => RenderTree.Remove(item);

        public void CollectShadowMaps() => Lights?.CollectShadowMaps(this);
        public void RenderShadowMaps() => Lights?.RenderShadowMaps(this);
        public void Voxelize(bool clearVoxelization = true)
        {
            TMaterial voxMat = _voxelizationMaterial;

            RenderTex3D tex = voxMat.Textures[0].RenderTextureGeneric as RenderTex3D;
            if (clearVoxelization)
                tex.Clear(ColorF4.Black);

            Engine.Renderer.ClearFrameBufferBinding();

            Engine.Renderer.ColorMask(false, false, false, false);
            Engine.Renderer.PushRenderArea(tex.Width, tex.Height);
            Engine.Renderer.MeshMaterialOverride = voxMat;
            {
                // Texture.
                tex.Bind();

                //voxelTexture->Activate(material->program, "texture3D", 0);
                //Engine.Renderer.BindImageTexture(0, voxelTexture->textureID, 0, GL_TRUE, 0, GL_WRITE_ONLY, GL_RGBA8);

                //// Lighting.
                //uploadLighting(renderingScene, material->program);

                //// Render.
                //renderQueue(renderingScene.renderers, material->program, true);

                Engine.Renderer.GenerateMipmap(ETexTarget.Texture3D);
            }
            Engine.Renderer.MeshMaterialOverride = null;
            Engine.Renderer.PopRenderArea();
            Engine.Renderer.ColorMask(true, true, true, true);
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

        public override void RegenerateTree()
        {

        }
        public override void GlobalRender()
        {
            //Voxelize();
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
        public override void GlobalCollectVisible()
        {
            CollectShadowMaps();
        }
        /// <summary>
        /// Clears all items from the scene and sets the bounds.
        /// </summary>
        /// <param name="sceneBounds">The total extent of the items in the scene.</param>
        public void Clear(BoundingBoxStruct sceneBounds)
        {
            Renderables.Clear();
            RenderTree = new Octree(sceneBounds);
            Lights.Clear();
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
                            viewport.HDRSceneTexture.CalcDotLuminance();
                        
                        camera.UpdateExposure(viewport.HDRSceneTexture);

                        TMaterial postMat = camera.PostProcessMaterial;
                        if (postMat != null)
                            RenderPostProcessPass(viewport, postMat);
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

        public bool IBLCaptured { get; private set; } = false;
        private bool _capturing = false;
        public void CaptureIBL(bool force = false)
        {
            if (_capturing || (!force && IBLCaptured))
                return;

            Engine.Out(EOutputVerbosity.Normal, true, true, true, true, 0, 10, "Capturing scene IBL...");
            IBLCaptured = true;
            _capturing = true;
            IBLProbeActor?.InitAndCaptureAll(1024);
            _capturing = false;
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

        //public override IEnumerator<IRenderable> GetEnumerator() => Renderables.GetEnumerator();
    }
}
