using System;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Worlds.Actors
{
    public class DecalComponent : BoxComponent
    {
        private Texture _texture;
        private StaticMesh _mesh;
        
        public DecalComponent() : base() { }
        public DecalComponent(Vec3 extents) : base(extents, null) { }

        protected internal void Tick(float delta)
        {
            //var objects = Engine.Renderer.Scene.RenderTree.FindAllJustOutside(BoundingBox).Where(x => x is Mesh);
            //foreach (var obj in objects)
            //{
                
            //}
        }
    }
}
