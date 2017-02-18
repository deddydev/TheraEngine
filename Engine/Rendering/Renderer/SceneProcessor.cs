using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Rendering
{
    public class SceneProcessor
    {
        private bool _commandsInvalidated;
        private Dictionary<ulong, IRenderable> _commands = new Dictionary<ulong, IRenderable>();
        private List<ulong> _sortedCommands = new List<ulong>();

        private RenderOctree _cullingTree;
        private Camera _currentCamera;
        private LightManager _lightManager = new LightManager();
        
        public RenderOctree RenderTree { get { return _cullingTree; } }
        public Camera CurrentCamera { get { return _currentCamera; } }
        public LightManager Lights { get { return _lightManager; } }

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
            //            foreach (PrimitiveComponent p in a.RenderableComponentCache)
            //                if (p.Primitive != null)
            //                {
            //                    IRenderable[] r = p.Primitive.GetChildren(true);
            //                    foreach (IRenderable o in r)
            //                    {
            //                        o.OnSpawned();
            //                        renderables.Add(o);
            //                    }
            //                }

            _cullingTree = new RenderOctree(ws.WorldBounds, renderables);
        }
        public void Render(Camera camera)
        {
            if (_cullingTree == null || camera == null)
                return;

            _currentCamera = camera;
            //_cullingTree.Cull(camera.GetFrustum());

            //TODO: render in a sorted order by render keys, not just in whatever order like this
            //also perform culling directly before rendering something, to avoid an extra log(n) operation
            _cullingTree.Render();

            //if (_commandsInvalidated)
            //    RenderKey.RadixSort(ref _sortedCommands);

            //Frustum f = camera.GetFrustum();
            //foreach (ulong cmd in _sortedCommands)
            //{
            //    IRenderable r = _commands[cmd];
            //    r.RenderNode.Cull(f);
            //    if (r.IsRendering)
            //        r.Render();
            //}
        }
        public void AddRenderable(IRenderable obj)
        {
            _cullingTree?.Add(obj);
        }
        public void AddRenderable(IRenderable obj, RenderKey key)
        {
            _cullingTree?.Add(obj);
        }
        public void RemoveRenderable(IRenderable obj)
        {
            _cullingTree?.Remove(obj);
        }
        internal void SetUniforms()
        {
            CurrentCamera.SetUniforms();
            Lights.SetUniforms();
        }
        //public void QueueCommand(RenderKey key, Action method)
        //{
        //    _commandsInvalidated = true;
        //    _sortedCommands.Add(key);
        //    _commands.Add(key, method);
        //}
        //public void UnqueueCommand(RenderKey key)
        //{
        //    _commandsInvalidated = true;
        //    _sortedCommands.Remove(key);
        //    _commands.Remove(key);
        //}
    }
}
