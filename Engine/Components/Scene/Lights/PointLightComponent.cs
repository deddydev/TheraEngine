using System;
using TheraEngine.Rendering.Models.Materials;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Maths.Transforms;
using System.IO;

namespace TheraEngine.Worlds.Actors.Components.Scene.Lights
{
    [FileDef("Point Light Component")]
    public class PointLightComponent : LightComponent
    {
        [Category("Point Light Component")]
        public float Radius
        {
            get => _cullingVolume.Radius;
            set => _cullingVolume.Radius = value;
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
        public RenderInfo3D RenderInfo => _cullingVolume.RenderInfo;
        [Browsable(false)]
        public Shape CullingVolume => _cullingVolume.CullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode
        {
            get => _cullingVolume.OctreeNode;
            set => _cullingVolume.OctreeNode = value;
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

            ShadowCameras = new PerspectiveCamera[6];
            Rotator[] rotations = new Rotator[]
            {
                new Rotator(0.0f, -90.0f, 0.0f), //right
                new Rotator(0.0f,  90.0f, 0.0f), //left
                new Rotator(0.0f,   0.0f, 0.0f), //forward
                new Rotator(0.0f, 180.0f, 0.0f), //backward
                new Rotator(90.0f,  0.0f, 0.0f), //up
                new Rotator(-90.0f, 0.0f, 0.0f), //down
            };
            ShadowCameras.FillWith(i => new PerspectiveCamera(Vec3.Zero, rotations[i], 0.01f, 10000.0f, 90.0f, 1.0f));

            SetShadowMapResolution(1024);
        }

        protected override void OnWorldTransformChanged()
        {
            _cullingVolume.SetRenderTransform(WorldMatrix);
            foreach (PerspectiveCamera cam in ShadowCameras)
                cam.LocalPoint.Raw = WorldMatrix.GetPoint();
            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
                Engine.Scene.Lights.Add(this);
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
                Engine.Scene.Lights.Remove(this);
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

            _shadowMap.Material.SetTextureUniform(0, Viewport.GBufferTextureCount +
                Engine.Scene.Lights.DirectionalLights.Count + Engine.Scene.Lights.SpotLights.Count + LightIndex,
                indexer + "ShadowMap", programBindingId);
        }
        /// <summary>
        /// This is to set special uniforms each time something is rendered with the shadow depth shader.
        /// </summary>
        private void SetShadowDepthUniforms(int programBindingId)
        {
            Engine.Renderer.Uniform(programBindingId, "FarPlane", 10000.0f);
            Engine.Renderer.Uniform(programBindingId, "LightPos", _cullingVolume.Center);
            for (int i = 0; i < ShadowCameras.Length; ++i)
                Engine.Renderer.Uniform(programBindingId, string.Format("ShadowCameraMatrices[{0}]", i), ShadowCameras[i].WorldToCameraSpaceMatrix * ShadowCameras[i].ProjectionMatrix);
        }
        public void SetShadowMapResolution(int resolution)
        {
            _shadowResolution = resolution;
            if (_shadowMap == null)
            {
                _shadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(resolution));
                _shadowMap.Material.SettingUniforms += SetShadowDepthUniforms;
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
        private static TMaterial GetShadowMapMaterial(int cubeExtent, EDepthPrecision precision = EDepthPrecision.Flt32)
        {
            //These are listed in order of appearance in the shader
            TexRefCube[] refs = new TexRefCube[]
            {
                new TexRefCube("PointDepth", cubeExtent,
                    EPixelInternalFormat.DepthComponent, EPixelFormat.DepthComponent, EPixelType.Float)
                {
                    MinFilter = ETexMinFilter.Nearest,
                    MagFilter = ETexMagFilter.Nearest,
                    UWrap = ETexWrapMode.ClampToEdge,
                    VWrap = ETexWrapMode.ClampToEdge,
                    WWrap = ETexWrapMode.ClampToEdge,
                    FrameBufferAttachment = EFramebufferAttachment.DepthAttachment,
                },
            };
            Shader fragShader = new Shader(ShaderMode.Fragment, new TextFile(Path.Combine(Engine.Settings.ShadersFolder, "point_shadows_depth.fs")));
            Shader geomShader = new Shader(ShaderMode.Geometry, new TextFile(Path.Combine(Engine.Settings.ShadersFolder, "point_shadows_depth.gs")));
            return new TMaterial("PointLightShadowMat", new ShaderVar[0], refs, fragShader, geomShader);
        }
        public override void RenderShadowMap(Scene3D scene)
        {
            //Engine.Renderer.MaterialOverride = _shadowMap.Material;
            _shadowMap.Bind(EFramebufferTarget.Framebuffer);
            //Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowResolution, _shadowResolution, 0.0f, 0.0f));
            //{
            //    Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
            //    Engine.Renderer.AllowDepthWrite(true);

            //    scene.Render(null, null, null, true);
            //}
            //Engine.Renderer.PopRenderArea();
            _shadowMap.Unbind(EFramebufferTarget.Framebuffer);
            //Engine.Renderer.MaterialOverride = null;
        }

        public override void BakeShadowMaps()
        {

        }
        
#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (selected)
            {
                Engine.Scene.Add(_cullingVolume);
                foreach (PerspectiveCamera c in ShadowCameras)
                    Engine.Scene.Add(c);
            }
            else
            {
                Engine.Scene.Remove(_cullingVolume);
                foreach (PerspectiveCamera c in ShadowCameras)
                    Engine.Scene.Remove(c);
            }
            base.OnSelectedChanged(selected);
        }
    }
#endif
}
