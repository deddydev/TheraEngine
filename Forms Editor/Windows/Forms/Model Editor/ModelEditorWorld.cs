using TheraEngine.Actors.Types.Lights;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Rendering.Models.Materials;
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

            //Collada.Data d = Collada.Import(Path.Combine(TestDefaults.DesktopPath, "teapot.dae"), new ModelImportOptions());
            //StaticModel m = d.Models[0].StaticModel;
            //StaticMeshComponent c = new StaticMeshComponent(m, null);
            //Actor<StaticMeshComponent> a = new Actor<StaticMeshComponent>(c);
            //SpawnActor(a, new Vec3(100.0f, 100.0f, 100.0f));
        }
    }
}
