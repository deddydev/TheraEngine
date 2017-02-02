using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PointLightComponent : LightComponent
    {
        public float ConstantAttenuation
        {
            get { return _attenuation.X; }
            set { _attenuation.X = value; }
        }
        public float LinearAttenuation
        {
            get { return _attenuation.Y; }
            set { _attenuation.Y = value; }
        }
        public float QuadraticAttenuation
        {
            get { return _attenuation.Z; }
            set { _attenuation.Z = value; }
        }

        protected Vec3 _attenuation = new Vec3(0.5f, 0.0f, 0.01f);
        
        public PointLightComponent(float constantAtt, float linearAtt, float quadraticAtt, ColorF3 color, float diffuseIntensity, float ambientIntensity) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            ConstantAttenuation = constantAtt;
            LinearAttenuation = linearAtt;
            QuadraticAttenuation = quadraticAtt;
        }

        public override void OnSpawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Renderer.Scene.Lights.Add(this);
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Renderer.Scene.Lights.Remove(this);
        }

        public override void SetUniforms()
        {
            Vec3 point = WorldMatrix.GetPoint();

            string indexer = Uniform.PointLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.Uniform(indexer + "Base.Color", _color);
            Engine.Renderer.Uniform(indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(indexer + "Position", point);
            Engine.Renderer.Uniform(indexer + "Attenuation", _attenuation);
        }
    }
}
