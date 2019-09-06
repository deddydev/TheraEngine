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

        public List<RenderContext> AssociatedContexts { get; set; } = new List<RenderContext>();
        public int ID { get; internal set; }

        public async void LoadWorldFromPath(string filePath)
        {
            World = await TFileObject.LoadAsync<World>(filePath);
        }
        public void UseEngineWorld()
        {
            World = null;
        }

        public void GlobalUpdate() => World?.Scene?.GlobalUpdate();
        public void GlobalPreRender() => World?.Scene?.GlobalPreRender();
        public void GlobalSwap() => World?.Scene?.GlobalSwap();

    }
}
