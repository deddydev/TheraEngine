using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SpotLightComponent : GenericSceneComponent
    {
        float _openAngle = 20.0f;

        public SpotLightComponent() : base() { }
    }
}
