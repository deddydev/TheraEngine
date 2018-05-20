﻿using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Lights
{
    [FileDef("Spot Light Component")]
    public class SpotLightComponent : LightComponent, I3DRenderable
    {
        private float _outerCutoff, _innerCutoff, _exponent, _brightness, _distance;
        private Vec3 _direction;
        private MaterialFrameBuffer _shadowMap;
        private PerspectiveCamera _shadowCamera;
        private IVec2 _shadowDims;
        private ConeZ _innerCone, _outerCone;

        [Category("Spotlight Component")]
        public Vec3 Position
        {
            get => WorldMatrix.Translation;
            set => WorldMatrix = value.AsTranslationMatrix();
        }

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

                _shadowCamera.FarZ = _distance;

                RecalcLocalTransform();
            }
        }
        [TSerialize]
        [Category("Spotlight Component")]
        public Vec3 Direction
        {
            get => _direction;
            set
            {
                _direction = value.Normalized();
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
            set => SetCutoffs(InnerCutoffAngleDegrees, value, true);
        }
        [Category("Spotlight Component")]
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
            _outerCone.Radius = TMath.Tanf(radOuter) * _distance;

            float radInner = TMath.DegToRad(innerDegrees);
            _innerCutoff = TMath.Cosf(radInner);
            _innerCone.Radius = TMath.Tanf(radInner) * _distance;
            
            _shadowCamera.VerticalFieldOfView = Math.Max(outerDegrees, innerDegrees) * 2.0f;

            RecalcLocalTransform();
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public ConeZ OuterCone => _outerCone;
        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public ConeZ InnerCone => _innerCone;
        [Browsable(true)]
        [ReadOnly(true)]
        [Category("Spotlight Component")]
        public PerspectiveCamera ShadowCamera  => _shadowCamera;

        [Browsable(false)]
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);
        [Browsable(false)]
        public Shape CullingVolume => null; //TODO: use outer cone as culling volume
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public void Render()
        {
            Engine.Renderer.RenderPoint(Position, Color.Orange, 10.0f);
        }

        public SpotLightComponent()
            : this(100.0f, new ColorF3(0.0f, 0.0f, 0.0f), 1.0f, Vec3.Down, 60.0f, 30.0f, 1.0f, 1.0f) { }

        public SpotLightComponent(
            float distance, ColorF3 color, float diffuseIntensity,
            Vec3 direction, float outerCutoffDeg, float innerCutoffDeg, float brightness, float exponent) 
            : base(color, diffuseIntensity)
        {
            _outerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(outerCutoffDeg)) * distance, distance);
            _innerCone = new ConeZ((float)Math.Tan(TMath.DegToRad(innerCutoffDeg)) * distance, distance);

            _outerCutoff = (float)Math.Cos(TMath.DegToRad(outerCutoffDeg));
            _innerCutoff = (float)Math.Cos(TMath.DegToRad(innerCutoffDeg));
            _distance = distance;
            _brightness = brightness;
            _exponent = exponent;
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

            _outerCone.State.Rotation.SetDirection(_direction);
            _outerCone.State.Translation.Raw = translation;

            _innerCone.State.Rotation.SetDirection(_direction);
            _innerCone.State.Translation.Raw = translation;

            if (_shadowCamera != null)
            {
                _shadowCamera.LocalRotation.SetRotations(_rotation);
                _shadowCamera.LocalPoint.Raw = _translation;
            }

            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.Inverted().GetMatrix();

            Matrix4
                t = _translation.AsTranslationMatrix(),
                it = (-_translation).AsTranslationMatrix();

            Matrix4
                s = Matrix4.CreateScale(_outerCone.Radius, _outerCone.Radius, _outerCone.Height),
                iS = Matrix4.CreateScale(1.0f / _outerCone.Radius, 1.0f / _outerCone.Radius, 1.0f / _outerCone.Height);

            localTransform = t * r * s;
            inverseLocalTransform = iS * ir * it;
        }
        internal Matrix4 LightMatrix { get; private set; }
        protected override void OnWorldTransformChanged()
        {
            Vec3 translation = _direction * (_distance / 2.0f);

            Matrix4 t = translation.AsTranslationMatrix();
            Matrix4 s = Matrix4.CreateScale(_outerCone.Radius, _outerCone.Radius, _outerCone.Height);

            LightMatrix = WorldMatrix * t * s;

            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
            {
                OwningScene.Lights.Add(this);
                SetShadowMapResolution(1024, 1024);
            }

#if EDITOR
            if (!Engine.EditorState.InGameMode)
                OwningScene.Add(this);
#endif
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
                OwningScene.Lights.Remove(this);

#if EDITOR
            if (!Engine.EditorState.InGameMode)
                OwningScene.Remove(this);
#endif
        }
        public override void SetUniforms(int programBindingId)
        {
            string indexer = Uniform.SpotLightsName + ".";
            Engine.Renderer.Uniform(programBindingId, indexer + "Direction", _direction);
            Engine.Renderer.Uniform(programBindingId, indexer + "OuterCutoff", _outerCutoff);
            Engine.Renderer.Uniform(programBindingId, indexer + "InnerCutoff", _innerCutoff);
            Engine.Renderer.Uniform(programBindingId, indexer + "Position", Position);
            Engine.Renderer.Uniform(programBindingId, indexer + "Radius", _distance);
            Engine.Renderer.Uniform(programBindingId, indexer + "Brightness", _brightness);
            Engine.Renderer.Uniform(programBindingId, indexer + "Exponent", _exponent);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "WorldToLightSpaceProjMatrix", _shadowCamera.WorldToCameraProjSpaceMatrix);

            TMaterialBase.SetTextureUniform(
                _shadowMap.Material.Textures[0].GetRenderTextureGeneric(true), 4, "Texture4", programBindingId);
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
                _shadowCamera = new PerspectiveCamera(
                    1.0f, _distance, Math.Max(OuterCutoffAngleDegrees, InnerCutoffAngleDegrees) * 2.0f, 1.0f);
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
        private static TMaterial GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Int16)
        {
            TexRef2D depthTex = TexRef2D.CreateFrameBufferTexture("SpotDepth", width, height,
                GetFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float,
                EFramebufferAttachment.DepthAttachment);
            depthTex.MinFilter = ETexMinFilter.Nearest;
            depthTex.MagFilter = ETexMagFilter.Nearest;
            TexRef2D[] refs = new TexRef2D[] { depthTex };

            //This material is used for rendering to the framebuffer.
            GLSLShaderFile shader = new GLSLShaderFile(EShaderMode.Fragment, ShaderHelpers.Frag_Nothing);
            TMaterial mat = new TMaterial("SpotLightShadowMat", new ShaderVar[0], refs, shader);

            //No culling so if a light exists inside of a mesh it will shadow everything.
            mat.RenderParams.CullMode = Culling.None;

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
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowDims.X, _shadowDims.Y, 0.0f, 0.0f));
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
                {
                    OwningScene.Add(OuterCone);
                    OwningScene.Add(InnerCone);
                    //#if DEBUG
                    //                Engine.Scene.Add(ShadowCamera);
                    //#endif
                }
                else
                {
                    OwningScene.Remove(OuterCone);
                    OwningScene.Remove(InnerCone);
                    //#if DEBUG
                    //                Engine.Scene.Remove(ShadowCamera);
                    //#endif
                }
            }
            base.OnSelectedChanged(selected);
        }

        public void AddRenderables(RenderPasses passes, Camera camera)
        {

        }
#endif
    }
}
