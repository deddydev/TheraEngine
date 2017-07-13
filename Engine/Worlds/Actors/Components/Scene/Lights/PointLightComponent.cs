using System;
using TheraEngine.Rendering.Models.Materials;
using System.ComponentModel;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public class PointLightComponent : LightComponent
    {
        [Category("Point Light Component")]
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

        public override void SetUniforms(int programBindingId)
        {
            Vec3 point = WorldMatrix.GetPoint();

            string indexer = Uniform.PointLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Position", point);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Radius", _radius);
        }

        public override void RenderShadowMap(SceneProcessor scene)
        {

        }

        public override void BakeShadowMaps()
        {

        }
    }
}
