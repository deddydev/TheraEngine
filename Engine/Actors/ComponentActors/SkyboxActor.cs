using System.ComponentModel;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes;
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
            SkyboxTextureRef = new GlobalFileRef<TextureFile2D>();
            HalfExtents = 5000.0f;
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

        private GlobalFileRef<TextureFile2D> _skyboxTextureRef;
        private TMaterial _material;
        [TSerialize(nameof(HalfExtents))]
        private Vec3 _halfExtents = 5000.0f;
        [TSerialize(nameof(TexCoordEdgeBias))]
        private float _bias = 0.0f;
        private BoundingBox.ECubemapTextureUVs _uvType = BoundingBox.ECubemapTextureUVs.WidthLarger;
        
        public float TexCoordEdgeBias
        {
            get => _bias;
            set
            {
                _bias = value;
                Remake();
            }
        }
        public Vec3 HalfExtents
        {
            get => _halfExtents;
            set
            {
                _halfExtents = value;
                Remake();
            }
        }
        [TString(false, true)]
        public string TexturePath
        {
            get => SkyboxTextureRef?.AbsolutePath;
            set
            {
                GlobalFileRef<TextureFile2D> tref = SkyboxTextureRef;
                if (tref is null)
                    return;

                tref.AbsolutePath = value;
                tref.Reload();
            }
        }
        [TSerialize]
        public GlobalFileRef<TextureFile2D> SkyboxTextureRef
        {
            get => _skyboxTextureRef;
            set
            {
                if (_skyboxTextureRef != null)
                {
                    _skyboxTextureRef.Loaded -= TextureLoaded;
                    _skyboxTextureRef.Unloaded -= TextureUnloaded;
                }
                _skyboxTextureRef = value;
                if (_skyboxTextureRef != null)
                {
                    _skyboxTextureRef.Loaded += TextureLoaded;
                    _skyboxTextureRef.Unloaded += TextureUnloaded;
                }
            }
        }

        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;

                //Update model reference and renderable mesh

                StaticModel model = RootComponent?.ModelRef?.File;
                if (model is null || model.RigidChildren.Count == 0)
                    return;

                StaticRigidSubMesh mesh = model.RigidChildren[0];
                if (mesh is null)
                    return;
                
                if (mesh.LODs.Count > 0)
                {
                    ILOD lod = mesh.LODs[0];
                    if (lod?.MaterialRef != null)
                        lod.MaterialRef.File = _material;
                }

                var renderMeshes = RootComponent?.Meshes;
                if (renderMeshes is null || renderMeshes.Count <= 0)
                    return;

                StaticRenderableMesh rmesh = renderMeshes[0];
                if (rmesh.LODs is null || rmesh.LODs.Count == 0)
                    return;

                RenderableLOD rlod = rmesh?.LODs[0];
                if (rlod is null)
                    return;

                rlod.Manager.Material = _material;
            }
        }

        private void TextureLoaded(TextureFile2D tex)
        {
            if (Material is null || Material.Textures.Count == 0)
                return;

            TexRef2D tref = (TexRef2D)Material.Textures[0];
            if (tref.Mipmaps.Length == 0)
                return;
            
            if (tref.Mipmaps[0] != null)
                tref.Mipmaps[0].File = tex;
            else
                tref.Mipmaps[0] = new GlobalFileRef<TextureFile2D>(tex);

            tref.OnMipLoaded(tex);

            BoundingBox.ECubemapTextureUVs uvType = tex is null || tex.Bitmaps.Length == 0 || tex.Bitmaps[0] is null || tex.Bitmaps[0].Width > tex.Bitmaps[0].Height ?
                BoundingBox.ECubemapTextureUVs.WidthLarger :
                BoundingBox.ECubemapTextureUVs.HeightLarger;

            if (_uvType != uvType)
            {
                _uvType = uvType;
                Remake();
            }
        }
        private void Remake()
        {
            StaticModel model = RootComponent?.ModelRef?.File;
            if (model is null || model.RigidChildren.Count == 0)
                return;

            StaticRigidSubMesh mesh = model.RigidChildren[0];
            if (mesh is null)
                return;

            BoundingBox box = mesh.RenderInfo.CullingVolume as BoundingBox;
            if (box != null)
            {
                box.Minimum = -HalfExtents;
                box.Maximum = HalfExtents;
            }

            if (mesh.LODs.Count > 0)
            {
                ILOD lod = mesh.LODs[0];
                if (lod?.PrimitivesRef != null)
                {
                    lod.PrimitivesRef.File?.Dispose();
                    lod.PrimitivesRef.File = BoundingBox.SolidMesh(-HalfExtents, HalfExtents, true, _uvType, TexCoordEdgeBias);
                }
            }

            var renderMeshes = RootComponent?.Meshes;
            if (renderMeshes is null || renderMeshes.Count <= 0 || renderMeshes[0] is null)
                return;
            
            StaticRenderableMesh rmesh = renderMeshes[0];
            //rmesh.RenderInfo.CullingVolume = box.HardCopy();
            if (rmesh.LODs is null || rmesh.LODs.Count == 0)
                return;

            RenderableLOD rlod = rmesh.LODs[0];
            if (rlod is null)
                return;

            rlod.Manager.Data?.Dispose();
            rlod.Manager.Data = BoundingBox.SolidMesh(-HalfExtents, HalfExtents, true, _uvType, TexCoordEdgeBias);
            rlod.Manager.BufferInfo.BillboardMode = ETransformFlags.ConstrainTranslations;
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

            RenderingParameters renderParams = new RenderingParameters()
            {
                DepthTest = new DepthTest()
                {
                    Enabled = ERenderParamUsage.Enabled,
                    UpdateDepth = false,
                    Function = EComparison.Less
                }
            };
          
            TexRef2D texRef = new TexRef2D("SkyboxTexture", tex)
            {
                MagFilter = ETexMagFilter.Nearest,
                MinFilter = ETexMinFilter.Nearest
            };

            _material = TMaterial.CreateUnlitTextureMaterialForward(texRef);
            _material.RenderParams = renderParams;

            _uvType = tex is null || tex.Bitmaps.Length == 0 || tex.Bitmaps[0] is null || tex.Bitmaps[0].Width > tex.Bitmaps[0].Height ?
                BoundingBox.ECubemapTextureUVs.WidthLarger :
                BoundingBox.ECubemapTextureUVs.HeightLarger;

            //TShape box = BoundingBox.FromMinMax(min, max);
            RenderInfo3D renderInfo = new RenderInfo3D(true, false) { /*CullingVolume = box*/ };
            StaticRigidSubMesh mesh = new StaticRigidSubMesh(
                "Mesh",
                renderInfo,
                ERenderPass.Background,
                BoundingBox.SolidMesh(min, max, true, _uvType, TexCoordEdgeBias),
                Material);

            foreach (LOD lod in mesh.LODs)
                lod.TransformFlags = ETransformFlags.ConstrainTranslations;
            
            skybox.RigidChildren.Add(mesh);

            return new StaticMeshComponent() { ModelRef = skybox, AllowOriginRebase = false };
        }
    }
}
