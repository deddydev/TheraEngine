using System;
using System.ComponentModel;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;

namespace TheraEngine.Actors.Types
{
    public class SkyboxActor : Actor<StaticMeshComponent>
    {
        public SkyboxActor()
        {
            SkyboxTexture = null;
            HalfExtents = 5000.0f;
        }
        public SkyboxActor(TextureFile2D skyboxTexture, Vec3 halfExtents) : base(true)
        {
            SkyboxTexture = skyboxTexture;
            HalfExtents = halfExtents;
            Initialize();
        }

        [TSerialize]
        public Vec3 HalfExtents { get; set; }
        [TSerialize]
        public TextureFile2D SkyboxTexture { get; set; }

        protected override StaticMeshComponent OnConstruct()
        {
            Vec3 max = HalfExtents;
            Vec3 min = -max;

            StaticModel skybox = new StaticModel("Skybox");
            
            TexRef2D texRef = new TexRef2D("SkyboxTexture", SkyboxTexture)
            {
                MagFilter = ETexMagFilter.Nearest,
                MinFilter = ETexMinFilter.Nearest
            };

            TMaterial mat = TMaterial.CreateUnlitTextureMaterialForward(texRef, new RenderingParameters()
            {
                DepthTest = new DepthTest()
                {
                    Enabled = ERenderParamUsage.Enabled,
                    UpdateDepth = false,
                    Function = EComparison.Less
                }
            });

            BoundingBox.ECubemapTextureUVs uvType =
                SkyboxTexture == null || 
                SkyboxTexture.Bitmaps[0].Width > SkyboxTexture.Bitmaps[0].Height ?
                    BoundingBox.ECubemapTextureUVs.WidthLarger :
                    BoundingBox.ECubemapTextureUVs.HeightLarger;

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
