using System;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors
{
    public class DirectionalLightComponent : LightComponent
    {
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _rotation.Pitch = -90.0f;
        }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity, Rotator rotation)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _rotation.SetRotations(rotation);
        }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity, Vec3 direction) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            Direction = direction;
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
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
                Engine.Renderer.Scene.Lights.Add(this);
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
                Engine.Renderer.Scene.Lights.Remove(this);
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
