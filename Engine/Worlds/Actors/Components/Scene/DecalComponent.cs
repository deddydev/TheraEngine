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
        private MeshProgram _projectionMaterial;

        public DecalComponent() : base() { }

        internal override void Tick(float delta)
        {
            var objects = Engine.Renderer.Scene.RenderTree.FindAllJustOutside(Box).Where(x => x is Mesh);
            foreach (var obj in objects)
            {
                
            }
        }
    }
}
