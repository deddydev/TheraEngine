using System;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Core.Shapes;
using System.ComponentModel;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Models;
using System.IO;

namespace TheraEngine.Components.Scene
{
    public class DecalComponent : BoxComponent, I3DRenderable
    {
        internal PrimitiveManager DecalManager;
        internal Matrix4 DecalRenderMatrix { get; private set; }
        internal Matrix4 InverseDecalRenderMatrix { get; private set; }
        //private FrameBuffer RenderFBO { get; set; }
        //private QuadFrameBuffer QuadFBO { get; set; }
        internal TexRef2D AlbedoOpacity { get; private set; }
        //internal TexRef2D Normal { get; private set; }
        //internal TexRef2D RMSI { get; private set; }
        //private BoundingRectangle ViewRect { get; set; }

        [Category("Decal")]
        public TMaterial Material { get; set; }
        [Category("Decal")]
        public bool RenderIntersectionWireframe { get; set; } = false;
        [Category("Decal")]
        public bool PreRenderEnabled { get; set; } = true;
        [Category("Decal")]
        public override Vec3 HalfExtents
        {
            get => base.HalfExtents;
            set
            {
                base.HalfExtents = value;
                Vec3 halfExtents = Box.HalfExtents.Raw;
                DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
                InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;
            }
        }

        public DecalComponent() : base() { }
        public DecalComponent(Vec3 halfExtents) : base(halfExtents, null) { }

        protected override void OnWorldTransformChanged()
        {
            Vec3 halfExtents = Box.HalfExtents.Raw;
            DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
            InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;
            base.OnWorldTransformChanged();
        }
        public async void Initialize(int width, int height)
        {
            TextureFile2D tex = await Engine.LoadEngineTexture2DAsync("decal guide.png");
            AlbedoOpacity = new TexRef2D("DecalTex", tex);
            TexRef2D[] decalRefs = new TexRef2D[]
            {
                null,
                null,
                null,
                null,
                AlbedoOpacity
            };
            GLSLShaderFile decalShader = Engine.LoadEngineShader(Path.Combine(Viewport.SceneShaderPath, "DeferredDecal.fs"), EShaderMode.Fragment);
            ShaderVar[] decalVars = new ShaderVar[] { };
            RenderingParameters decalRenderParams = new RenderingParameters
            {
                CullMode = ECulling.Front,
                Requirements = EUniformRequirements.Camera,
                BlendMode = new BlendMode()
                {
                    Enabled = ERenderParamUsage.Enabled,
                    RgbEquation = EBlendEquationMode.FuncAdd,
                    AlphaEquation = EBlendEquationMode.FuncAdd,
                    RgbSrcFactor = EBlendingFactor.One,
                    RgbDstFactor = EBlendingFactor.Zero,
                    AlphaSrcFactor = EBlendingFactor.One,
                    AlphaDstFactor = EBlendingFactor.Zero,
                },
            };
            decalRenderParams.DepthTest.Enabled = ERenderParamUsage.Disabled;
            TMaterial decalMat = new TMaterial("DecalMat", decalRenderParams, decalVars, decalRefs, decalShader);
            PrimitiveData decalMesh = BoundingBox.SolidMesh(-Vec3.One, Vec3.One);
            DecalManager = new PrimitiveManager(decalMesh, decalMat);
            DecalManager.SettingUniforms += DecalManager_SettingUniforms;
            //ViewRect = new BoundingRectangle(0, 0, width, height);
            //QuadFBO = new QuadFrameBuffer(Material);
            AlbedoOpacity = TexRef2D.CreateFrameBufferTexture("DecalAlbedoOpacity", width, height,
                EPixelInternalFormat.Rgba8, EPixelFormat.Rgba, EPixelType.UnsignedByte);
            //Normal = TexRef2D.CreateFrameBufferTexture("DecalNormal", width, height,
            //    EPixelInternalFormat.Rgb16f, EPixelFormat.Rgb, EPixelType.HalfFloat);
            //RMSI = TexRef2D.CreateFrameBufferTexture("DecalRMSI", width, height,
            //    EPixelInternalFormat.Rgba8, EPixelFormat.Rgba, EPixelType.UnsignedByte);
            //RenderFBO = new FrameBuffer();
            //RenderFBO.SetRenderTargets(
            //    (AlbedoOpacity, EFramebufferAttachment.ColorAttachment0, 0, -1),
            //    (Normal, EFramebufferAttachment.ColorAttachment1, 0, -1),
            //    (RMSI, EFramebufferAttachment.ColorAttachment2, 0, -1));
        }
        private void DecalManager_SettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            Viewport v = Engine.Renderer.CurrentlyRenderingViewport;
            materialProgram.Sampler("Texture0", v.AlbedoOpacityTexture.RenderTextureGeneric, 0);
            materialProgram.Sampler("Texture1", v.NormalTexture.RenderTextureGeneric, 1);
            materialProgram.Sampler("Texture2", v.RMSITexture.RenderTextureGeneric, 2);
            materialProgram.Sampler("Texture3", v.DepthViewTexture.RenderTextureGeneric, 3);
            materialProgram.Uniform("BoxWorldMatrix", WorldMatrix);
            materialProgram.Uniform("InvBoxWorldMatrix", InverseWorldMatrix);
            materialProgram.Uniform("BoxHalfScale", Box.HalfExtents.Raw);
        }
        public override void OnSpawned()
        {
            Initialize(128, 128);
            //OwningScene.Decals.Add(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            //OwningScene.Decals.Add(this);
            base.OnDespawned();
        }
        public override void Render()
        {
            DecalManager.Render(DecalRenderMatrix);
            if (RenderIntersectionWireframe)
                base.Render();
        }
        //public void PreRenderUpdate(Camera camera)
        //{

        //}
        //public void PreRenderSwap()
        //{

        //}
        //public void PreRender(Viewport viewport, Camera camera)
        //{
        //    RenderFBO.Bind(EFramebufferTarget.DrawFramebuffer);
        //    Engine.Renderer.PushRenderArea(ViewRect);
        //    Engine.Renderer.Clear(EFBOTextureType.Color);
        //    QuadFBO.RenderFullscreen();
        //    Engine.Renderer.PopRenderArea();
        //    RenderFBO.Unbind(EFramebufferTarget.DrawFramebuffer);
        //    PreRenderEnabled = false;
        //}
    }
}
