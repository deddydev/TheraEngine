﻿using TheraEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using TheraEngine.Particles;
using TheraEngine.Worlds.Actors.Components.Scene;
using TheraEngine.Worlds.Actors.Components.Scene.Lights;
using System.Drawing;
using TheraEngine.Files;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Use for calculating something right before *anything* in the scene is rendered.
    /// Generally used for setting up data for a collection of sub-renderables just before they are rendered separately.
    /// </summary>
    public interface IPreRenderNeeded
    {
        void PreRender();
    }
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
    public class RenderPasses
    {
        public RenderPasses()
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
        private List<LightComponent> _lights = new List<LightComponent>();
        
        public List<I3DRenderable> Skybox => _passes[0];
        public List<I3DRenderable> OpaqueDeferredLit => _passes[1];
        public List<I3DRenderable> OpaqueForward => _passes[2];
        public List<I3DRenderable> TransparentForward => _passes[3];
        public List<I3DRenderable> OnTopForward => _passes[4];
        public List<LightComponent> Lights => _lights;

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
    public class SceneProcessor
    {
        private static List<Material> _activeMaterials = new List<Material>();
        private static Queue<int> _removedIds = new Queue<int>();

        private RenderPasses _passes;
        private Octree _renderTree;
        private LightManager _lightManager;
        private ParticleManager _particles;
        private List<IPreRenderNeeded> _preRenderList = new List<IPreRenderNeeded>();
        private List<SplineComponent> _splines = new List<SplineComponent>();
        private List<Camera> _cameras;

        public Octree RenderTree => _renderTree;
        public LightManager Lights => _lightManager;
        public ParticleManager Particles => _particles;
        internal RenderPasses RenderPasses => _passes;

        private SingleFileRef<Material> _voxelizationMaterial = new Material();

        public void RenderShadowMaps() => Lights?.RenderShadowMaps(this);
        public void Voxelize()
        {
            Material m = _voxelizationMaterial.File;

            glUseProgram(material->program);
            glBindFramebuffer(GL_FRAMEBUFFER, 0);

            // Settings.
            glViewport(0, 0, voxelTextureSize, voxelTextureSize);
            glColorMask(GL_FALSE, GL_FALSE, GL_FALSE, GL_FALSE);
            glDisable(GL_CULL_FACE);
            glDisable(GL_DEPTH_TEST);
            glDisable(GL_BLEND);

            // Texture.
            voxelTexture->Activate(material->program, "texture3D", 0);
            glBindImageTexture(0, voxelTexture->textureID, 0, GL_TRUE, 0, GL_WRITE_ONLY, GL_RGBA8);

            // Lighting.
            uploadLighting(renderingScene, material->program);

            // Render.
            renderQueue(renderingScene.renderers, material->program, true);
            if (automaticallyRegenerateMipmap || regenerateMipmapQueued)
            {
                glGenerateMipmap(GL_TEXTURE_3D);
                regenerateMipmapQueued = false;
            }
            glColorMask(GL_TRUE, GL_TRUE, GL_TRUE, GL_TRUE);
        }
        
        /// <summary>
        /// Call this to only enable visibility for items visible from the given camera.
        /// </summary>
        internal void PreRender(Camera camera, bool shadowPass)
        {
            AbstractRenderer.PushCurrentCamera(camera);
            //_renderTree.Cull(camera.GetFrustum(), resetVisibility, cullOffscreen, Engine.Settings.RenderOctree);
            _renderTree.CollectVisible(camera.GetFrustum(), _passes, shadowPass);
            foreach (IPreRenderNeeded p in _preRenderList)
                p.PreRender();
        }
        internal void Render(ERenderPass3D pass)
        {
            if (_renderTree == null)
                return;
            
            _passes.Render(pass);
        }
        internal void PostRender()
        {
            AbstractRenderer.PopCurrentCamera();
        }
        public void Add(I3DBoundable obj)
        {
            _renderTree?.Add(obj);
        }
        public void Remove(I3DBoundable obj)
        {
            _renderTree?.Remove(obj);
        }
        internal void WorldChanged()
        {
            if (Engine.World == null)
            {
                _renderTree = null;
                _lightManager = null;
                _passes = null;
                return;
            }

            _renderTree = new Octree(Engine.World.Settings.File.Bounds);
            //_renderTree.ItemRenderChanged += RenderModified;
            _lightManager = new LightManager();
            _passes = new RenderPasses();
        }

        //private void RenderModified(I3DBoundable item)
        //{
        //    I3DRenderable r = item as I3DRenderable;
        //    if (item.IsRendering)
        //    {
        //        if (r.HasTransparency)
        //            _passes.TransparentForward.PushFront(r);
        //        else
        //        {
        //            if (Engine.Settings.ShadingStyle == ShadingStyle.Deferred)
        //                _passes.OpaqueDeferred.PushFront(r);
        //            else
        //                _passes.OpaqueForward.PushFront(r);
        //        }
        //    }
        //}

        internal int AddActiveMaterial(Material material)
        {
            int id = _removedIds.Count > 0 ? _removedIds.Dequeue() : _activeMaterials.Count;
            _activeMaterials.Add(material);
            return id;
        }
        internal void RemoveActiveMaterial(Material material)
        {
            _removedIds.Enqueue(material.UniqueID);
            _activeMaterials.RemoveAt(material.UniqueID);
        }

        public void AddPreRenderedObject(IPreRenderNeeded obj)
        {
            if (obj == null)
                return;
            if (!_preRenderList.Contains(obj))
                _preRenderList.Add(obj);
        }
        public void RemovePreRenderedObject(IPreRenderNeeded obj)
        {
            if (obj == null)
                return;
            if (_preRenderList.Contains(obj))
                _preRenderList.Remove(obj);
        }
        public void RegisterSpline(SplineComponent comp)
        {
            if (Engine.Settings.RenderSplines)
                Add(comp);
        }
        public void UnregisterSpline(SplineComponent comp)
        {
            Remove(comp);
        }
    }
}
