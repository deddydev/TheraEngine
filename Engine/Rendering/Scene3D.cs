using TheraEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Particles;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering
{
    public enum ERenderPass3D
    {
        /// <summary>
        /// Use for any objects that will ALWAYS be rendered behind the scene, even if they are outside of the viewing frustum.
        /// </summary>
        Skybox,
        /// <summary>
        /// Use for any fully opaque objects that are always lit.
        /// </summary>
        OpaqueDeferredLit,
        /// <summary>
        /// Use for any opaque objects that you need special lighting for (or no lighting at all).
        /// </summary>
        OpaqueForward,
        /// <summary>
        /// Use for all objects that use alpha translucency! Material.HasTransparency will help you determine this.
        /// </summary>
        TransparentForward,
        /// <summary>
        /// Renders on top of everything that has been previously rendered.
        /// </summary>
        OnTopForward,
    }
    public class RenderPasses3D
    {
        public RenderPasses3D()
        {
            _sorter = new RenderSort();
            _passes = new List<I3DRenderable>[]
            {
                new List<I3DRenderable>(),
                new List<I3DRenderable>(),
                new List<I3DRenderable>(),
                new List<I3DRenderable>(),
                new List<I3DRenderable>(),
            };
        }

        private RenderSort _sorter;
        private List<I3DRenderable>[] _passes;
        
        public List<I3DRenderable> Skybox => _passes[0];
        public List<I3DRenderable> OpaqueDeferredLit => _passes[1];
        public List<I3DRenderable> OpaqueForward => _passes[2];
        public List<I3DRenderable> TransparentForward => _passes[3];
        public List<I3DRenderable> OnTopForward => _passes[4];

        private class RenderSort : IComparer<I3DRenderable>
        {
            public bool ShadowPass { get; set; }
            int IComparer<I3DRenderable>.Compare(I3DRenderable x, I3DRenderable y)
            {
                float xOrder = x.RenderInfo.GetRenderOrder(ShadowPass);
                float yOrder = y.RenderInfo.GetRenderOrder(ShadowPass);

                if (xOrder < yOrder)
                    return -1;

                if (xOrder > yOrder)
                    return 1;

                return 0;
            }
        }

        public void Render(ERenderPass3D pass)
        {
            var list = _passes[(int)pass];
            list.ForEach(x =>
            {
                x.Render();
                if (!_sorter.ShadowPass)
                    x.RenderInfo.LastRenderedTime = DateTime.Now;
            });
            list.Clear();
        }

        public void Add(I3DRenderable item)
        {
            List<I3DRenderable> r = _passes[(int)item.RenderInfo.RenderPass];
            r.Add(item);
        }

        public void Sort(bool shadowPass)
        {
            _sorter.ShadowPass = shadowPass;
            foreach (var list in _passes)
                list.Sort(_sorter);
        }
    }
    /// <summary>
    /// Processes all scene information that will be sent to the renderer.
    /// </summary>
    public class Scene3D : Scene
    {
        private LightManager _lightManager;
        private ParticleManager _particles;
        private RenderPasses3D _passes = new RenderPasses3D();
        
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

        public override void CollectVisibleRenderables(Frustum frustum, bool shadowPass)
        {
            //TODO: implement octree on GPU with compute shader instead of here on CPU
            //Also implement occlusion culling along with frustum culling
            RenderTree.CollectVisible(frustum, _passes, shadowPass);
            _passes.Sort(shadowPass);
        }
        public void CollectVisibleRenderables(Sphere sphere, bool shadowPass)
        {
            //TODO: implement octree on GPU with compute shader instead of here on CPU
            //Also implement occlusion culling along with frustum culling
            RenderTree.CollectVisible(sphere, _passes, shadowPass);
            _passes.Sort(shadowPass);
        }

        public void RenderForward(Camera c, Viewport v, MaterialFrameBuffer target)
        {
            AbstractRenderer.PushCurrentCamera(c);
            {
                foreach (IPreRendered p in _preRenderList)
                    p.PreRender(c);

                //Enable internal resolution
                Engine.Renderer.PushRenderArea(v.InternalResolution);
                {
                    v.PostProcessFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                    {
                        //Initial scene setup
                        Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                        Engine.Renderer.AllowDepthWrite(true);

                        //Render forward opaque objects first
                        _passes.Render(ERenderPass3D.OpaqueForward);
                        //Render forward transparent objects next
                        _passes.Render(ERenderPass3D.TransparentForward);
                        
                        //Render forward on-top objects last
                        _passes.Render(ERenderPass3D.OnTopForward);
                    }
                    v.PostProcessFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
                Engine.Renderer.PopRenderArea();

                //Render the last pass to the actual screen resolution
                Engine.Renderer.PushRenderArea(v.Region);
                {
                    Engine.Renderer.CropRenderArea(v.Region);

                    //Render final post process quad
                    v.PostProcessFBO.Render();
                }
                Engine.Renderer.PopRenderArea();
            }
            AbstractRenderer.PopCurrentCamera();
        }
        public void RenderDeferred(Camera c, Viewport v, MaterialFrameBuffer target)
        {
            AbstractRenderer.PushCurrentCamera(c);
            AbstractRenderer.PushCurrent3DScene(this);
            {
                foreach (IPreRendered p in _preRenderList)
                    p.PreRender(c);

                if (v != null)
                {
                    //Enable internal resolution
                    Engine.Renderer.PushRenderArea(v.InternalResolution);
                    {
                        //Render to deferred framebuffer.
                        v.GBufferFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                        {
                            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                            Engine.Renderer.DepthFunc(EComparison.Lequal);

                            Engine.Renderer.AllowDepthWrite(false);
                            _passes.Render(ERenderPass3D.Skybox);

                            Engine.Renderer.AllowDepthWrite(true);
                            _passes.Render(ERenderPass3D.OpaqueDeferredLit);
                        }
                        v.GBufferFBO.Unbind(EFramebufferTarget.DrawFramebuffer);

                        //Engine.Renderer.BlitFrameBuffer(
                        //v.GBufferFBO.BindingId, v.PostProcessFBO.BindingId,
                        //0, 0, v.InternalResolution.IntWidth, v.InternalResolution.IntHeight,
                        //0, 0, v.InternalResolution.IntWidth, v.InternalResolution.IntHeight,
                        //EClearBufferMask.DepthBufferBit, EBlitFramebufferFilter.Nearest);

                        //Now render to final post process framebuffer.
                        v.PostProcessFBO.Bind(EFramebufferTarget.DrawFramebuffer);
                        {
                            //No need to clear anything, 
                            //color will be fully overwritten by the previous pass, 
                            //and we need depth from the previous pass
                            //Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                            Engine.Renderer.AllowDepthWrite(false);

                            //Render the deferred pass result
                            v.GBufferFBO.Render();

                            Engine.Renderer.AllowDepthWrite(true);

                            //c.OwningComponent?.OwningWorld?.PhysicsWorld.DrawDebugWorld();
                            //RenderTree.DebugRender(c?.Frustum, true);

                            _passes.Render(ERenderPass3D.OpaqueForward);
                            //Render forward transparent objects next
                            _passes.Render(ERenderPass3D.TransparentForward);
                            
                            //Render forward on-top objects last
                            Engine.Renderer.EnableDepthTest(false);
                            _passes.Render(ERenderPass3D.OnTopForward);
                            Engine.Renderer.EnableDepthTest(true);
                        }
                        v.PostProcessFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
                    }
                    //Disable internal resolution
                    Engine.Renderer.PopRenderArea();

                    //Render the last pass to the actual screen resolution
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);
                    {
                        Engine.Renderer.PushRenderArea(v.Region);
                        {
                            Engine.Renderer.AllowDepthWrite(true);
                            Engine.Renderer.DepthFunc(EComparison.Lequal);
                            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);

                            v.PostProcessFBO.Render();

                            if (v.HUD?.Scene != null)
                            {
                                v.HUD.Scene.CollectVisibleRenderables();
                                v.HUD.Scene.DoRender(v.HUD.Camera, v, null);
                            }
                        }
                        Engine.Renderer.PopRenderArea();
                    }
                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
                else
                {
                    target?.Bind(EFramebufferTarget.DrawFramebuffer);
                    Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);

                    Engine.Renderer.AllowDepthWrite(false);
                    _passes.Render(ERenderPass3D.Skybox);
                    
                    Engine.Renderer.AllowDepthWrite(true);
                    _passes.Render(ERenderPass3D.OpaqueDeferredLit);
                    _passes.Render(ERenderPass3D.OpaqueForward);
                    _passes.Render(ERenderPass3D.TransparentForward);

                    //Render forward on-top objects last
                    //Disable depth fail for objects on top
                    Engine.Renderer.DepthFunc(EComparison.Always);
                    _passes.Render(ERenderPass3D.OnTopForward);
                    target?.Unbind(EFramebufferTarget.DrawFramebuffer);
                }
            }
            AbstractRenderer.PopCurrent3DScene();
            AbstractRenderer.PopCurrentCamera();
        }
        public void Add(I3DBoundable obj)
        {
            if (RenderTree?.Add(obj) == true)
            {
                if (obj is I3DRenderable r && r.CullingVolume != null)
                    RegisterCullingVolume(r.CullingVolume);
                //Engine.PrintLine("Added {0} to the scene.", obj.ToString());
            }
        }
        public void Remove(I3DBoundable obj)
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
            _passes = new RenderPasses3D();
        }
    }
}
