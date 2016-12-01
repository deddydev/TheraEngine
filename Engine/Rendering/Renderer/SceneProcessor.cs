using CustomEngine.Rendering.Cameras;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class SceneProcessor
    {
        //private bool _commandsInvalidated;
        //private Dictionary<ulong, Action> _commands = new Dictionary<ulong, Action>();
        //private List<ulong> _sortedCommands = new List<ulong>();
        RenderOctree _renderTree;
        private Camera _currentCamera;

        public RenderOctree RenderTree { get { return _renderTree; } }

        public Camera CurrentCamera { get { return _currentCamera; } }

        internal void WorldChanged()
        {
            WorldSettings ws = Engine.World.Settings;
            Actor[] actors = ws._maps.
                Where(x => x.Settings.VisibleByDefault).
                SelectMany(x => x.Settings._defaultActors).
                Where(x => x.SceneComponentCache.Contains())
            _renderTree = Engine.World != null ? new RenderOctree(ws.WorldBounds) : null;
        }
        public void Render(Camera camera)
        {
            if (_renderTree == null || camera == null)
                return;

            if (_currentCamera != camera)
            {
                if (_currentCamera != null)
                    _currentCamera.IsActive = false;
                _currentCamera = camera;
                _currentCamera.IsActive = true;
            }
            
            _renderTree.Cull(camera.GetFrustum());
            _renderTree.Render();
            
            //if (_commandsInvalidated)
            //    RenderKey.RadixSort(ref _sortedCommands);
            //foreach (ulong key in _sortedCommands)
            //    _commands[key]();
        }
        public void AddRenderable(RenderableObject obj)
        {
            _renderTree.Add(obj);
        }
        public void RemoveRenderable(RenderableObject obj)
        {
            _renderTree.Remove(obj);
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
