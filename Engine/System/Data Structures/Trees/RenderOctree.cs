using CustomEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class RenderOctree : Octree<IRenderable>
    {
        public RenderOctree(BoundingBox bounds) : base(bounds) { }
        public RenderOctree(BoundingBox bounds, List<IRenderable> items) : base(bounds, items) { }

        private class RenderNode : Node
        {
            public RenderNode(BoundingBox bounds) : base(bounds)
            {

            }
        }
    }
}
