using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Scene.Lights
{
    [FileDef("Directional Light Component")]
    public class DirectionalLightComponent : LightComponent
    {
        [TSerialize(nameof(ShadowMapResolutionWidth))]
        private int _shadowWidth = 4096;
        [TSerialize(nameof(ShadowMapResolutionHeight))]
        private int _shadowHeight = 4096;
        [TSerialize(nameof(WorldRadius))]
        private float _worldRadius;

        private MaterialFrameBuffer _shadowMap = null;
        private OrthographicCamera _shadowCamera = null;
        private Vec3 _direction;

        [Category("Directional Light Component")]
        public float WorldRadius
        {
            get => _worldRadius;
            set
            {
                _worldRadius = value;
                if (_shadowCamera != null)
                {
                    _shadowCamera.Resize(WorldRadius, WorldRadius);
                    _shadowCamera.FarZ = WorldRadius * 2.0f + 1.0f;
                    _shadowCamera.LocalPoint.Raw = WorldPoint;
                    _shadowCamera.TranslateRelative(0.0f, 0.0f, WorldRadius + 1.0f);
                }
            }
        }

        [Category("Directional Light Component")]
        public int ShadowMapResolutionWidth
        {
            get => _shadowWidth;
            set => SetShadowMapResolution(value, _shadowHeight);
        }
        [Category("Directional Light Component")]
        public int ShadowMapResolutionHeight
        {
            get => _shadowHeight;
            set => SetShadowMapResolution(_shadowWidth, value);
        }
        [Category("Directional Light Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.NormalizedFast();
                _rotation.SetDirection(_direction);
            }
        }
        [Browsable(false)]
        public OrthographicCamera ShadowCamera => _shadowCamera;

        public MaterialFrameBuffer ShadowMap => _shadowMap;

        public DirectionalLightComponent() 
            : this(new ColorF3(1.0f, 1.0f, 1.0f), 1.0f, 0.0f) { }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity)
            : base(color, diffuseIntensity)
        {
            _rotation.Pitch = -90.0f;
        }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, Rotator rotation)
            : base(color, diffuseIntensity)
        {
            _rotation.SetRotations(rotation);
        }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, Vec3 direction) 
            : base(color, diffuseIntensity)
        {
            Direction = direction;
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }

        public override void RecalcWorldTransform()
        {
            base.RecalcWorldTransform();
            if (_shadowCamera != null)
            {
                _shadowCamera.LocalPoint.Raw = WorldPoint;
                _shadowCamera.TranslateRelative(0.0f, 0.0f, WorldRadius + 1.0f);
            }
        }

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
            {
                OwningScene.Lights.Add(this);

                if (_shadowMap == null)
                    SetShadowMapResolution(_shadowWidth, _shadowHeight);

                _shadowCamera.LocalPoint.Raw = WorldPoint;
                _shadowCamera.TranslateRelative(0.0f, 0.0f, WorldRadius + 1.0f);
            }
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
                OwningScene.Lights.Remove(this);
        }

        public override void SetUniforms(RenderProgram program)
        {
            string indexer = Uniform.DirectionalLightsName + ".";
            program.Uniform(indexer + "Direction", _direction);
            program.Uniform(indexer + "Base.Color", _color.Raw);
            program.Uniform(indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            program.Uniform(indexer + "WorldToLightSpaceProjMatrix", _shadowCamera.WorldToCameraProjSpaceMatrix);

            TMaterialBase.SetTextureUniform(
                _shadowMap.Material.Textures[0].GetRenderTextureGeneric(true), 4, "Texture4", program);
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
                _shadowCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Half, 1.0f, WorldRadius * 2.0f + 1.0f);
                _shadowCamera.Resize(WorldRadius, WorldRadius);
                _shadowCamera.LocalRotation.SyncFrom(_rotation);
            }
            //else
            //    _shadowCamera.Resize(_worldRadius, _worldRadius);
        }
        
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
            TexRef2D depthTex = TexRef2D.CreateFrameBufferTexture("Depth", width, height,
                GetFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float,
                EFramebufferAttachment.DepthAttachment);
            depthTex.MinFilter = ETexMinFilter.Nearest;
            depthTex.MagFilter = ETexMagFilter.Nearest;
            TexRef2D[] refs = new TexRef2D[] { depthTex };

            //This material is used for rendering to the framebuffer.
            GLSLShaderFile shader = new GLSLShaderFile(EShaderMode.Fragment, ShaderHelpers.Frag_Nothing);
            TMaterial mat = new TMaterial("DirLightShadowMat", new ShaderVar[0], refs, shader);

            //No culling so if a light exists inside of a mesh it will shadow everything.
            mat.RenderParams.CullMode = ECulling.None;

            return mat;
        }
        public override void UpdateShadowMap(BaseScene scene)
        {
            scene.Update(_passes, _shadowCamera.Frustum, _shadowCamera, null, true);
        }
        public override void RenderShadowMap(BaseScene scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;
            _shadowMap.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowWidth, _shadowHeight, 0.0f, 0.0f));
            {
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);
                scene.Render(_passes, _shadowCamera, null, null, null);
            }
            Engine.Renderer.PopRenderArea();
            _shadowMap.Unbind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (IsSpawned)
            {
                if (selected)
                    OwningScene.Add(_shadowCamera);
                else
                    OwningScene.Remove(_shadowCamera);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
