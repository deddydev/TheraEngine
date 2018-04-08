using System;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
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

            Vec3 max = 1000.0f;
            Vec3 min = -max;
            TextureFile2D skyTex = Engine.LoadEngineTexture2D("modelviewerbg.png");
            StaticModel skybox = new StaticModel("Skybox");
            StaticRigidSubMesh mesh = new StaticRigidSubMesh("Mesh", true, 
                BoundingBox.FromMinMax(min, max),
                BoundingBox.SolidMesh(min, max, true, 
                skyTex.Bitmaps[0].Width > skyTex.Bitmaps[0].Height ?
                    BoundingBox.ECubemapTextureUVs.WidthLarger :
                    BoundingBox.ECubemapTextureUVs.HeightLarger),
                TMaterial.CreateUnlitTextureMaterialForward(new TexRef2D("SkyboxTexture", skyTex), new RenderingParameters() { DepthTest = new DepthTest() { Enabled = false } }));
            mesh.RenderInfo.RenderPass = TheraEngine.Rendering.ERenderPass3D.OpaqueForward;
            skybox.RigidChildren.Add(mesh);
            Actor<StaticMeshComponent> skyboxActor = new Actor<StaticMeshComponent>();
            skyboxActor.RootComponent.ModelRef = skybox;
            SpawnActor(skyboxActor);
        }
    }
}
