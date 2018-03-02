using System;
using TheraEngine.Rendering.Models.Materials;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;
using System.Drawing;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Scene.Lights
{
    [FileDef("Point Light Component")]
    public class PointLightComponent : LightComponent, I3DRenderable
    {
        [Category("Point Light Component")]
        public Vec3 Position
        {
            get => _cullingVolume.Center;
            set => WorldMatrix = value.AsTranslationMatrix();
        }
        [Category("Point Light Component")]
        public float Radius
        {
            get => _cullingVolume.Radius;
            set
            {
                _cullingVolume.Radius = value;
                foreach (PerspectiveCamera cam in ShadowCameras)
                    cam.FarZ = value;
            }
        }
        [Category("Point Light Component")]
        public int ShadowMapResolution
        {
            get => _shadowResolution;
            set => SetShadowMapResolution(value);
        }
        [Category("Point Light Component")]
        public float Brightness
        {
            get => _brightness;
            set => _brightness = value;
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [Category("Point Light Component")]
        public PerspectiveCamera[] ShadowCameras { get; }

        [Browsable(false)]
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass3D.OpaqueForward, null, false, false);
        [Browsable(false)]
        public Shape CullingVolume => _cullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }

        public void Render()
        {
            Engine.Renderer.RenderPoint(Position, Color.LimeGreen, 20.0f);
        }

        private Sphere _cullingVolume;
        private MaterialFrameBuffer _shadowMap;
        private int _shadowResolution;
        private float _brightness = 1.0f;

        public PointLightComponent() : this(100.0f, 1.0f, new ColorF3(1.0f, 1.0f, 1.0f), 1.0f, 0.0f) { }
        public PointLightComponent(float radius, float brightness, ColorF3 color, float diffuseIntensity, float ambientIntensity) 
            : base(color, diffuseIntensity, ambientIntensity)
        {
            _cullingVolume = new Sphere(radius);
            _brightness = brightness;

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

        protected override void OnWorldTransformChanged()
        {
            _cullingVolume.SetRenderTransform(WorldMatrix);
            foreach (PerspectiveCamera cam in ShadowCameras)
                cam.LocalPoint.Raw = WorldMatrix.Translation;
            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
            {
                OwningScene.Lights.Add(this);
                SetShadowMapResolution(1024);
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

        /// <summary>
        /// This is to set uniforms in the GBuffer lighting shader or in a forward shader that requests lighting uniforms.
        /// </summary>
        public override void SetUniforms(int programBindingId)
        {
            string indexer = Uniform.PointLightsName + "[" + _lightIndex + "].";
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.Color", _color.Raw);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.AmbientIntensity", _ambientIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            Engine.Renderer.Uniform(programBindingId, indexer + "Position", _cullingVolume.Center);
            Engine.Renderer.Uniform(programBindingId, indexer + "Radius", Radius);
            Engine.Renderer.Uniform(programBindingId, indexer + "Brightness", _brightness);
            //Engine.Renderer.Uniform(programBindingId, indexer + "FarPlaneDist", _farPlaneDist);

            _shadowMap.Material.SetTextureUniform(0, Viewport.GBufferTextureCount +
                OwningScene.Lights.DirectionalLights.Count + OwningScene.Lights.SpotLights.Count + LightIndex,
                string.Format("PointShadowMaps[{0}]", LightIndex.ToString()), programBindingId);
        }
        /// <summary>
        /// This is to set special uniforms each time something is rendered with the shadow depth shader.
        /// </summary>
        private void SetShadowDepthUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "FarPlaneDist", Radius);
            Engine.Renderer.Uniform(programBindingId, "LightPos", _cullingVolume.Center);
            for (int i = 0; i < ShadowCameras.Length; ++i)
                Engine.Renderer.Uniform(programBindingId, string.Format("ShadowMatrices[{0}]", i), ShadowCameras[i].WorldToCameraProjSpaceMatrix);
        }
        public void SetShadowMapResolution(int resolution)
        {
            _shadowResolution = resolution;
            if (_shadowMap == null)
            {
                _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(resolution));
                _shadowMap.Material.SettingUniforms += SetShadowDepthUniforms;

                _shadowMap.Compile();
            }
            else
                _shadowMap.ResizeTextures(resolution, resolution);
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
        private static TMaterial GetShadowMapMaterial(int cubeExtent, EDepthPrecision precision = EDepthPrecision.Int16)
        {
            //These are listed in order of appearance in the shader
            TexRefCube[] refs = new TexRefCube[]
            {
                new TexRefCube("PointDepth", cubeExtent,
                    GetFormat(precision), EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    WWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };
            Shader fragShader = Engine.LoadEngineShader("PointLightShadowDepth.fs", ShaderMode.Fragment);
            Shader geomShader = Engine.LoadEngineShader("PointLightShadowDepth.gs", ShaderMode.Geometry);
            TMaterial mat = new TMaterial("PointLightShadowMat", new ShaderVar[0], refs, fragShader, geomShader);
            mat.RenderParams.CullMode = Culling.None;
            return mat;
        }
        public override void RenderShadowMap(Scene3D scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;
            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowResolution, _shadowResolution, 0.0f, 0.0f));
            {
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);
                scene.CollectVisibleRenderables(_cullingVolume, true);
                scene.Render(null, null, null);
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
            {
                OwningScene.Add(_cullingVolume);
                //foreach (PerspectiveCamera c in ShadowCameras)
                //    Engine.Scene.Add(c);
            }
            else
            {
                OwningScene.Remove(_cullingVolume);
                //foreach (PerspectiveCamera c in ShadowCameras)
                //    Engine.Scene.Remove(c);
            }
            base.OnSelectedChanged(selected);
        }
#endif
    }
}
