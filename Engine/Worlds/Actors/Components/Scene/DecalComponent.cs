using System;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models;
using TheraEngine.Worlds.Actors.Components.Scene.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Worlds.Actors.Components.Scene
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
