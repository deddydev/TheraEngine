using System;
using System.Collections.Generic;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds
{
    public class WorldManager
    {
        public World World { get; set; }
        public List<RenderContext> AssociatedContexts { get; set; } = new List<RenderContext>();
        public int ID { get; internal set; }

        public void GlobalUpdate() => World?.Scene?.GlobalUpdate();
        public void GlobalPreRender() => World?.Scene?.GlobalPreRender();
        public void GlobalSwap() => World?.Scene?.GlobalSwap();

    }
}
