﻿using System;
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
    public class DecalComponent : BoxComponent, I3DRenderable, IEditorPreviewIconRenderable
    {
        [TSerialize]
        [Category("Decal")]
        public TMaterial Material { get; set; }
        
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
        public DecalComponent(Vec3 halfExtents, TMaterial material)
            : base(halfExtents, null)
        {
            RenderInfo.VisibleByDefault = true;
            Material = material;
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
                    _shape.HalfExtents.Raw = new Vec3(bmp.Width * 0.5f, height, bmp.Height * 0.5f);
                    Material = CreateDefaultMaterial(texture);
                }
            }
        }
        
        protected override void OnWorldTransformChanged()
        {
            Vec3 halfExtents = _shape.HalfExtents.Raw;

            //DecalRenderMatrix = WorldMatrix * halfExtents.AsScaleMatrix();
            //InverseDecalRenderMatrix = (1.0f / halfExtents).AsScaleMatrix() * InverseWorldMatrix;

            RenderCommandDecal.WorldMatrix = WorldMatrix * halfExtents.AsScaleMatrix();

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
                null, //Viewport's Albedo/Opacity texture
                null, //Viewport's Normal texture
                null, //Viewport's RMSI texture
                null, //Viewport's Depth texture
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
        public override void OnSpawned()
        {
            if (Material == null)
                return;

            PrimitiveData decalMesh = BoundingBox.SolidMesh(-Vec3.One, Vec3.One);
            RenderCommandDecal.Mesh = new PrimitiveManager(decalMesh, Material);
            RenderCommandDecal.Mesh.SettingUniforms += DecalManager_SettingUniforms;

            base.OnSpawned();
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

        public RenderCommandMesh3D RenderCommandDecal { get; } = new RenderCommandMesh3D(ERenderPass.DeferredDecals);

#if EDITOR
        [Category("Editor Traits")]
        public bool ScalePreviewIconByDistance { get; set; } = true;
        [Category("Editor Traits")]
        public float PreviewIconScale { get; set; } = 0.05f;

        string IEditorPreviewIconRenderable.PreviewIconName => PreviewIconName;
        protected string PreviewIconName { get; } = "CameraIcon.png";

        RenderCommandMesh3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private RenderCommandMesh3D PreviewIconRenderCommand { get; set; }
#endif

        //TODO: separate visibility of the decal mesh and wireframe intersection
        public override void AddRenderables(RenderPasses passes, Camera camera)
        {
            passes.Add(RenderCommandDecal);
#if EDITOR
            if (Engine.EditorState.InEditMode)
            {
                base.AddRenderables(passes, camera);
                AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
            }
#endif
        }
    }
}
