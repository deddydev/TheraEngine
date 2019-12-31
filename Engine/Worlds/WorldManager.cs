using System.Collections.Concurrent;
using System.Collections.Generic;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds
{
    /// <summary>
    /// Represents a global interface for connecting a world to render handlers
    /// related to a particular instance of work.
    /// For example, main editor world, model editor world, a 2D UI world, etc
    /// </summary>
    public class WorldManager : TObject
    {
        protected IWorld _targetWorld;
        public IWorld TargetWorld
        {
            get => _targetWorld;
            set
            {
                if (_targetWorld?.Manager == this)
                    _targetWorld.Manager = null;

                _targetWorld = value;

                if (_targetWorld != null)
                    _targetWorld.Manager = this;
            }
        }

        private readonly List<RenderContext> _contexts = new List<RenderContext>();
        public IReadOnlyList<RenderContext> AssociatedContexts => _contexts;

        private readonly ConcurrentQueue<RenderContext> _contextAddQueue = new ConcurrentQueue<RenderContext>();
        private readonly ConcurrentQueue<RenderContext> _contextRemoveQueue = new ConcurrentQueue<RenderContext>();
        
        public virtual void SwapBuffers()
        {
            while (_contextRemoveQueue.TryDequeue(out RenderContext ctx))
                _contexts.Remove(ctx);
            while (_contextAddQueue.TryDequeue(out RenderContext ctx))
                _contexts.Add(ctx);
        }

        public int ID { get; internal set; }

        public async void LoadWorldFromPath(string filePath) 
            => TargetWorld = await TFileObject.LoadAsync<World>(filePath);
        public void UseEngineWorld() 
            => TargetWorld = null;

        public void GlobalUpdate() => TargetWorld?.GlobalUpdate();
        public void GlobalPreRender() => TargetWorld?.GlobalPreRender();
        public void GlobalSwap() => TargetWorld?.GlobalSwap();

        public void AddContext(RenderContext ctx) => _contextAddQueue.Enqueue(ctx);
        public void RemoveContext(RenderContext ctx) => _contextRemoveQueue.Enqueue(ctx);
    }
}
