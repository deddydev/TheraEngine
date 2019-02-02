using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Components.Scene.Lights
{
    public enum ELightType
    {
        //Movable. Always calculates light for everything per-frame.
        Dynamic,
        //Moveable. Bakes into shadow maps when not moving.
        DynamicCached,
        //Does not move. Allows baking light into shadow maps.
        Static,
    }
    public abstract class LightComponent : TRComponent, IEditorPreviewIconRenderable
    {
        public LightComponent(ColorF3 color, float diffuseIntensity) : base()
        {
            _color = color;
            _diffuseIntensity = diffuseIntensity;
        }

        protected EventColorF3 _color = (ColorF3)Color.White;
        protected float _diffuseIntensity = 1.0f;
        protected int _lightIndex = -1;
        protected RenderPasses _passes = new RenderPasses();
        protected BoundingRectangle _region = new BoundingRectangle(0, 0, 1024, 1024);

        [Browsable(false)]
        public Matrix4 LightMatrix { get; protected set; }
        [Browsable(false)]
        public MaterialFrameBuffer ShadowMap { get; protected set; }
        [Browsable(false)]
        public Camera ShadowCamera { get; protected set; }

        [TSerialize]
        [DisplayName("Exponent Base")]
        [Category("Shadow Map Settings")]
        public float ShadowExponentBase { get; set; } = 0.04f;
        [TSerialize]
        [DisplayName("Exponent")]
        [Category("Shadow Map Settings")]
        public float ShadowExponent { get; set; } = 2.0f;
        [TSerialize]
        [DisplayName("Minimum Bias")]
        [Category("Shadow Map Settings")]
        public float ShadowMinBias { get; set; } = 0.00001f;
        [TSerialize]
        [DisplayName("Maximum Bias")]
        [Category("Shadow Map Settings")]
        public float ShadowMaxBias { get; set; } = 0.1f;

        [Category("Light Component")]
        public int ShadowMapResolutionWidth
        {
            get => _region.Width;
            set => SetShadowMapResolution(value, _region.Height);
        }
        [Category("Light Component")]
        public int ShadowMapResolutionHeight
        {
            get => _region.Height;
            set => SetShadowMapResolution(_region.Width, value);
        }
        [Category("Light Component")]
        public EventColorF3 LightColor
        {
            get => _color;
            set => _color = value;
        }
        [Category("Light Component")]
        public float DiffuseIntensity
        {
            get => _diffuseIntensity;
            set => _diffuseIntensity = value;
        }
        [Category("Light Component")]
        public ELightType Type { get; set; } = ELightType.Dynamic;

        [Browsable(false)]
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true)
        {
            VisibleInIBLCapture = false,
            EditorVisibilityMode = Rendering.RenderInfo.EEditorVisibility.VisibleAlways
        };

        internal void SetShadowUniforms(RenderProgram program)
        {
            program.Uniform("ShadowBase", ShadowExponentBase);
            program.Uniform("ShadowMult", ShadowExponent);
            program.Uniform("ShadowBiasMin", ShadowMinBias);
            program.Uniform("ShadowBiasMax", ShadowMaxBias);
        }

        public virtual void SetShadowMapResolution(int width, int height)
        {
            _region.Width = width;
            _region.Height = height;
            if (ShadowMap == null)
                ShadowMap = new MaterialFrameBuffer(GetShadowMapMaterial(width, height));
            else
                ShadowMap.Resize(width, height);
        }
        public abstract void SetUniforms(RenderProgram program, string targetStructName);
        protected virtual IVolume GetShadowVolume() => ShadowCamera?.Frustum;
        public abstract TMaterial GetShadowMapMaterial(int width, int height, EDepthPrecision precision = EDepthPrecision.Flt32);
        public void UpdateShadowMap(BaseScene scene)
        {
            IVolume volume = GetShadowVolume();
            scene.CollectVisible(_passes, volume, ShadowCamera, true);
        }
        internal void SwapBuffers()
        {
            _passes.SwapBuffers();
        }
        public void RenderShadowMap(BaseScene scene)
        {
            if (ShadowMap == null)
                return;

            Engine.Renderer.MaterialOverride = ShadowMap.Material;
            Engine.Renderer.PushRenderArea(_region);

            //scene.PreRender(null, ShadowCamera);
            scene.Render(_passes, ShadowCamera, null, ShadowMap);
            
            Engine.Renderer.PopRenderArea();
            Engine.Renderer.MaterialOverride = null;
        }
        public static EPixelInternalFormat GetShadowDepthMapFormat(EDepthPrecision precision)
        {
            switch (precision)
            {
                case EDepthPrecision.Int16: return EPixelInternalFormat.DepthComponent16;
                case EDepthPrecision.Int24: return EPixelInternalFormat.DepthComponent24;
                case EDepthPrecision.Int32: return EPixelInternalFormat.DepthComponent32;
            }
            return EPixelInternalFormat.DepthComponent32f;
        }

#if EDITOR
        [Category("Editor Traits")]
        public bool ScalePreviewIconByDistance { get; set; } = true;
        [Category("Editor Traits")]
        public float PreviewIconScale { get; set; } = 0.05f;

        string IEditorPreviewIconRenderable.PreviewIconName => PreviewIconName;
        protected abstract string PreviewIconName { get; }

        RenderCommandMesh3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private RenderCommandMesh3D PreviewIconRenderCommand { get; set; }

        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
        }
#endif
    }
}
