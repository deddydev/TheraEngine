using System;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors
{
    public class PointLightComponent : LightComponent
    {
        public float Radius
        {
            get => _radius;
            set => _radius = value;
        }

        protected float _radius = 1.0f;
        
        public PointLightComponent(float radius, ColorF3 color, float diffuseIntensity, float ambientIntensity) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            Radius = radius;
        }

        public override void OnSpawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Scene.Lights.Add(this);
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Scene.Lights.Remove(this);
        }

        public override void SetUniforms()
        {
            Vec3 point = WorldMatrix.GetPoint();

            string indexer = Uniform.PointLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.Uniform(indexer + "Base.Color", _color);
            Engine.Renderer.Uniform(indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(indexer + "Position", point);
            Engine.Renderer.Uniform(indexer + "Radius", _radius);
        }
    }
}
