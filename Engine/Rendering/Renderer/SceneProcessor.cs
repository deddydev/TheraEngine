using CustomEngine.Rendering.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class SceneProcessor
    {
        private bool _commandsInvalidated;
        private Dictionary<ulong, Action> _commands = new Dictionary<ulong, Action>();
        private List<ulong> _sortedCommands = new List<ulong>();
        RenderOctree _renderTree;
        private Camera _currentCamera;

        public Camera CurrentCamera { get { return _currentCamera; } }

        internal void WorldChanged()
        {
            //TODO: populate with default actors right now
            _renderTree = Engine.World != null ? new RenderOctree(Engine.World.Settings.WorldBounds) : null;
        }
        public void Render(Camera camera)
        {
            if (_renderTree == null || camera == null)
                return;

            _currentCamera = camera;
            _renderTree.Cull(camera.GetFrustrum());
            _renderTree.Render();
            _currentCamera = null;
            
            //if (_commandsInvalidated)
            //    RenderKey.RadixSort(ref _sortedCommands);
            //foreach (ulong key in _sortedCommands)
            //    _commands[key]();
        }
        public void AddRenderable(IRenderable obj)
        {
            _renderTree.Add(obj);
            ++obj.InstanceCount;
        }
        public void RemoveRenderable(IRenderable obj)
        {
            if (obj.InstanceCount > 0)
            {
                _renderTree.Remove(obj);
                --obj.InstanceCount;
            }
        }
        public void QueueCommand(RenderKey key, Action method)
        {
            _commandsInvalidated = true;
            _sortedCommands.Add(key);
            _commands.Add(key, method);
        }
        public void UnqueueCommand(RenderKey key)
        {
            _commandsInvalidated = true;
            _sortedCommands.Remove(key);
            _commands.Remove(key);
        }

    }
}
