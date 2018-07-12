using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Lights
{
    [FileDef("Directional Light Component")]
    public class DirectionalLightComponent : LightComponent
    {
        private Vec3 _scale;
        private Vec3 _direction;

        [TSerialize]
        [Category("Transform")]
        public Vec3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                if (ShadowCamera != null)
                {
                    ShadowCamera.Resize(Scale.X, Scale.Y);
                    ShadowCamera.FarZ = Scale.Z;
                    ShadowCamera.LocalPoint.Raw = WorldPoint;
                    ShadowCamera.TranslateRelative(0.0f, 0.0f, Scale.Z * 0.5f);
                }
            }
        }

        [Category("Directional Light Component")]
        public int ShadowMapResolutionWidth
        {
            get => _region.Width;
            set => SetShadowMapResolution(value, _region.Height);
        }
        [Category("Directional Light Component")]
        public int ShadowMapResolutionHeight
        {
            get => _region.Height;
            set => SetShadowMapResolution(_region.Width, value);
        }
        [Category("Directional Light Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.Normalized();
                _rotation.SetDirection(_direction);
            }
        }

        public DirectionalLightComponent() 
            : this(new ColorF3(1.0f, 1.0f, 1.0f), 1.0f, 0.0f) { }
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity)
            : base(color, diffuseIntensity) => _rotation.Pitch = -90.0f;
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, Rotator rotation)
            : base(color, diffuseIntensity) => _rotation.SetRotations(rotation);
        public DirectionalLightComponent(ColorF3 color, float diffuseIntensity, Vec3 direction) 
            : base(color, diffuseIntensity) => Direction = direction;
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            _direction = _rotation.GetDirection();
            base.OnRecalcLocalTransform(out localTransform, out inverseLocalTransform);
        }
        protected override void OnWorldTransformChanged()
        {
            if (ShadowCamera != null)
            {
                ShadowCamera.LocalPoint.Raw = WorldPoint;
                ShadowCamera.TranslateRelative(0.0f, 0.0f, Scale.Z * 0.5f);
            }
            
            LightMatrix = WorldMatrix * Scale.AsScaleMatrix();

            base.OnWorldTransformChanged();
        }
        public override void OnSpawned()
        {
            if (Type == ELightType.Dynamic)
            {
                OwningScene.Lights.Add(this);

                if (ShadowMap == null)
                    SetShadowMapResolution(1024, 1024);

                ShadowCamera.LocalPoint.Raw = WorldPoint;
                ShadowCamera.TranslateRelative(0.0f, 0.0f, Scale.Z * 0.5f);
            }
        }
        public override void OnDespawned()
        {
            if (Type == ELightType.Dynamic)
                OwningScene.Lights.Remove(this);
        }
        public override void SetUniforms(RenderProgram program, string targetStructName)
        {
            targetStructName = targetStructName ?? Uniform.DirectionalLightsName;
            targetStructName += ".";

            program.Uniform(targetStructName + "Direction", _direction);
            program.Uniform(targetStructName + "Color", _color.Raw);
            program.Uniform(targetStructName + "DiffuseIntensity", _diffuseIntensity);
            program.Uniform(targetStructName + "WorldToLightSpaceProjMatrix", ShadowCamera.WorldToCameraProjSpaceMatrix);

            var tex = ShadowMap.Material.Textures[1].RenderTextureGeneric;
            program.Sampler("Texture4", tex, 4);
        }
        public override void SetShadowMapResolution(int width, int height)
        {
            base.SetShadowMapResolution(width, height);
            if (ShadowCamera == null)
            {
                ShadowCamera = new OrthographicCamera(Vec3.One, Vec3.Zero, Rotator.GetZero(), Vec2.Half, 0.0f, Scale.Z);
                ShadowCamera.LocalRotation.SyncFrom(_rotation);
                ShadowCamera.Resize(Scale.X, Scale.Y);
            }
        }
        public override TMaterial GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Flt32)
        {
            TexRef2D[] refs = new TexRef2D[]
            {
                new TexRef2D("DirShadowDepth", width, height, GetShadowDepthMapFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
                new TexRef2D("DirShadowColor", width, height, EPixelInternalFormat.R32f, EPixelFormat.Red, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                },
            };

            //This material is used for rendering to the framebuffer.
            TMaterial mat = new TMaterial("DirLightShadowMat", new ShaderVar[0], refs, 
                new GLSLShaderFile(EShaderMode.Fragment, ShaderHelpers.Frag_DepthOutput));

            //No culling so if a light exists inside of a mesh it will shadow everything.
            mat.RenderParams.CullMode = ECulling.None;

            return mat;
        }
#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (IsSpawned)
            {
                if (selected)
                    OwningScene.Add(ShadowCamera);
                else
                    OwningScene.Remove(ShadowCamera);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
