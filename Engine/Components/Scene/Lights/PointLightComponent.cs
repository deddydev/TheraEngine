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
        public MaterialFrameBuffer ShadowMap => _shadowMap;
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
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(ERenderPass.OpaqueForward, false, false);
        [Browsable(false)]
        public Shape CullingVolume => _cullingVolume;
        [Browsable(false)]
        public IOctreeNode OctreeNode { get; set; }
        
        private Sphere _cullingVolume;
        private MaterialFrameBuffer _shadowMap;
        private int _shadowResolution;
        private float _brightness = 1.0f;

        public PointLightComponent() : this(100.0f, 1.0f, new ColorF3(1.0f, 1.0f, 1.0f), 1.0f) { }
        public PointLightComponent(float radius, float brightness, ColorF3 color, float diffuseIntensity) 
            : base(color, diffuseIntensity)
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
        internal Matrix4 LightMatrix { get; private set; }
        protected override void OnWorldTransformChanged()
        {
            _cullingVolume.SetRenderTransform(WorldMatrix);
            foreach (PerspectiveCamera cam in ShadowCameras)
                cam.LocalPoint.Raw = WorldMatrix.Translation;
            LightMatrix = WorldMatrix * Matrix4.CreateScale(Radius);
            base.OnWorldTransformChanged();
        }

        public override void OnSpawned()
        {
            if (Type == LightType.Dynamic)
            {
                OwningScene.Lights.Add(this);
                SetShadowMapResolution(512);
            }

#if EDITOR
            if (Engine.EditorState.InEditMode)
                OwningScene.Add(this);
#endif
        }
        public override void OnDespawned()
        {
            if (Type == LightType.Dynamic)
                OwningScene.Lights.Remove(this);

#if EDITOR
            if (Engine.EditorState.InEditMode)
                OwningScene.Remove(this);
#endif
        }

        /// <summary>
        /// This is to set uniforms in the GBuffer lighting shader or in a forward shader that requests lighting uniforms.
        /// </summary>
        public override void SetUniforms(RenderProgram program)
        {
            string indexer = Uniform.PointLightsName + ".";
            program.Uniform(indexer + "Base.Color", _color.Raw);
            program.Uniform(indexer + "Base.DiffuseIntensity", _diffuseIntensity);
            program.Uniform(indexer + "Position", _cullingVolume.Center);
            program.Uniform(indexer + "Radius", Radius);
            program.Uniform(indexer + "Brightness", _brightness);

            TMaterialBase.SetTextureUniform(
                _shadowMap.Material.Textures[0].GetRenderTextureGeneric(true), 4, "Texture4", program);
        }
        /// <summary>
        /// This is to set special uniforms each time something is rendered with the shadow depth shader.
        /// </summary>
        private void SetShadowDepthUniforms(RenderProgram program)
        {
            program.Uniform("FarPlaneDist", Radius);
            program.Uniform("LightPos", _cullingVolume.Center);
            for (int i = 0; i < ShadowCameras.Length; ++i)
                program.Uniform(string.Format("ShadowMatrices[{0}]", i), ShadowCameras[i].WorldToCameraProjSpaceMatrix);
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
        private static TMaterial GetShadowMapMaterial(int cubeExtent, EDepthPrecision precision = EDepthPrecision.Int16)
        {
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

            //This material is used for rendering to the framebuffer.
            GLSLShaderFile fragShader = Engine.LoadEngineShader("PointLightShadowDepth.fs", EShaderMode.Fragment);
            GLSLShaderFile geomShader = Engine.LoadEngineShader("PointLightShadowDepth.gs", EShaderMode.Geometry);
            TMaterial mat = new TMaterial("PointLightShadowMat", new ShaderVar[0], refs, fragShader, geomShader);

            //No culling so if a light exists inside of a mesh it will shadow everything.
            mat.RenderParams.CullMode = ECulling.None;

            return mat;
        }
        public override void UpdateShadowMap(BaseScene scene)
        {
            scene.Update(_passes, _cullingVolume, null, null, true);
        }
        public override void RenderShadowMap(BaseScene scene)
        {
            Engine.Renderer.MaterialOverride = _shadowMap.Material;
            _shadowMap.Bind(EFramebufferTarget.DrawFramebuffer);
            Engine.Renderer.PushRenderArea(new BoundingRectangle(0.0f, 0.0f, _shadowResolution, _shadowResolution, 0.0f, 0.0f));
            {
                Engine.Renderer.Clear(EBufferClear.Color | EBufferClear.Depth);
                Engine.Renderer.AllowDepthWrite(true);
                scene.Render(_passes, null, null, null, null);
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

        public void AddRenderables(RenderPasses passes)
        {
            //passes.Add(Position, Color.LimeGreen, 20.0f);
        }

        public void AddRenderables(RenderPasses passes, Camera camera)
        {

        }
#endif
    }
}
