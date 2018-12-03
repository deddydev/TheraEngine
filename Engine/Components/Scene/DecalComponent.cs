using System;
using System.ComponentModel;
using System.IO;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Components.Scene
{
    public class DecalComponent : BoxComponent, I3DRenderable
    {
        protected PrimitiveManager DecalManager { get; set; }
        internal Matrix4 DecalRenderMatrix { get; private set; }
        internal Matrix4 InverseDecalRenderMatrix { get; private set; }
        protected float _uniformScale = 1.0f;
        
        [TSerialize]
        [Category("Decal")]
        public TMaterial Material { get; set; }
        [TSerialize]
        [Category("Decal")]
        public bool RenderIntersectionWireframe { get; set; } = false;
        //[TSerialize]
        //[Category("Decal")]
        //public override Vec3 HalfExtents
        //{
        //    get => base.HalfExtents;
        //    set
        //    {
        //        base.HalfExtents = value;
        //        //Vec3 halfExtents = Box.HalfExtents.Raw * UniformScale;
        //        //DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
        //        //InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;
        //    }
        //}

        public DecalComponent() : base() { RenderInfo.RenderPass = ERenderPass.DeferredDecals; RenderInfo.VisibleByDefault = true; }
        public DecalComponent(Vec3 halfExtents) : base(halfExtents, null) { RenderInfo.RenderPass = ERenderPass.DeferredDecals; RenderInfo.VisibleByDefault = true; }
        public DecalComponent(Vec3 halfExtents, TextureFile2D texture) : base(halfExtents, null)
        {
            RenderInfo.RenderPass = ERenderPass.DeferredDecals;
            RenderInfo.VisibleByDefault = true;
            if (texture != null)
                Material = CreateDefaultMaterial(texture);
        }
        public DecalComponent(float height, TextureFile2D texture) : base(new Vec3(1.0f, height, 1.0f), null)
        {
            RenderInfo.RenderPass = ERenderPass.DeferredDecals;
            RenderInfo.VisibleByDefault = true;
            if (texture != null)
            {
                var bmp = texture.GetLargestBitmap();
                if (bmp != null)
                {
                    _shape.HalfExtents = new Vec3(bmp.Width * 0.5f, height, bmp.Height * 0.5f);
                    Material = CreateDefaultMaterial(texture);
                }
            }
        }
        
        protected override void OnWorldTransformChanged()
        {
            Vec3 halfExtents = _shape.HalfExtents.Raw;
            DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
            InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;
            base.OnWorldTransformChanged();
        }
        /// <summary>
        /// Generates a basic decal material that projects a single texture onto surfaces. Texture may use transparency.
        /// </summary>
        /// <param name="texture">The texture to project as a decal.</param>
        /// <returns>The <see cref="TMaterial"/> to be used with a <see cref="DecalComponent"/>.</returns>
        public static TMaterial CreateDefaultMaterial(TextureFile2D texture)
        {
            TexRef2D[] decalRefs = new TexRef2D[]
            {
                null,
                null,
                null,
                null,
                new TexRef2D("DecalTexture", texture)
            };
            GLSLShaderFile decalShader = Engine.LoadEngineShader(Path.Combine(Viewport.SceneShaderPath, "DeferredDecal.fs"), EShaderMode.Fragment);
            ShaderVar[] decalVars = new ShaderVar[] { };
            RenderingParameters decalRenderParams = new RenderingParameters
            {
                CullMode = ECulling.Front,
                Requirements = EUniformRequirements.Camera,
                DepthTest = new DepthTest() { Enabled = ERenderParamUsage.Disabled }
            };
            return new TMaterial("DecalMat", decalRenderParams, decalVars, decalRefs, decalShader);
        }
        public void Initialize()
        {
            if (Material == null)
                return;
            PrimitiveData decalMesh = BoundingBox.SolidMesh(-Vec3.One, Vec3.One);
            DecalManager = new PrimitiveManager(decalMesh, Material);
            DecalManager.SettingUniforms += DecalManager_SettingUniforms;
        }
        protected virtual void DecalManager_SettingUniforms(RenderProgram vertexProgram, RenderProgram materialProgram)
        {
            if (materialProgram == null)
                return;
            Viewport v = Engine.Renderer.CurrentlyRenderingViewport;
            materialProgram.Sampler("Texture0", v.AlbedoOpacityTexture.RenderTextureGeneric, 0);
            materialProgram.Sampler("Texture1", v.NormalTexture.RenderTextureGeneric, 1);
            materialProgram.Sampler("Texture2", v.RMSITexture.RenderTextureGeneric, 2);
            materialProgram.Sampler("Texture3", v.DepthViewTexture.RenderTextureGeneric, 3);
            materialProgram.Uniform("BoxWorldMatrix", WorldMatrix);
            materialProgram.Uniform("InvBoxWorldMatrix", InverseWorldMatrix);
            materialProgram.Uniform("BoxHalfScale", _shape.HalfExtents.Raw);
        }
        public override void OnSpawned()
        {
            Initialize();
            base.OnSpawned();
        }
        protected override void Render()
        {
            DecalManager?.Render(DecalRenderMatrix);
            if (RenderIntersectionWireframe)
                base.Render();
        }
    }
}
