using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PointLightComponent : LightComponent
    {
        float _constantAtt = 1.0f, _linearAtt = 0.0f, _quadraticAtt = 0.1f;
        
        public PointLightComponent() : base() { }

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
            Engine.Renderer.Uniform(indexer + "Diffuse", _color);
            Engine.Renderer.Uniform(indexer + "Specular", _specular);
            Engine.Renderer.Uniform(indexer + "Position", point);
            Engine.Renderer.Uniform(indexer + "ConstantAtt", _constantAtt);
            Engine.Renderer.Uniform(indexer + "LinearAtt", _linearAtt);
            Engine.Renderer.Uniform(indexer + "QuadraticAtt", _quadraticAtt);
        }
    }
}
