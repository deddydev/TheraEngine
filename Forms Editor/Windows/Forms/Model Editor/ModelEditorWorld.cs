using System;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public class ModelEditorWorld : World
    {
        public override void BeginPlay()
        {
            base.BeginPlay();

            DirectionalLightActor light = new DirectionalLightActor();
            DirectionalLightComponent comp = light.RootComponent;
            comp.AmbientIntensity = 0.01f;
            comp.DiffuseIntensity = 1.0f;
            comp.LightColor = new EventColorF3(1.0f);
            comp.Rotation.Yaw = 45.0f;
            comp.Rotation.Pitch = -45.0f;
            SpawnActor(light);

            Vec3 max = 5000.0f;
            Vec3 min = -max;
            TextureFile2D skyTex = Engine.LoadEngineTexture2D("skybox.png");
            StaticModel skybox = new StaticModel("Skybox");
            TexRef2D texRef = new TexRef2D("SkyboxTexture", skyTex)
            {
                MagFilter = ETexMagFilter.Nearest,
                MinFilter = ETexMinFilter.Nearest
            };
            StaticRigidSubMesh mesh = new StaticRigidSubMesh("Mesh", true,
                BoundingBox.FromMinMax(min, max),
                BoundingBox.SolidMesh(min, max, true,
                skyTex.Bitmaps[0].Width > skyTex.Bitmaps[0].Height ?
                    BoundingBox.ECubemapTextureUVs.WidthLarger :
                    BoundingBox.ECubemapTextureUVs.HeightLarger),
                TMaterial.CreateUnlitTextureMaterialForward(texRef, new RenderingParameters()
                {
                    DepthTest = new DepthTest() { Enabled = true, UpdateDepth = false, Function = EComparison.Less }
                }));
            mesh.RenderInfo.RenderPass = ERenderPass3D.Skybox;
            skybox.RigidChildren.Add(mesh);
            Actor<StaticMeshComponent> skyboxActor = new Actor<StaticMeshComponent>();
            skyboxActor.RootComponent.ModelRef = skybox;
            SpawnActor(skyboxActor);
        }
    }
}
