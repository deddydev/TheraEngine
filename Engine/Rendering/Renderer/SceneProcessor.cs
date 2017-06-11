using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering
{
    public class RenderPass
    {
        private Deque<IRenderable> _opaque = new Deque<IRenderable>();
        private Deque<IRenderable> _transparent = new Deque<IRenderable>();

        public void Render()
        {
            foreach (IRenderable r in _opaque)
                r.Render();
            foreach (IRenderable r in _transparent)
                r.Render();
        }
    }
    public class SceneProcessor
    {
        private RenderPass 
            _deferredPass = new RenderPass(), 
            _forwardPass = new RenderPass();

        private Octree<IRenderable> _renderTree;
        private LightManager _lightManager;

        internal Octree<IRenderable> RenderTree => _renderTree;
        internal LightManager Lights => _lightManager;

        internal void WorldChanged()
        {
            if (Engine.World == null)
            {
                _renderTree = null;
                _lightManager = null;
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
            _lightManager = new LightManager();
        }
        internal void Render(Camera camera, bool deferredPass)
        {
            if (_renderTree == null || camera == null)
                return;

            AbstractRenderer.CurrentCamera = camera;
            
            _renderTree.Cull(camera.GetFrustum(), Engine.Settings.RenderOctree);
            if (deferredPass)
                _deferredPass.Render();
            else
                _forwardPass.Render();

            foreach (IRenderable r in _renderables)
                r.Render();

            AbstractRenderer.CurrentCamera = null;
        }
        public void Add(IRenderable obj)
        {
            _renderTree?.Add(obj);
        }
        public void Remove(IRenderable obj)
        {
            _renderTree?.Remove(obj);
        }
        internal void SetUniforms()
        {
            AbstractRenderer.CurrentCamera.SetUniforms();
            Lights.SetUniforms();
        }
    }
}
