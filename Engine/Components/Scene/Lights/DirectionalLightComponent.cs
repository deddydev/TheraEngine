using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials.Textures;

namespace TheraEngine.Worlds.Actors.Components.Scene.Lights
{
    [FileDef("Directional Light Component")]
    public class DirectionalLightComponent : LightComponent
    {
        public DirectionalLightComponent() 
            : this(new ColorF3(1.0f, 0.0f, 0.0f), 1.0f, 0.1f) { }
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

        private float _worldRadius;
        private int _shadowWidth, _shadowHeight;
        private MaterialFrameBuffer _shadowMap;
        private OrthographicCamera _shadowCamera;
        private Matrix4 _worldToLightSpaceProjMatrix;

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
        
        public OrthographicCamera ShadowCamera => _shadowCamera;

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
            {
                Engine.Scene.Lights.Add(this);

                _worldRadius = Engine.World.Settings.Bounds.HalfExtents.LengthFast;
                SetShadowMapResolution(4096, 4096);
                _shadowCamera.LocalPoint.Raw = GetWorldPoint();
                _shadowCamera.TranslateRelative(0.0f, 0.0f, _worldRadius + 1.0f);
            }
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
                Engine.Scene.Lights.Remove(this);
        }

        public override void SetUniforms(int programBindingId)
        {
            string indexer = Uniform.DirectionalLightsName + "[" + _lightIndex + "].";

            Engine.Renderer.Uniform(programBindingId, indexer + "Direction", _direction);

            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "WorldToLightSpaceProjMatrix", _worldToLightSpaceProjMatrix);

            _shadowMap.Material.SetTextureUniform(0, Viewport.GBufferTextureCount + LightIndex, indexer + "ShadowMap", programBindingId);
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
                _shadowCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Half, 1.0f, _worldRadius * 2.0f + 1.0f);
                _shadowCamera.Resize(_worldRadius, _worldRadius);
                _shadowCamera.LocalRotation.SyncFrom(_rotation);
                _shadowCamera.ProjectionChanged += UpdateMatrix;
                _shadowCamera.TransformChanged += UpdateMatrix;
                UpdateMatrix();
            }
            else
                _shadowCamera.Resize(_worldRadius, _worldRadius);
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
        private static TMaterial GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Flt32)
        {
            //These are listed in order of appearance in the shader
            TexRef2D[] refs = new TexRef2D[]
            {
                new TexRef2D("Depth", width, height,
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
            return new TMaterial("DirLightShadowMat", new ShaderVar[0], refs, shader);
        }
        public override void RenderShadowMap(Scene3D scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;
            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowWidth, _shadowHeight, 0.0f, 0.0f));
            {
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);

                scene.Render(_shadowCamera, _shadowCamera.Frustum, null, true);
            }
            Engine.Renderer.PopRenderArea();
            _shadowMap.Unbind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (selected)
                Engine.Scene.Add(_shadowCamera);
            else
                Engine.Scene.Remove(_shadowCamera);
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
