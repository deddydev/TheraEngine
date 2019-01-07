using System.ComponentModel;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
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
        public SkyboxActor() : base(true)
        {
            Initialize();
        }
        public SkyboxActor(TextureFile2D skyboxTexture, Vec3 halfExtents) : base(true)
        {
            SkyboxTextureRef = skyboxTexture;
            HalfExtents = halfExtents;
            Initialize();
        }
        public SkyboxActor(GlobalFileRef<TextureFile2D> skyboxTexture, Vec3 halfExtents) : base(true)
        {
            SkyboxTextureRef = skyboxTexture;
            HalfExtents = halfExtents;
            Initialize();
        }

        private GlobalFileRef<TextureFile2D> _skyboxTextureRef = new GlobalFileRef<TextureFile2D>();
        private TMaterial _material;

        [TSerialize]
        public Vec3 HalfExtents { get; set; } = 5000.0f;
        [TSerialize]
        public GlobalFileRef<TextureFile2D> SkyboxTextureRef
        {
            get => _skyboxTextureRef;
            set
            {
                if (_skyboxTextureRef != null)
                {
                    _skyboxTextureRef.UnregisterLoadEvent(TextureLoaded);
                    _skyboxTextureRef.UnregisterUnloadEvent(TextureUnloaded);
                }
                _skyboxTextureRef = value;
                if (_skyboxTextureRef != null)
                {
                    _skyboxTextureRef.RegisterLoadEvent(TextureLoaded);
                    _skyboxTextureRef.RegisterUnloadEvent(TextureUnloaded);
                }
            }
        }
        private void TextureLoaded(TextureFile2D tex)
        {
            if (_material != null && _material.Textures.Length > 0)
            {
                TexRef2D tref = (TexRef2D)_material.Textures[0];
                if (tref.Mipmaps.Length > 0)
                {
                    if (tref.Mipmaps[0] != null)
                        tref.Mipmaps[0].File = tex;
                    else
                        tref.Mipmaps[0] = new GlobalFileRef<TextureFile2D>(tex);
                    tref.OnMipLoaded(tex);
                }
            }
        }
        private void TextureUnloaded(TextureFile2D tex)
        {

        }
        protected override StaticMeshComponent OnConstructRoot()
        {
            _material = null;

            TextureFile2D tex = SkyboxTextureRef?.File;
            Vec3 max = HalfExtents;
            Vec3 min = -max;

            StaticModel skybox = new StaticModel("Skybox");
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

            //if (tex != null)
            //{
                TexRef2D texRef = new TexRef2D("SkyboxTexture", tex)
                {
                    MagFilter = ETexMagFilter.Nearest,
                    MinFilter = ETexMinFilter.Nearest
                };
                _material = TMaterial.CreateUnlitTextureMaterialForward(texRef, renderParams);
                uvType = tex == null || tex.Bitmaps[0].Width > tex.Bitmaps[0].Height ?
                    BoundingBox.ECubemapTextureUVs.WidthLarger :
                    BoundingBox.ECubemapTextureUVs.HeightLarger;
            //}
            //else
            //{
            //    _material = TMaterial.CreateUnlitColorMaterialForward(Color.Magenta);
            //    _material.RenderParams = renderParams;
            //}

            StaticRigidSubMesh mesh = new StaticRigidSubMesh(
                "Mesh",
                null,
                BoundingBox.FromMinMax(min, max),
                BoundingBox.SolidMesh(min, max, true, uvType),
                _material);

            foreach (LOD lod in mesh.LODs)
                lod.TransformFlags = ETransformFlags.ConstrainTranslations;

            mesh.RenderInfo.RenderPass = ERenderPass.Background;

            skybox.RigidChildren.Add(mesh);

            return new StaticMeshComponent() { ModelRef = skybox };
        }
    }
}
