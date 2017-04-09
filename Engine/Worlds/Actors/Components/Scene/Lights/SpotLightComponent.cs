using System;
using CustomEngine.Rendering.Textures;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Worlds.Actors
{
    public class SpotLightComponent : PointLightComponent
    {
        float _cutoff, _exponent;
        private Vec3 _direction;

        public SpotLightComponent(
            float constantAtt, float linearAtt, float quadraticAtt, 
            ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Vec3 direction, float cutoffAngleDegrees, float exponent) 
            : base(constantAtt, linearAtt, quadraticAtt, color, diffuseIntensity, ambientIntensity)
        {
            Direction = direction;
            _cutoff = CustomMath.DegToRad(cutoffAngleDegrees);
            _exponent = exponent;
        }
        public SpotLightComponent(
            float constantAtt, float linearAtt, float quadraticAtt,
            ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Rotator rotation, float cutoffAngleDegrees, float exponent)
            : base(constantAtt, linearAtt, quadraticAtt, color, diffuseIntensity, ambientIntensity)
        {
            _rotation.SetRotations(rotation);
            _cutoff = CustomMath.DegToRad(cutoffAngleDegrees);
            _exponent = exponent;
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        public Vec3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }
        public float CutoffAngleDegrees
        {
            get { return CustomMath.RadToDeg(_cutoff); }
            set { _cutoff = CustomMath.DegToRad(value); }
        }
        public float Exponent
        {
            get { return _exponent; }
            set { _exponent = value; }
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

            string indexer = Uniform.SpotLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.Uniform(indexer + "Base.Base.Color", _color);
            Engine.Renderer.Uniform(indexer + "Base.Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(indexer + "Base.Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(indexer + "Base.Position", point);
            Engine.Renderer.Uniform(indexer + "Base.Attenuation", _attenuation);
            Engine.Renderer.Uniform(indexer + "Direction", _direction);
            Engine.Renderer.Uniform(indexer + "Cutoff", _cutoff);
            Engine.Renderer.Uniform(indexer + "Exponent", _exponent);
        }
    }
}
