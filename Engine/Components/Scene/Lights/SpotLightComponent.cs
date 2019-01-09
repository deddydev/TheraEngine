using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Lights
{
    [TFileDef("Spot Light Component")]
    public class SpotLightComponent : LightComponent
    {
        private float _outerCutoff, _innerCutoff, _distance;
        private Vec3 _direction;

        [Category("Spot Light Component")]
        public int ShadowMapResolutionWidth
        {
            get => _region.Width;
            set => SetShadowMapResolution(value, _region.Height);
        }
        [Category("Spot Light Component")]
        public int ShadowMapResolutionHeight
        {
            get => _region.Height;
            set => SetShadowMapResolution(_region.Width, value);
        }

        [TSerialize]
        [Category("Spot Light Component")]
        public float Distance
        {
            get => _distance;
            set
            {
                _distance = value;

                Vec3 translation = _translation + _direction * (_distance / 2.0f);

                OuterCone.Center.Raw = translation;
                OuterCone.Height = _distance;
                OuterCone.Radius = (float)Math.Tan(TMath.DegToRad(OuterCutoffAngleDegrees)) * _distance;

                InnerCone.Center.Raw = translation;
                InnerCone.Height = _distance;
                InnerCone.Radius = (float)Math.Tan(TMath.DegToRad(InnerCutoffAngleDegrees)) * _distance;

                ShadowCamera.FarZ = _distance;

                RecalcLocalTransform();
            }
        }
        [TSerialize]
        [Category("Spot Light Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.Normalized();
                _rotation.SetDirection(_direction);
            }
        }
        [TSerialize]
        [Category("Spot Light Component")]
        public float Exponent { get; set; }
        [TSerialize]
        [Category("Spot Light Component")]
        public float Brightness { get; set; }
        [Category("Spot Light Component")]
        public float OuterCutoffAngleDegrees
        {
            get => TMath.RadToDeg((float)Math.Acos(_outerCutoff));
            set => SetCutoffs(InnerCutoffAngleDegrees, value, true);
        }
        [Category("Spot Light Component")]
        public float InnerCutoffAngleDegrees
        {
            get => TMath.RadToDeg((float)Math.Acos(_innerCutoff));
            set => SetCutoffs(value, OuterCutoffAngleDegrees, false);
        }

        private void SetCutoffs(float innerDegrees, float outerDegrees, bool settingOuter)
        {
            innerDegrees = innerDegrees.Clamp(0.0f, 90.0f);
            outerDegrees = outerDegrees.Clamp(0.0f, 90.0f);

            if (outerDegrees < innerDegrees)
            {
                float bias = 0.0001f;
                if (settingOuter)
                    innerDegrees = outerDegrees - bias;
                else
                    outerDegrees = innerDegrees + bias;
            }
            
            float radOuter = TMath.DegToRad(outerDegrees);
            _outerCutoff = TMath.Cosf(radOuter);
            OuterCone.Radius = TMath.Tanf(radOuter) * _distance;

            float radInner = TMath.DegToRad(innerDegrees);
            _innerCutoff = TMath.Cosf(radInner);
            InnerCone.Radius = TMath.Tanf(radInner) * _distance;
            
            ((PerspectiveCamera)ShadowCamera).VerticalFieldOfView = Math.Max(outerDegrees, innerDegrees) * 2.0f;

            RecalcLocalTransform();
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public ConeZ OuterCone { get; }
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public ConeZ InnerCone { get; }
        
        [Browsable(false)]
        public override Shape CullingVolume => OuterCone;

        //public void Render()
        //{
        //    Engine.Renderer.RenderPoint(WorldMatrix.Translation, Color.Orange, 10.0f);
        //}

        public SpotLightComponent()
            : this(100.0f, new ColorF3(0.0f, 0.0f, 0.0f), 1.0f, Vec3.Down, 60.0f, 30.0f, 1.0f, 1.0f) { }

        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity,
            Vec3 direction, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent) 
            : base(color, diffuseIntensity)
        {
            OuterCone = new ConeZ((float)Math.Tan(TMath.DegToRad(outerCutoffDeg)) * distance, distance);
            InnerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(innerCutoffDeg)) * distance, distance);

            _outerCutoff = (float)Math.Cos(TMath.DegToRad(outerCutoffDeg));
            _innerCutoff = (float)Math.Cos(TMath.DegToRad(innerCutoffDeg));
            _distance = distance;
            Brightness = brightness;
            Exponent = exponent;
            Direction = direction;

            SetShadowMapResolution(1024, 1024);

            //_cullingVolume.State.Rotation.SyncFrom(_rotation);
            //_cullingVolume.State.Translation.SyncFrom(_translation);
        }
        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity,
            Rotator rotation, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent)
            : base(color, diffuseIntensity)
        {
            OuterCone = new ConeZ((float)Math.Tan(TMath.DegToRad(outerCutoffDeg)) * distance, distance);
            InnerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(innerCutoffDeg)) * distance, distance);

            _outerCutoff = (float)Math.Cos(TMath.DegToRad(outerCutoffDeg));
            _innerCutoff = (float)Math.Cos(TMath.DegToRad(innerCutoffDeg));
            _distance = distance;
            Brightness = brightness;
            Exponent = exponent;
            _rotation.SetRotations(rotation);

            //_cullingVolume.State.Rotation.SyncFrom(_rotation);
            //_cullingVolume.State.Translation.SyncFrom(_translation);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.Inverted().GetMatrix();

            Matrix4
                t = _translation.AsTranslationMatrix(),
                it = (-_translation).AsTranslationMatrix();
            
            localTransform = t * r;
            inverseLocalTransform = ir * it;
        }
        protected override void OnWorldTransformChanged()
        {
            _direction = _rotation.GetDirection();

            Vec3 coneOrigin = _translation + _direction * (_distance * 0.5f);

            if (OuterCone != null)
            {
                //OuterCone.UpAxis = _direction;
                //OuterCone.Transform.Translation.Raw = coneOrigin;
            }
            if (InnerCone != null)
            {
                //InnerCone.Transform.Rotation.SetDirection(_direction);
                //InnerCone.Transform.Translation.Raw = coneOrigin;
            }
            if (ShadowCamera != null)
            {
                ShadowCamera.LocalRotation.SetRotations(_rotation);
                ShadowCamera.LocalPoint.Raw = _translation;
            }

            Vec3 lightMeshOrigin = _direction * (_distance / 2.0f);
            Matrix4 t = lightMeshOrigin.AsTranslationMatrix();
            Matrix4 s = Matrix4.CreateScale(OuterCone.Radius, OuterCone.Radius, OuterCone.Height);
            LightMatrix = t * WorldMatrix * s;

            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            Scene3D s3d = OwningScene3D;
            if (s3d != null)
            {
                if (Type == ELightType.Dynamic)
                {
                    s3d.Lights.Add(this);
                    if (ShadowMap == null)
                        SetShadowMapResolution(_region.Width, _region.Height);
                }
                InnerCone.RenderInfo.LinkScene(InnerCone, s3d);
                OuterCone.RenderInfo.LinkScene(OuterCone, s3d);
            }
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Type == ELightType.Dynamic)
                OwningScene3D?.Lights?.Remove(this);
                
            InnerCone.RenderInfo.UnlinkScene();
            OuterCone.RenderInfo.UnlinkScene();
            
            base.OnDespawned();
        }
        public override void SetUniforms(RenderProgram program, string targetStructName)
        {
            targetStructName = targetStructName ?? Uniform.SpotLightsName;
            targetStructName += ".";

            program.Uniform(targetStructName + "Direction", _direction);
            program.Uniform(targetStructName + "OuterCutoff", _outerCutoff);
            program.Uniform(targetStructName + "InnerCutoff", _innerCutoff);
            program.Uniform(targetStructName + "Position", WorldMatrix.Translation);
            program.Uniform(targetStructName + "Radius", _distance);
            program.Uniform(targetStructName + "Brightness", Brightness);
            program.Uniform(targetStructName + "Exponent", Exponent);
            program.Uniform(targetStructName + "Color", _color.Raw);
            program.Uniform(targetStructName + "DiffuseIntensity", _diffuseIntensity);
            program.Uniform(targetStructName + "WorldToLightSpaceProjMatrix", ShadowCamera.WorldToCameraProjSpaceMatrix);

            var tex = ShadowMap.Material.Textures[1].RenderTextureGeneric;
            program.Sampler("ShadowMap", tex, 4);
        }
        public override void SetShadowMapResolution(int width, int height)
        {
            base.SetShadowMapResolution(width, height);
            if (ShadowCamera == null)
            {
                float cutoff = Math.Max(OuterCutoffAngleDegrees, InnerCutoffAngleDegrees);
                ShadowCamera = new PerspectiveCamera(1.0f, _distance, cutoff * 2.0f, 1.0f);
                //ShadowCamera.LocalRotation.SyncFrom(_rotation);
                //ShadowCamera.LocalPoint.SyncFrom(_translation);
            }
        }
        
        public override TMaterial GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Int16)
        {
            TexRef2D[] refs = new TexRef2D[]
            {
                new TexRef2D("SpotShadowDepth", width, height, 
                GetShadowDepthMapFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
                new TexRef2D("SpotShadowColor", width, height, 
                EPixelInternalFormat.R16f, EPixelFormat.Red, EPixelType.HalfFloat)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.ColorAttachment0,
                    SamplerName = "ShadowMap"
                },
            };

            //This material is used for rendering to the framebuffer.
            TMaterial mat = new TMaterial("SpotLightShadowMat", new ShaderVar[0], refs, 
                new GLSLScript(EGLSLType.Fragment, ShaderHelpers.Frag_DepthOutput));

            //No culling so if a light exists inside of a mesh it will shadow everything.
            mat.RenderParams.CullMode = ECulling.None;

            return mat;
        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            OuterCone.RenderInfo.Visible = selected;
            InnerCone.RenderInfo.Visible = selected;
        }
#endif
    }
}
