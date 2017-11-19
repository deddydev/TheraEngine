using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Files;
using TheraEngine.Worlds.Actors.Components;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Rendering.Particles
{
    [FileClass("cptcl", "Particle System Component")]
    public class ParticleSystemComponent : TRComponent
    {
        private List<Emitter> _emitters;

        public List<Emitter> Emitters
        {
            get => _emitters;
            set => _emitters = value;
        }
    }
}
