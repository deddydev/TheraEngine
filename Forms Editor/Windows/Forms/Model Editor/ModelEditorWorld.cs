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
        }
    }
}
