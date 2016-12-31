using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors.Components
{
    public class DirectionalLightComponent : LightComponent
    {
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _rotation.Pitch = -90.0f;
        }
        public DirectionalLightComponent(Rotator rotation, ColorF3 color, float diffuseIntensity, float ambientIntensity)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _rotation.SetRotations(rotation);
        }
        public DirectionalLightComponent(Vec3 direction, ColorF3 color, float diffuseIntensity, float ambientIntensity) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            Direction = direction;
        }

        public override void RecalcLocalTransform()
        {
            _direction = _rotation.GetDirection();
            base.RecalcLocalTransform();
        }

        private Vec3 _direction;
        public Vec3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }

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
            string indexer = Uniform.DirectionalLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.Uniform(indexer + "Base.Color", _color);
            Engine.Renderer.Uniform(indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(indexer + "Direction", _direction);
        }
    }
}
