using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering
{
    public class SceneProcessor
    {
        private List<IRenderable> _renderables = new List<IRenderable>();
        private SortedDictionary<uint, IRenderable> _renderCommands = new SortedDictionary<uint, IRenderable>();

        private Octree<IRenderable> _cullingTree;
        private LightManager _lightManager = new LightManager();

        public Octree<IRenderable> RenderTree => _cullingTree;
        public LightManager Lights => _lightManager;

        internal void WorldChanged()
        {
            if (Engine.World == null)
            {
                _cullingTree = null;
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

            _cullingTree = new Octree<IRenderable>(ws.Bounds, renderables);
        }
        internal void Render(Camera camera, bool deferredPass)
        {
            if (_cullingTree == null || camera == null)
                return;

            AbstractRenderer.CurrentCamera = camera;

            Frustum f = camera.GetFrustum();
            _cullingTree.Cull(f);
            _cullingTree.DebugRender();

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

            var e = _renderables.GetEnumerator();
            while (e.MoveNext())
                if (e.Current.IsRendering)
                    e.Current.Render();

            AbstractRenderer.CurrentCamera = null;
        }
        public void Add(IRenderable obj)
        {
            AddRenderable(obj, 0);
        }
        public void AddRenderable(IRenderable obj, RenderKey key)
        {
            _cullingTree?.Add(obj);
            //_commands.Add(key, obj);
            _renderables.Add(obj);
        }
        public void Remove(IRenderable obj)
        {
            _cullingTree?.Remove(obj);
            _renderables.Remove(obj);
        }
        internal void SetUniforms()
        {
            AbstractRenderer.CurrentCamera.SetUniforms();
            Lights.SetUniforms();
        }
    }
}
