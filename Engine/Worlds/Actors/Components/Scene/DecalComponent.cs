using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class DecalComponent : GenericSceneComponent
    {
        private Texture _texture;
        private Material _projectionMaterial;

        public DecalComponent() : base() { }
    }
}
