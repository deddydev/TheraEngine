using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;
using System.Drawing;

namespace CustomEngine.Worlds.Actors.Components
{
    public class LightComponent : GenericSceneComponent
    {
        ColorF4 _color = Color.White;
        float _intensity = 1.0f;
        
        public LightComponent() : base() { }
    }
}
