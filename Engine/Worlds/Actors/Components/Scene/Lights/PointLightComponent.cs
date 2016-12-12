using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PointLightComponent : LightComponent
    {
        float _radius;

        public PointLightComponent() : base() { }

        public override void OnSpawned()
        {
            Engine.Renderer.Scene.LightManager.AddLight(this);
        }
        public override void OnDespawned()
        {
            Engine.Renderer.Scene.LightManager.RemoveLight(this);
        }
    }
}
