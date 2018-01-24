using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Rendering.Particles
{
    [FileDef("Particle System Component")]
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
