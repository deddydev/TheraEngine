using System;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering;

namespace TheraEngine.Worlds.Actors
{
    public class DirectionalLightComponent : LightComponent
    {
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _shadowCamera = new OrthographicCamera();
            _shadowCamera.LocalRotation.SyncFrom(_rotation);
            _rotation.Pitch = -90.0f;
        }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity, Rotator rotation)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _shadowCamera = new OrthographicCamera();
            _shadowCamera.LocalRotation.SyncFrom(_rotation);
            _rotation.SetRotations(rotation);
        }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, float ambientIntensity, Vec3 direction) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _shadowCamera = new OrthographicCamera();
            _shadowCamera.LocalRotation.SyncFrom(_rotation);
            Direction = direction;
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();

            _shadowCamera.TranslateRelative(0.0f, 0.0f, _worldRadius);

            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            _shadowCamera.LocalPoint = GetWorldPoint();
        }

        public void SetShadowMapResolution(int width, int height)
        {
            _shadowMapTexture.Resize(width, height);
        }
        private Texture2D _shadowMapTexture;
        private FrameBuffer _shadowMap;
        private float _worldRadius;
        private OrthographicCamera _shadowCamera;

        private Vec3 _direction;
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }

        public override void OnSpawned()
        {
            if (_type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Add(this);
                _worldRadius = Engine.World.Settings.Bounds.HalfExtents.LengthFast;
                _shadowCamera.TranslateRelative(0.0f, 0.0f, _worldRadius + 1.0f);
            }
        }
        public override void OnDespawned()
        {
            if (_type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Remove(this);
            }
        }

        public override void SetUniforms(int programBindingId)
        {
            string indexer = Uniform.DirectionalLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Direction", _direction);
        }
    }
}
