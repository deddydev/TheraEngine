using System;
using System.ComponentModel;
using System.IO;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Components.Scene
{
    public class DecalComponent : BoxComponent, I3DRenderable
    {
        internal Matrix4 DecalRenderMatrix { get; private set; }
        internal Matrix4 InverseDecalRenderMatrix { get; private set; }
        
        [TSerialize]
        [Category("Decal")]
        public TMaterial Material { get; set; }

        //[TSerialize(nameof(AlwaysRenderIntersectionWireframe))]
        //private bool _alwaysRenderIntersectionWireframe = false;
        //[Category("Editor Traits")]
        //[Description("If true, the intersection wireframe will always be rendered in edit mode even if the decal is not selected.")]
        //public bool AlwaysRenderIntersectionWireframe
        //{
        //    get => _alwaysRenderIntersectionWireframe;
        //    set
        //    {
        //        if (_alwaysRenderIntersectionWireframe == value)
        //            return;

        //        _alwaysRenderIntersectionWireframe = value;

        //        if (IsSpawned)
        //        {
        //            if (_alwaysRenderIntersectionWireframe)
        //                RenderInfo.Visible = true;
        //            else if (!EditorState.Selected)
        //                RenderInfo.Visible = false;
        //        }
        //    }
        //}

        public RenderCommandMesh3D RenderCommandDecal { get; } = new RenderCommandMesh3D(ERenderPass.DeferredDecals);

        public DecalComponent() 
            : base()
        {
            RenderInfo.VisibleByDefault = true;
        }
        public DecalComponent(Vec3 halfExtents) 
            : base(halfExtents, null)
        {
            RenderInfo.VisibleByDefault = true;
        }
        public DecalComponent(Vec3 halfExtents, TextureFile2D texture)
            : base(halfExtents, null)
        {
            RenderInfo.VisibleByDefault = true;
            if (texture != null)
                Material = CreateDefaultMaterial(texture);
        }
        public DecalComponent(float height, TextureFile2D texture)
            : base(new Vec3(1.0f, height, 1.0f), null)
        {
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

            RenderCommandDecal.WorldMatrix = DecalRenderMatrix;

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
            GLSLScript decalShader = Engine.Files.LoadEngineShader(Path.Combine(Viewport.SceneShaderPath, "DeferredDecal.fs"), EGLSLType.Fragment);
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
            RenderCommandDecal.Mesh = new PrimitiveManager(decalMesh, Material);
            RenderCommandDecal.Mesh.SettingUniforms += DecalManager_SettingUniforms;
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
        //TODO: separate visibility of the decal mesh and wireframe intersection
        public override void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(RenderCommandDecal);
            if (Engine.EditorState.InEditMode)
                base.AddRenderables(passes, camera);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            RenderInfo.Visible = selected;
        }
    }
}
