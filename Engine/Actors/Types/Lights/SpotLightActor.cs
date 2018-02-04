using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Components.Scene.Lights;

namespace TheraEngine.Actors.Types.Lights
{
    public class SpotLightActor : Actor<SpotLightComponent>
    {
        public SpotLightActor() : base(false) { }
    }
}
