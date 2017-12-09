using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Worlds.Actors.Components.Scene.Lights
{
    [FileClass("cspotlight", "Spot Light Component")]
    public class SpotLightComponent : LightComponent
    {
        private float _outerCutoff, _innerCutoff, _exponent, _brightness, _distance;
        private Vec3 _direction;
        private MaterialFrameBuffer _shadowMap;
        private PerspectiveCamera _shadowCamera;
        private IVec2 _shadowDims;
        private Matrix4 _worldToLightSpaceProjMatrix;
        private ConeZ _innerCone, _outerCone;
        
        [Category("Spotlight Component")]
        public int ShadowMapResolutionWidth
        {
            get => _shadowDims.X;
            set => SetShadowMapResolution(value, _shadowDims.Y);
        }
        [Category("Spotlight Component")]
        public int ShadowMapResolutionHeight
        {
            get => _shadowDims.Y;
            set => SetShadowMapResolution(_shadowDims.X, value);
        }

        [TSerialize]
        [Category("Spotlight Component")]
        public float Distance
        {
            get => _distance;
            set
            {
                _distance = value;

                Vec3 translation = _translation + _direction * (_distance / 2.0f);

                _outerCone.State.Translation.Raw = translation;
                _outerCone.Height = _distance;
                _outerCone.Radius = (float)Math.Tan(TMath.DegToRad(OuterCutoffAngleDegrees)) * _distance;

                _innerCone.State.Translation.Raw = translation;
                _innerCone.Height = _distance;
                _innerCone.Radius = (float)Math.Tan(TMath.DegToRad(InnerCutoffAngleDegrees)) * _distance;

                _shadowCamera.FarZ = _distance - 0.1f;
            }
        }
        [TSerialize]
        [Category("Spotlight Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }
        [Category("Spotlight Component")]
        public float Exponent
        {
            get => _exponent;
            set => _exponent = value;
        }
        [Category("Spotlight Component")]
        public float Brightness
        {
            get => _brightness;
            set => _brightness = value;
        }
        [Category("Spotlight Component")]
        public float OuterCutoffAngleDegrees
        {
            get => TMath.RadToDeg((float)Math.Acos(_outerCutoff));
            set
            {
                float rad = TMath.DegToRad(value);
                _outerCutoff = (float)Math.Cos(rad);
                _outerCone.Radius = (float)Math.Tan(rad) * _distance;

                _shadowCamera.VerticalFieldOfView = Math.Max(OuterCutoffAngleDegrees, InnerCutoffAngleDegrees) * 2.0f;
            }
        }
        [Category("Spotlight Component")]
        public float InnerCutoffAngleDegrees
        {
            get => TMath.RadToDeg((float)Math.Acos(_innerCutoff));
            set
            {
                float rad = TMath.DegToRad(value);
                _innerCutoff = (float)Math.Cos(rad);
                _innerCone.Radius = (float)Math.Tan(rad) * _distance;

                _shadowCamera.VerticalFieldOfView = Math.Max(OuterCutoffAngleDegrees, InnerCutoffAngleDegrees) * 2.0f;
            }
        }

        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public ConeZ OuterCone => _outerCone;
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public ConeZ InnerCone => _innerCone;
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public PerspectiveCamera ShadowCamera  => _shadowCamera;

        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Vec3 direction, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _outerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(outerCutoffDeg)) * distance, distance);
            _innerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(innerCutoffDeg)) * distance, distance);

            _outerCutoff = (float)Math.Cos(TMath.DegToRad(outerCutoffDeg));
            _innerCutoff = (float)Math.Cos(TMath.DegToRad(innerCutoffDeg));
            _distance = distance;
            _brightness = brightness;
            _exponent = exponent;
            Direction = direction;

            //_cullingVolume.State.Rotation.SyncFrom(_rotation);
            //_cullingVolume.State.Translation.SyncFrom(_translation);
        }
        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Rotator rotation, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _outerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(outerCutoffDeg)) * distance, distance);
            _innerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(innerCutoffDeg)) * distance, distance);

            _outerCutoff = (float)Math.Cos(TMath.DegToRad(outerCutoffDeg));
            _innerCutoff = (float)Math.Cos(TMath.DegToRad(innerCutoffDeg));
            _distance = distance;
            _brightness = brightness;
            _exponent = exponent;
            _rotation.SetRotations(rotation);

            //_cullingVolume.State.Rotation.SyncFrom(_rotation);
            //_cullingVolume.State.Translation.SyncFrom(_translation);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();

            Vec3 translation = _translation + _direction * (_distance / 2.0f);

            _outerCone.State.Rotation.SetDirection(-_direction);
            _outerCone.State.Translation.Raw = translation;

            _innerCone.State.Rotation.SetDirection(-_direction);
            _innerCone.State.Translation.Raw = translation;

            if (_shadowCamera != null)
            {
                _shadowCamera.LocalRotation.SetRotations(_rotation);
                _shadowCamera.LocalPoint.Raw = _translation;
            }

            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Add(this);
                
                SetShadowMapResolution(3840, 2160);

                if (Engine.Settings.RenderCameraFrustums)
                    Engine.Scene.Add(_shadowCamera);
            }
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Remove(this);
                if (Engine.Settings.RenderCameraFrustums)
                    Engine.Scene.Remove(_shadowCamera);
            }
        }
        public override void SetUniforms(int programBindingId)
        {
            Vec3 point = WorldMatrix.GetPoint();

            string indexer = Uniform.SpotLightsName + "[" + _lightIndex + "].";

            Engine.Renderer.Uniform(programBindingId, indexer + "Direction", _direction);
            Engine.Renderer.Uniform(programBindingId, indexer + "OuterCutoff", _outerCutoff);
            Engine.Renderer.Uniform(programBindingId, indexer + "InnerCutoff", _innerCutoff);
            Engine.Renderer.Uniform(programBindingId, indexer + "Exponent", _exponent);

            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Position", point);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Radius", _distance);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Brightness", _brightness);

            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.Color", _color.Raw);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Base.WorldToLightSpaceProjMatrix", _worldToLightSpaceProjMatrix);

            _shadowMap.Material.SetTextureUniform(0, Viewport.GBufferTextureCount + Engine.Scene.Lights.DirectionalLights.Count + LightIndex, indexer + "Base.Base.ShadowMap", programBindingId);
        }

        public void SetShadowMapResolution(int width, int height)
            => SetShadowMapResolution(new IVec2(width, height));
        public void SetShadowMapResolution(IVec2 dims)
        {
            _shadowDims = dims;
            if (_shadowMap == null)
                _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(dims.X, dims.Y));
            else
                _shadowMap.ResizeTextures(dims.X, dims.Y);

            if (_shadowCamera == null)
            {
                _shadowCamera = new PerspectiveCamera(0.1f, _distance - 0.1f, Math.Max(OuterCutoffAngleDegrees, InnerCutoffAngleDegrees) * 2.0f, 1.0f);
                _shadowCamera.ProjectionChanged += UpdateMatrix;
                _shadowCamera.TransformChanged += UpdateMatrix;
                UpdateMatrix();
            }
        }
        private void UpdateMatrix()
            => _worldToLightSpaceProjMatrix = _shadowCamera.ProjectionMatrix * _shadowCamera.WorldToCameraSpaceMatrix;

        private static EPixelInternalFormat GetFormat(EDepthPrecision precision)
        {
            switch (precision)
            {
                case EDepthPrecision.Int16: return EPixelInternalFormat.DepthComponent16;
                case EDepthPrecision.Int24: return EPixelInternalFormat.DepthComponent24;
                case EDepthPrecision.Int32: return EPixelInternalFormat.DepthComponent32;
            }
            return EPixelInternalFormat.DepthComponent32f;
        }
        private static TMaterial GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Int24)
        {
            //These are listed in order of appearance in the shader
            TextureReference2D[] refs = new TextureReference2D[]
            {
                new TextureReference2D("SpotDepth", width, height,
                    GetFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };
            Shader shader = new Shader(ShaderMode.Fragment, ShaderHelpers.Frag_Nothing);
            return new TMaterial("SpotLightShadowMat", new ShaderVar[0], refs, shader);
        }
        public override void RenderShadowMap(Scene3D scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;

            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowDims.X, _shadowDims.Y, 0.0f, 0.0f));

            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);

            scene.Render(_shadowCamera, _shadowCamera.Frustum, null, true);

            Engine.Renderer.PopRenderArea();
            _shadowMap.Unbind(EFramebufferTarget.Framebuffer);

            Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }
    }
}
