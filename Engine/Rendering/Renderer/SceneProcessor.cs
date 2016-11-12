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
        Octree<IRenderable> _renderTree = new Octree<IRenderable>();

        public void Render(Camera camera)
        {
            CullFrustrum(camera.GetFrustrum());

            

            //if (_commandsInvalidated)
            //    RenderKey.RadixSort(ref _sortedCommands);
            //foreach (ulong key in _sortedCommands)
            //    _commands[key]();
        }
        private void CullFrustrum(Frustrum frustrum)
        {

        }
        public void AddRenderable(IRenderable obj)
        {
            _renderTree.Add(obj);
        }
        public void RemoveRenderable(IRenderable obj) { _renderTree.Remove(obj); }
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
