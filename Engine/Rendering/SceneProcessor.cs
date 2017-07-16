using TheraEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    /// <summary>
    /// Use for calculating something right before anything in the scene is rendered.
    /// Generally used for setting up data for a collection of sub-renderables just before they are rendered separately,
    /// </summary>
    public interface IPreRenderNeeded
    {
        void PreRender();
    }
    public enum RenderPass
    {
        OpaqueDeferred,
        OpaqueForward,
        TransparentForward
    }
    public class RenderPasses
    {
        private Deque<I3DRenderable> _opaqueDeferred = new Deque<I3DRenderable>();
        private Deque<I3DRenderable> _opaqueForward = new Deque<I3DRenderable>();
        private Deque<I3DRenderable> _transparentForward = new Deque<I3DRenderable>();

        public Deque<I3DRenderable> OpaqueDeferred => _opaqueDeferred;
        public Deque<I3DRenderable> OpaqueForward => _opaqueForward;
        public Deque<I3DRenderable> TransparentForward => _transparentForward;

        public void Render(RenderPass pass)
        {
            switch (pass)
            {
                case RenderPass.OpaqueDeferred:
                    foreach (I3DRenderable r in OpaqueDeferred)
                        r.Render();
                    OpaqueDeferred.Clear();
                    break;
                case RenderPass.OpaqueForward:
                    foreach (I3DRenderable r in OpaqueForward)
                        r.Render();
                    OpaqueForward.Clear();
                    break;
                case RenderPass.TransparentForward:
                    foreach (I3DRenderable r in TransparentForward)
                        r.Render();
                    TransparentForward.Clear();
                    break;
            }
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
        private List<IPreRenderNeeded> _preRenderList = new List<IPreRenderNeeded>();

        public Octree RenderTree => _renderTree;
        public LightManager Lights => _lightManager;
        internal RenderPasses RenderPasses => _passes;

        public void RenderShadowMaps()
            => Lights?.RenderShadowMaps(this);
        
        /// <summary>
        /// Call this to only enable visibility for items visible from the given camera.
        /// </summary>
        /// <param name="camera">The camera viewpoint.</param>
        /// <param name="resetVisibility">If true, changes all visible objects back to invisible before testing for visibility again.</param>
        /// <param name="cullOffscreen">If true, will set all offscreen items to invisible.</param>
        /// <param name="renderOctree">If true, will render the subdivisions of the octree.</param>
        internal void PreRender(Camera camera, bool resetVisibility = true, bool cullOffscreen = true, bool renderOctree = false)
        {
            AbstractRenderer.PushCurrentCamera(camera);
            //_renderTree.Cull(camera.GetFrustum(), resetVisibility, cullOffscreen, Engine.Settings.RenderOctree);
            _renderTree.CollectVisible(camera.GetFrustum(), _passes);
            foreach (IPreRenderNeeded p in _preRenderList)
                p.PreRender();
        }
        internal void Render(RenderPass pass)
        {
            if (_renderTree == null)
                return;
            
            _passes.Render(pass);
        }
        internal void PostRender()
        {
            AbstractRenderer.PopCurrentCamera();
        }
        internal void AddDebugPrimitive(I3DRenderable obj)
        {
            _passes.OpaqueForward.PushFront(obj);
        }
        public void Add(I3DRenderable obj)
        {
            _renderTree?.Add(obj);
        }
        public void Remove(I3DRenderable obj)
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

            _renderTree = new Octree(Engine.World.Settings.Bounds);
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
    }
}
