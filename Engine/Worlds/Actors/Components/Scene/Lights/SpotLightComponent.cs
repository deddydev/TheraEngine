using System;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Worlds.Actors
{
    public class SpotLightComponent : LightComponent, I3DRenderable
    {
        private RenderInfo3D _renderInfo = new RenderInfo3D(ERenderPassType3D.OpaqueForward, null, false);
        public RenderInfo3D RenderInfo => _renderInfo;
        [Browsable(false)]
        public Shape CullingVolume => _cullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        private float _cutoff, _exponent, _distance;
        private Vec3 _direction;

        [Category("Spot Light Component")]
        public float Distance
        {
            get => _distance;
            set => _distance = value;
        }
        [Category("Spot Light Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }
        [Category("Spot Light Component")]
        public float Exponent
        {
            get => _exponent;
            set => _exponent = value;
        }
        [Category("Spot Light Component")]
        public float CutoffAngleDegrees
        {
            get => CustomMath.RadToDeg(_cutoff);
            set => _cutoff = CustomMath.DegToRad(value);
        }

        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Vec3 direction, float cutoffAngleDegrees, float exponent) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            Direction = direction;
            _distance = distance;
            _cutoff = CustomMath.DegToRad(cutoffAngleDegrees);
            _exponent = exponent;
            _cullingVolume = new ConeY(0.0f, 0.0f);
        }
        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity, float ambientIntensity,
            Rotator rotation, float cutoffAngleDegrees, float exponent)
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _distance = distance;
            _rotation.SetRotations(rotation);
            _cutoff = CustomMath.DegToRad(cutoffAngleDegrees);
            _exponent = exponent;
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        private MaterialFrameBuffer _shadowMap;
        private PerspectiveCamera _shadowCamera;
        private int _shadowWidth, _shadowHeight;
        private Matrix4 _worldToLightSpaceProjMatrix;
        internal protected ConeY _cullingVolume;

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
        public void SetShadowMapResolution(int width, int height)
        {
            _shadowWidth = width;
            _shadowHeight = height;
            if (_shadowMap == null)
                _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(width, height));
            else
                _shadowMap.ResizeTextures(width, height);

            if (_shadowCamera == null)
            {
                _shadowCamera = new PerspectiveCamera()
                {
                    NearZ = 0.0f,
                    FarZ = _distance,
                };
                _shadowCamera.Resize(1.0f, 1.0f);
                _shadowCamera.LocalRotation.SyncFrom(_rotation);
                _shadowCamera.ProjectionChanged += UpdateMatrix;
                _shadowCamera.TransformChanged += UpdateMatrix;
                UpdateMatrix();
            }
            else
                _shadowCamera.Resize(width, height);
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
        private static Material GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Int24)
        {
            //These are listed in order of appearance in the shader
            TextureReference[] refs = new TextureReference[]
            {
                new TextureReference("Depth", width, height,
                    GetFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };
            Shader shader = new Shader(ShaderMode.Fragment, ShaderHelpers.Frag_DepthOutput);
            Material m = new Material("SpotLightShadowMat", new ShaderVar[0], refs, shader)
            {
                Requirements = Material.UniformRequirements.None,
                //CullMode = Culling.Front,
            };
            return m;
        }
        public override void RenderShadowMap(SceneProcessor scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;

            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowWidth, _shadowHeight, 0.0f, 0.0f));

            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);

            scene.PreRender(_shadowCamera);
            scene.Render(ERenderPassType3D.OpaqueDeferredLit);
            scene.Render(ERenderPassType3D.OpaqueForward);
            scene.Render(ERenderPassType3D.TransparentForward);
            scene.Render(ERenderPassType3D.OnTopForward);
            scene.PostRender();

            Engine.Renderer.PopRenderArea();
            _shadowMap.Unbind(EFramebufferTarget.Framebuffer);

            Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }

        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
