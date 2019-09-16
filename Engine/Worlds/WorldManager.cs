using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds
{
    public class WorldManager
    {
        protected IWorld _targetWorld;
        public virtual IWorld World
        {
            get => _targetWorld ?? Engine.World;
            set => _targetWorld = value;
        }

        private List<RenderContext> _contexts = new List<RenderContext>();
        public IReadOnlyList<RenderContext> AssociatedContexts => _contexts;

        private ConcurrentQueue<RenderContext> _contextAddQueue = new ConcurrentQueue<RenderContext>();
        private ConcurrentQueue<RenderContext> _contextRemoveQueue = new ConcurrentQueue<RenderContext>();
        
        public virtual void SwapBuffers()
        {
            while (_contextRemoveQueue.TryDequeue(out RenderContext ctx))
                _contexts.Remove(ctx);
            while (_contextAddQueue.TryDequeue(out RenderContext ctx))
                _contexts.Add(ctx);
        }

        public int ID { get; internal set; }

        public async void LoadWorldFromPath(string filePath)
        {
            World = await TFileObject.LoadAsync<World>(filePath);
        }
        public void UseEngineWorld()
        {
            World = null;
        }

        public void GlobalUpdate() => World?.GlobalUpdate();
        public void GlobalPreRender() => World?.GlobalPreRender();
        public void GlobalSwap() => World?.GlobalSwap();

        public void AddContext(RenderContext ctx) => _contextAddQueue.Enqueue(ctx);
        public void RemoveContext(RenderContext ctx) => _contextRemoveQueue.Enqueue(ctx);
    }
}
