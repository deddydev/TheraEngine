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

        internal override void RecalcGlobalTransform()
        {
            base.RecalcGlobalTransform();
            if (_shadowCamera != null)
            {
                _shadowCamera.LocalPoint.Raw = GetWorldPoint();
                _shadowCamera.TranslateRelative(0.0f, 0.0f, _worldRadius + 1.0f);
            }
        }
        
        private MaterialFrameBuffer _shadowMap;
        private float _worldRadius;
        private OrthographicCamera _shadowCamera;

        public Matrix4 LightToWorldMatrix => _shadowCamera.CameraToWorldMatrix;
        public Matrix4 WorldToLightMatrix => _shadowCamera.WorldToCameraMatrix;

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
                _worldRadius = Engine.World.Settings.Bounds.HalfExtents.LengthFast;
                SetShadowMapResolution(1024, 1024);
                Engine.Scene.Lights.Add(this);
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
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Base.WorldToLightMatrix", WorldToLightMatrix);
            Engine.Renderer.ProgramUniform(programBindingId, indexer + "Direction", _direction);
            //_shadowMap.Material.BindTexture(0, 4 + LightIndex, indexer + "Base.ShadowMap", programBindingId);
        }
        public void SetShadowMapResolution(int width, int height)
        {
            if (_shadowMap == null)
                _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(width, height));
            else
                _shadowMap.ResizeTextures(width, height);

            if (_shadowCamera == null)
            {
                _shadowCamera = new OrthographicCamera()
                {
                    NearZ = 1.0f,
                    FarZ = _worldRadius * 2.0f + 1.0f,
                };
                _shadowCamera.LocalRotation.SyncFrom(_rotation);
            }
            else
                _shadowCamera.Resize(width, height);
        }
        private static Material GetShadowMapMaterial(int width, int height)
        {
            //These are listed in order of appearance in the shader
            TextureReference[] refs = new TextureReference[]
            {
                new TextureReference("Depth", width, height,
                    EPixelInternalFormat.DepthComponent32f, EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.Clamp,
                    VWrap = ETexWrapMode.Clamp,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };
            Shader shader = new Shader(ShaderMode.Fragment, ShaderHelpers.Frag_DepthOutput);
            return new Material("DirLightShadowMat", new ShaderVar[0], refs, shader)
            {
                Requirements = Material.UniformRequirements.None
            };
        }
        public override void RenderShadowMap(SceneProcessor scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;
            
            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            
            Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
            Engine.Renderer.AllowDepthWrite(true);
            
            scene.PreRender(_shadowCamera);
            scene.Render(RenderPass.OpaqueDeferred);
            scene.Render(RenderPass.OpaqueForward);
            scene.Render(RenderPass.TransparentForward);
            scene.PostRender();
            
            _shadowMap.Unbind(EFramebufferTarget.Framebuffer);

            Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }
    }
}
