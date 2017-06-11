using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering
{
    public class RenderPass
    {
        private SortedDictionary<uint, IRenderable> _opaque = new SortedDictionary<uint, IRenderable>();
        private SortedDictionary<uint, IRenderable> _transparent = new SortedDictionary<uint, IRenderable>();

        public void Render()
        {
            foreach (IRenderable r in _opaque.Values)
                r.Render();
            foreach (IRenderable r in _transparent.Values)
                r.Render();
        }
    }
    public class SceneProcessor
    {
        private RenderPass 
            _deferredPass = new RenderPass(), 
            _forwardPass = new RenderPass();

        private Octree<IRenderable> _renderTree;
        private LightManager _lightManager = new LightManager();

        internal Octree<IRenderable> RenderTree => _renderTree;
        internal LightManager Lights => _lightManager;

        internal void WorldChanged()
        {
            if (Engine.World == null)
            {
                _renderTree = null;
                return;
            }

            WorldSettings ws = Engine.World.Settings;
            List<IRenderable> renderables = new List<IRenderable>();

            //foreach (Map m in ws._defaultMaps)
            //    if (m.Settings.VisibleByDefault)
            //        foreach (Actor a in m.Settings._defaultActors)
            //            foreach (SceneComponent p in a.SceneComponentCache)
            //            {
            //                if (p is IRenderable r)
            //                    renderables.Add(r);
            //                else
            //                {
            //                    IRenderable[] r = p.Primitive.GetChildren(true);
            //                    foreach (IRenderable o in r)
            //                    {
            //                        o.OnSpawned();
                                    
            //                    }
            //                }
            //            }

            _renderTree = new Octree<IRenderable>(ws.Bounds, renderables);
        }
        internal void Render(Camera camera, bool deferredPass)
        {
            if (_renderTree == null || camera == null)
                return;

            AbstractRenderer.CurrentCamera = camera;

            Frustum f = camera.GetFrustum();
            if (camera.Moved)
            {
                _renderTree.Cull(f);
                camera.Moved = false;
            }

            if (Engine.Settings.RenderOctree)
                _renderTree.DebugRender();

            //TODO: render in a sorted order by render keys, not just in whatever order like this
            //also perform culling directly before rendering something, to avoid an extra log(n) operation
            //_cullingTree.Render();

            //if (_commandsInvalidated)
            //    RenderKey.RadixSort(ref _sortedCommands);

            //foreach (uint cmd in _sortedCommands)
            //{
            //    IRenderable r = _commands[cmd];
            //    //r.RenderNode.Cull(f);
            //    if (r.IsRendering)
            //        r.Render();
            //}

            foreach (IRenderable r in _renderables)
                r.Render();

            AbstractRenderer.CurrentCamera = null;
        }
        public void Add(IRenderable obj)
        {
            _renderTree?.Add(obj);
            _renderables.Add(obj);
        }
        public void Remove(IRenderable obj)
        {
            _renderTree?.Remove(obj);
            _renderables.Remove(obj);
        }
        internal void SetUniforms()
        {
            AbstractRenderer.CurrentCamera.SetUniforms();
            Lights.SetUniforms();
        }
    }
}
