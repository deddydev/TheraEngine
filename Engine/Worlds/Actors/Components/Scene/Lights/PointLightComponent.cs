using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PointLightComponent : PositionComponent
    {
        float _radius;

        public PointLightComponent() : base() { }
    }
}
