using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;
using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering
{
    public class RenderPass
    {
        private Deque<IRenderable> _opaque = new Deque<IRenderable>();
        private Deque<IRenderable> _transparent = new Deque<IRenderable>();

        public Deque<IRenderable> Opaque => _opaque;
        public Deque<IRenderable> Transparent => _transparent;

        public void Render()
        {
            foreach (IRenderable r in _opaque)
                r.Render();
            foreach (IRenderable r in _transparent)
                r.Render();

            _opaque.Clear();
            _transparent.Clear();
        }
    }
    public class SceneProcessor
    {
        private RenderPass 
            _deferredPass = new RenderPass(), 
            _forwardPass = new RenderPass();

        private RenderOctree _renderTree;
        private LightManager _lightManager;

        internal RenderOctree RenderTree => _renderTree;
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

            _renderTree = new RenderOctree(ws.Bounds, renderables);
            _lightManager = new LightManager();
        }
        internal void Render(Camera camera, bool deferredPass)
        {
            if (_renderTree == null || camera == null)
                return;

            AbstractRenderer.CurrentCamera = camera;

            RenderPass pass = deferredPass ? _deferredPass : _forwardPass;
            _renderTree.Cull(camera.GetFrustum(), Engine.Settings.RenderOctree, pass.Opaque, pass.Transparent);
            pass.Render();
            
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
