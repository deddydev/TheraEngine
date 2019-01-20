using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
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
    public abstract class LightComponent : TRComponent, I3DRenderable
    {
        protected EventColorF3 _color = (ColorF3)Color.White;
        protected float _diffuseIntensity = 1.0f;
        protected int _lightIndex = -1;
        protected RenderPasses _passes = new RenderPasses();

        [Browsable(false)]
        public Matrix4 LightMatrix { get; protected set; }
        //[Browsable(false)]
        public MaterialFrameBuffer ShadowMap;
        [Browsable(false)]
        public Camera ShadowCamera { get; protected set; }
        public ShadowSettings ShadowSettings { get; } = new ShadowSettings();

        protected BoundingRectangle _region = new BoundingRectangle(0, 0, 1024, 1024);
        
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
        protected ELightType Type { get; set; } = ELightType.Dynamic;

        [Browsable(false)]
        public RenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true);
        [Browsable(false)]
        public virtual Shape CullingVolume { get; } = null;
        IOctreeNode I3DBoundable.OctreeNode { get; set; }

        public LightComponent(ColorF3 color, float diffuseIntensity) : base()
        {
            _color = color;
            _diffuseIntensity = diffuseIntensity;
        }
        internal void SwapBuffers()
        {
            _passes.SwapBuffers();
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
        protected abstract string GetPreviewTextureName();
        public override void OnSpawned()
        {
            base.OnSpawned();

            PreviewRenderCommand = new RenderCommandMesh3D(ERenderPass.TransparentForward);
            VertexQuad quad = VertexQuad.PosZQuad();
            PrimitiveData data = PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), quad.ToTriangles());
            string texPath = Engine.Files.TexturePath(GetPreviewTextureName());
            TexRef2D tex = new TexRef2D("LightPreviewIcon", texPath) { SamplerName = "Texture0" };
            TMaterial mat = TMaterial.CreateUnlitAlphaTextureMaterialForward(tex);
            mat.RenderParams = new RenderingParameters()
            {
                DepthTest = new DepthTest { Enabled = ERenderParamUsage.Disabled },
            };
            PreviewRenderCommand.Mesh = new PrimitiveManager(data, mat);
        }
        public bool ScalePreviewByDistance { get; set; } = true;
        public float PreviewScale { get; set; } = 0.08f;
        public RenderCommandMesh3D PreviewRenderCommand { get; private set; }
        public void AddRenderables(RenderPasses passes, Camera camera)
        {
            float camDist = camera.DistanceFromScreenPlane(WorldPoint);
            Vec3 scale = new Vec3(ScalePreviewByDistance ? camDist * PreviewScale : PreviewScale);

            PreviewRenderCommand.RenderDistance = camDist;
            PreviewRenderCommand.WorldMatrix = Matrix4.CreateSpacialTransform(WorldPoint, 
                camera.RightVector * scale, camera.UpVector * scale, camera.ForwardVector * scale);

            //passes.Add(PreviewRenderCommand);
        }
#endif
    }
}
