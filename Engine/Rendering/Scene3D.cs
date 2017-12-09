using TheraEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using TheraEngine.Rendering.Particles;
using TheraEngine.Worlds.Actors.Components.Scene;
using TheraEngine.Worlds.Actors.Components.Scene.Lights;
using System.Drawing;
using TheraEngine.Files;
using TheraEngine.Rendering.Textures;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Models;

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
            int IComparer<I3DRenderable>.Compare(I3DRenderable x, I3DRenderable y)
            {
                if (x.RenderInfo.RenderOrder < y.RenderInfo.RenderOrder)
                    return -1;
                if (x.RenderInfo.RenderOrder > y.RenderInfo.RenderOrder)
                    return 1;
                return 0;
            }
        }

        public void Render(ERenderPass3D pass)
        {
            var list = _passes[(int)pass];
            foreach (I3DRenderable r in list/*.OrderBy(x => x, _sorter)*/)
                r.Render();
            list.Clear();
        }

        public void Add(I3DRenderable item)
        {
            List<I3DRenderable> r = _passes[(int)item.RenderInfo.RenderPass];
            r.Add(item);
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

        private SingleFileRef<TMaterial> _voxelizationMaterial;

        public Scene3D()
        {
            Clear(new BoundingBox(0.5f, Vec3.Zero));
            Render = RenderDeferred;
            //Material m = new Material("VoxelizeMat",
            //    new ShaderVar[] 
            //    {

            //    },
            //    new BaseTextureReference[]
            //    {
            //        new TextureReference3D("VoxelScene"),
            //    }, 
            //    new Shader[]
            //    {

            //    }
            //);

            //m.RenderParams.CullMode = Models.Culling.None;
            //m.RenderParams.DepthTest.Enabled = false;
            //m.RenderParams.Blend.Enabled = false;


            //_voxelizationMaterial = m;
        }

        public void RenderShadowMaps() => Lights?.RenderShadowMaps(this);
        public void Voxelize()
        {
            //Material m = _voxelizationMaterial.File;
            //Texture3D tex = m.TexRefs[0].GetTextureGeneric() as Texture3D;

            //Engine.Renderer.UseProgram(material->program);
            //Engine.Renderer.BindFrameBuffer(EFramebufferTarget.Framebuffer, 0);
            
            //Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, voxelTextureSize, voxelTextureSize, 0.0f, 0.0f));
            
            //// Texture.
            //voxelTexture->Activate(material->program, "texture3D", 0);
            //Engine.Renderer.BindImageTexture(0, voxelTexture->textureID, 0, GL_TRUE, 0, GL_WRITE_ONLY, GL_RGBA8);

            //// Lighting.
            //uploadLighting(renderingScene, material->program);

            //// Render.
            //renderQueue(renderingScene.renderers, material->program, true);
            //if (automaticallyRegenerateMipmap || regenerateMipmapQueued)
            //{
            //    Engine.Renderer.GenerateMipmap(ETexTarget.Texture3D);
            //    regenerateMipmapQueued = false;
            //}
            //Engine.Renderer.ColorMask(true, true, true, true);
        }

        public void RenderForward(
            Camera camera,
            Frustum frustum,
            Viewport v,
            bool shadowPass)
        {
            if (camera == null)
                return;

            AbstractRenderer.PushCurrentCamera(camera);
            {
                RenderTree.CollectVisible(frustum, _passes, shadowPass);
                foreach (IPreRenderNeeded p in _preRenderList)
                    p.PreRender();

                //Enable internal resolution
                Engine.Renderer.PushRenderArea(v.InternalResolution);
                {
                    v._postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                    {
                        //Initial scene setup
                        Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                        Engine.Renderer.AllowDepthWrite(true);

                        //Render forward opaque objects first
                        _passes.Render(ERenderPass3D.OpaqueForward);
                        //Render forward transparent objects next
                        _passes.Render(ERenderPass3D.TransparentForward);

                        //Disable depth fail for objects on top
                        Engine.Renderer.DepthFunc(EComparison.Always);

                        //Render forward on-top objects last
                        _passes.Render(ERenderPass3D.OnTopForward);
                    }
                    v._postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);

                }
                //Disable internal resolution
                Engine.Renderer.PopRenderArea();

                //Render the last pass to the actual screen resolution
                Engine.Renderer.PushRenderArea(v.Region);
                {
                    Engine.Renderer.CropRenderArea(v.Region);

                    //Render final post process quad
                    v._postProcessFrameBuffer.Render();
                }
                Engine.Renderer.PopRenderArea();
            }
            AbstractRenderer.PopCurrentCamera();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="frustum"></param>
        /// <param name="lightingBuffer"></param>
        /// <param name="postProcessBuffer"></param>
        /// <param name="viewportRegion"></param>
        /// <param name="shadowPass"></param>
        public void RenderDeferred(
            Camera camera,
            Frustum frustum,
            Viewport v,
            bool shadowPass)
        {
            if (camera == null)
                return;

            AbstractRenderer.PushCurrentCamera(camera);
            {
                RenderTree.CollectVisible(frustum, _passes, shadowPass);
                foreach (IPreRenderNeeded p in _preRenderList)
                    p.PreRender();

                if (v != null)
                {
                    //Enable internal resolution
                    Engine.Renderer.PushRenderArea(v.InternalResolution);
                    {
                        //Render to deferred framebuffer.
                        v._deferredGBuffer.Bind(EFramebufferTarget.Framebuffer);
                        {
                            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                            Engine.Renderer.DepthFunc(EComparison.Lequal);

                            Engine.Renderer.AllowDepthWrite(false);
                            _passes.Render(ERenderPass3D.Skybox);

                            Engine.Renderer.AllowDepthWrite(true);
                            _passes.Render(ERenderPass3D.OpaqueDeferredLit);
                        }
                        v._deferredGBuffer.Unbind(EFramebufferTarget.Framebuffer);

                        //Now render to final post process framebuffer.
                        v._postProcessFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                        {
                            //No need to clear anything, 
                            //color will be fully overwritten by the previous pass, 
                            //and we need depth from the previous pass
                            //Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                            Engine.Renderer.AllowDepthWrite(false);

                            //Render the deferred pass result
                            v._deferredGBuffer.Render();

                            Engine.Renderer.AllowDepthWrite(true);
                            _passes.Render(ERenderPass3D.OpaqueForward);
                            //Render forward transparent objects next
                            _passes.Render(ERenderPass3D.TransparentForward);

                            //Disable depth fail for objects on top
                            Engine.Renderer.DepthFunc(EComparison.Always);

                            //Render forward on-top objects last
                            _passes.Render(ERenderPass3D.OnTopForward);
                        }
                        v._postProcessFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);

                        //if (v.HUD != null)
                        //{
                        //    //Render hud to hud framebuffer.
                        //    v._hudFrameBuffer.Bind(EFramebufferTarget.Framebuffer);
                        //    {
                        //        Engine.Renderer.AllowDepthWrite(true);
                        //        Engine.Renderer.DepthFunc(EComparison.Lequal);
                        //        v.HUD.Scene.Render(camera, frustum, v, false);
                        //    }
                        //    v._hudFrameBuffer.Unbind(EFramebufferTarget.Framebuffer);
                        //}
                    }
                    //Disable internal resolution
                    Engine.Renderer.PopRenderArea();

                    //Render the last pass to the actual screen resolution
                    Engine.Renderer.PushRenderArea(v.Region);
                    {
                        Engine.Renderer.CropRenderArea(v.Region);

                        //if (v.HUD != null)
                        //    v._hudFrameBuffer.Render();
                        v._postProcessFrameBuffer.Render();
                    }
                    Engine.Renderer.PopRenderArea();
                }
                else
                {
                    Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);

                    Engine.Renderer.AllowDepthWrite(false);
                    _passes.Render(ERenderPass3D.Skybox);

                    Engine.Renderer.AllowDepthWrite(true);
                    _passes.Render(ERenderPass3D.OpaqueDeferredLit);

                    //No need to clear anything, 
                    //color will be fully overwritten by the previous pass, 
                    //and we need depth from the previous pass
                    //Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                    Engine.Renderer.AllowDepthWrite(false);

                    Engine.Renderer.AllowDepthWrite(true);
                    _passes.Render(ERenderPass3D.OpaqueForward);
                    //Render forward transparent objects next
                    _passes.Render(ERenderPass3D.TransparentForward);

                    //Disable depth fail for objects on top
                    Engine.Renderer.DepthFunc(EComparison.Always);

                    //Render forward on-top objects last
                    _passes.Render(ERenderPass3D.OnTopForward);
                }
            }
        }
        public void Add(I3DBoundable obj)
        {
            RenderTree?.Add(obj);
        }
        public void Remove(I3DBoundable obj)
        {
            RenderTree?.Remove(obj);
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
