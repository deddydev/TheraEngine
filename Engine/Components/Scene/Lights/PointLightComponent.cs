﻿using System;
using TheraEngine.Rendering.Models.Materials;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Scene.Lights
{
    [FileDef("Point Light Component")]
    public class PointLightComponent : LightComponent
    {
        [Category("Point Light Component")]
        public float Radius
        {
            get => _influenceVolume.Radius;
            set
            {
                _influenceVolume.Radius = value;
                foreach (PerspectiveCamera cam in ShadowCameras)
                    cam.FarZ = value;
            }
        }
        [Category("Point Light Component")]
        public int ShadowMapResolution
        {
            get => _region.Width;
            set => SetShadowMapResolution(value);
        }
        [Category("Point Light Component")]
        public float Brightness { get; set; } = 1.0f;
        [Browsable(false)]
        [Category("Point Light Component")]
        public PerspectiveCamera[] ShadowCameras { get; }

        private Sphere _influenceVolume;

        public PointLightComponent() : this(100.0f, 1.0f, new ColorF3(1.0f, 1.0f, 1.0f), 1.0f) { }
        public PointLightComponent(float radius, float brightness, ColorF3 color, float diffuseIntensity) 
            : base(color, diffuseIntensity)
        {
            _influenceVolume = new Sphere(radius);
            Brightness = brightness;

            ShadowCameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f, -90.0f, 180.0f), //+X
                new Rotator(0.0f,  90.0f, 180.0f), //-X
                new Rotator(90.0f,  0.0f, 0.0f), //+Y
                new Rotator(-90.0f, 0.0f, 0.0f), //-Y
                new Rotator(0.0f, 180.0f, 180.0f), //+Z
                new Rotator(0.0f,   0.0f, 180.0f), //-Z
            };
            ShadowCameras.FillWith(i => new PerspectiveCamera(Vec3.Zero, rotations[i], 0.01f, radius, 90.0f, 1.0f));
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
            _influenceVolume.SetRenderTransform(WorldMatrix);
            foreach (PerspectiveCamera cam in ShadowCameras)
                cam.LocalPoint.Raw = WorldMatrix.Translation;
            LightMatrix = WorldMatrix * Matrix4.CreateScale(Radius);
            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            if (Type == ELightType.Dynamic)
            {
                OwningScene.Lights.Add(this);

                if (ShadowMap == null)
                    SetShadowMapResolution(512);
            }
#if EDITOR
            if (Engine.EditorState.InEditMode)
                OwningScene.Add(this);
#endif
        }
        public override void OnDespawned()
        {
            if (Type == ELightType.Dynamic)
                OwningScene.Lights.Remove(this);
#if EDITOR
            if (Engine.EditorState.InEditMode)
                OwningScene.Remove(this);
#endif
        }

        protected override IVolume GetShadowVolume() => _influenceVolume;
        /// <summary>
        /// This is to set uniforms in the GBuffer lighting shader or in a forward shader that requests lighting uniforms.
        /// </summary>
        public override void SetUniforms(RenderProgram program)
        {
            string indexer = Uniform.PointLightsName + ".";
            program.Uniform(indexer + "Base.Color", _color.Raw);
            program.Uniform(indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            program.Uniform(indexer + "Position", _influenceVolume.Center);
            program.Uniform(indexer + "Radius", Radius);
            program.Uniform(indexer + "Brightness", Brightness);

            var tex = ShadowMap.Material.Textures[0].GetRenderTextureGeneric(true);
            TMaterialBase.SetTextureUniform(tex, 4, "Texture4", program);
        }
        /// <summary>
        /// This is to set special uniforms each time something is rendered with the shadow depth shader.
        /// </summary>
        private void SetShadowDepthUniforms(RenderProgram program)
        {
            program.Uniform("FarPlaneDist", Radius);
            program.Uniform("LightPos", _influenceVolume.Center);
            for (int i = 0; i < ShadowCameras.Length; ++i)
                program.Uniform(string.Format("ShadowMatrices[{0}]", i), ShadowCameras[i].WorldToCameraProjSpaceMatrix);
        }
        public void SetShadowMapResolution(int resolution)
        {
            _region.Width = _region.Height = resolution;
            if (ShadowMap == null)
            {
                ShadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(resolution));
                ShadowMap.Material.SettingUniforms += SetShadowDepthUniforms;
            }
            else
                ShadowMap.ResizeTextures(resolution, resolution);
        }
        private static TMaterial GetShadowMapMaterial(int cubeExtent, EDepthPrecision precision = EDepthPrecision.Int16)
        {
            TexRefCube[] refs = new TexRefCube[]
            {
                new TexRefCube("PointDepth", cubeExtent, GetShadowMapFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    WWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };

            //This material is used for rendering to the framebuffer.
            GLSLShaderFile fragShader = Engine.LoadEngineShader("PointLightShadowDepth.fs", EShaderMode.Fragment);
            GLSLShaderFile geomShader = Engine.LoadEngineShader("PointLightShadowDepth.gs", EShaderMode.Geometry);
            TMaterial mat = new TMaterial("PointLightShadowMat", new ShaderVar[0], refs, fragShader, geomShader);

            //No culling so if a light exists inside of a mesh it will shadow everything.
            mat.RenderParams.CullMode = ECulling.None;

            return mat;
        }
        
#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (selected)
            {
                OwningScene.Add(_influenceVolume);
                //foreach (PerspectiveCamera c in ShadowCameras)
                //    Engine.Scene.Add(c);
            }
            else
            {
                OwningScene.Remove(_influenceVolume);
                //foreach (PerspectiveCamera c in ShadowCameras)
                //    Engine.Scene.Remove(c);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
