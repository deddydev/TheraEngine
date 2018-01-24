using System;
using TheraEngine.Rendering.Models;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Components.Scene
{
    public class DecalComponent : BoxComponent
    {
        private RenderTex2D _texture;
        private StaticModel _mesh;
        
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
