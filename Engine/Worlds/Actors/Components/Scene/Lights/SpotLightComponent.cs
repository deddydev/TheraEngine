using System;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors
{
    public class SpotLightComponent : PointLightComponent
    {
        float _cutoff, _exponent;
        private Vec3 _direction;
        
        public SpotLightComponent(
            float radius, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Vec3 direction, float cutoffAngleDegrees, float exponent) 
            : base(radius, color, diffuseIntensity, ambientIntensity)
        {
            Direction = direction;
            _cutoff = CustomMath.DegToRad(cutoffAngleDegrees);
            _exponent = exponent;
        }
        public SpotLightComponent(
            float radius, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Rotator rotation, float cutoffAngleDegrees, float exponent)
            : base(radius, color, diffuseIntensity, ambientIntensity)
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
            get => _direction;
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }
        public float CutoffAngleDegrees
        {
            get => CustomMath.RadToDeg(_cutoff);
            set => _cutoff = CustomMath.DegToRad(value);
        }
        public float Exponent
        {
            get => _exponent;
            set => _exponent = value;
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

            string indexer = Uniform.SpotLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Base.Color", _color.Raw);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Position", point);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Radius", _cullingVolume.Radius);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Direction", _direction);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Cutoff", _cutoff);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Exponent", _exponent);
        }

        public override void RenderShadowMap(SceneProcessor scene)
        {

        }

        public override void BakeShadowMaps()
        {

        }
    }
}
