using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Actors;
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
            light.RootComponent.AmbientIntensity = 0.0f;
            light.RootComponent.DiffuseIntensity = 2000.0f;
            light.RootComponent.LightColor = new EventColorF3(0.0f);
            SpawnActor(light);
        }
    }
}
