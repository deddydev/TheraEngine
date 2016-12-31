using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Linq;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Worlds.Actors.Components
{
    public class DecalComponent : BoxComponent
    {
        private Texture _texture;
        private StaticMesh _mesh;

        public DecalComponent() : base() { }
        public DecalComponent(Vec3 min, Vec3 max) : base(min, max, null) { }

        internal override void Tick(float delta)
        {
            //var objects = Engine.Renderer.Scene.RenderTree.FindAllJustOutside(BoundingBox).Where(x => x is Mesh);
            //foreach (var obj in objects)
            //{
                
            //}
        }
    }
}
