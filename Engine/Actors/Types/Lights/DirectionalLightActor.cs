using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Components.Scene.Lights;

namespace TheraEngine.Actors.Types.Lights
{
    public class DirectionalLightActor : Actor<DirectionalLightComponent>
    {
        public DirectionalLightActor() : base(false) { }
    }
}
