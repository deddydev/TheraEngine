using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Lights;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Tests;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public class ModelEditorWorld : World
    {
        public override void BeginPlay()
        {
            base.BeginPlay();

            DirectionalLightActor light = new DirectionalLightActor();
            light.RootComponent.AmbientIntensity = 0.0f;
            light.RootComponent.DiffuseIntensity = 2000.0f;
            light.RootComponent.LightColor = new EventColorF3(0.0f);
            SpawnActor(light);

            Collada.Data d = Collada.Import(Path.Combine(TestDefaults.DesktopPath, "teapot.dae"), new ModelImportOptions());
            StaticModel m = d.Models[0].StaticModel;
            StaticMeshComponent c = new StaticMeshComponent(m, null);
            Actor<StaticMeshComponent> a = new Actor<StaticMeshComponent>(c);
            SpawnActor(a, new Vec3(100.0f, 100.0f, 100.0f));
        }
    }
}
