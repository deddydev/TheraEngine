using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class SpotLightComponent : LightComponent
    {
        float _halfAngle = 10.0f;
        
        public SpotLightComponent() : base() { }

        public override void OnSpawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Renderer.Scene.Lights.AddLight(this);
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Renderer.Scene.Lights.RemoveLight(this);
        }

        public override void SetUniforms()
        {
            Engine.Renderer.Uniform(Uniform.PointLightsName + "", _color);

        }
    }
}
