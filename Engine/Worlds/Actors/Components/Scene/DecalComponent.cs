﻿using System;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Worlds.Actors
{
    public class DecalComponent : BoxComponent
    {
        private Texture2D _texture;
        private StaticMesh _mesh;
        
        public DecalComponent() : base() { }
        public DecalComponent(Vec3 extents) : base(extents, null) { }

        protected internal void Tick(float delta)
        {
            //var objects = Engine.Scene.RenderTree.FindAllJustOutside(BoundingBox).Where(x => x is Mesh);
            //foreach (var obj in objects)
            //{
                
            //}
        }
    }
}
