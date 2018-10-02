using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Actors.Types
{
    public class SkyboxActor : Actor<StaticMeshComponent>
    {
        public SkyboxActor() : base(true)
        {
            SkyboxTexture = null;
            HalfExtents = 5000.0f;
            Initialize();
        }
        public SkyboxActor(TextureFile2D skyboxTexture, Vec3 halfExtents) : base(true)
        {
            SkyboxTexture = skyboxTexture;
            HalfExtents = halfExtents;
            Initialize();
        }
        public SkyboxActor(GlobalFileRef<TextureFile2D> skyboxTexture, Vec3 halfExtents) : base(true)
        {
            SkyboxTexture = skyboxTexture;
            HalfExtents = halfExtents;
            Initialize();
        }

        [TSerialize]
        public Vec3 HalfExtents { get; set; }
        [TSerialize]
        public GlobalFileRef<TextureFile2D> SkyboxTexture { get; set; }

        protected override StaticMeshComponent OnConstructRoot()
        {
            TextureFile2D tex = SkyboxTexture?.File;
            Vec3 max = HalfExtents;
            Vec3 min = -max;

            StaticModel skybox = new StaticModel("Skybox");
            TMaterial mat = null;
            BoundingBox.ECubemapTextureUVs uvType = BoundingBox.ECubemapTextureUVs.WidthLarger;
            RenderingParameters renderParams = new RenderingParameters()
            {
                DepthTest = new DepthTest()
                {
                    Enabled = ERenderParamUsage.Enabled,
                    UpdateDepth = false,
                    Function = EComparison.Less
                }
            };

            if (tex != null)
            {
                TexRef2D texRef = new TexRef2D("SkyboxTexture", tex)
                {
                    MagFilter = ETexMagFilter.Nearest,
                    MinFilter = ETexMinFilter.Nearest
                };
                mat = TMaterial.CreateUnlitTextureMaterialForward(texRef, renderParams);
                uvType = tex.Bitmaps[0].Width > tex.Bitmaps[0].Height ?
                    BoundingBox.ECubemapTextureUVs.WidthLarger :
                    BoundingBox.ECubemapTextureUVs.HeightLarger;
            }
            else
            {
                mat = TMaterial.CreateUnlitColorMaterialForward(Color.Magenta);
                mat.RenderParams = renderParams;
            }

            StaticRigidSubMesh mesh = new StaticRigidSubMesh(
                "Mesh", 
                null,
                BoundingBox.FromMinMax(min, max),
                BoundingBox.SolidMesh(min, max, true, uvType),
                mat);

            foreach (LOD lod in mesh.LODs)
                lod.BillboardMode =
                    EBillboardMode.ConstrainTranslationX |
                    EBillboardMode.ConstrainTranslationY |
                    EBillboardMode.ConstrainTranslationZ;

            mesh.RenderInfo.RenderPass = ERenderPass.Background;

            skybox.RigidChildren.Add(mesh);

            return new StaticMeshComponent() { ModelRef = skybox };
        }
    }
}
