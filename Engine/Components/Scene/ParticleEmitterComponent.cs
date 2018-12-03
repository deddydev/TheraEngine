using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Rendering.Particles
{
    [TFileDef("Particle System Component")]
    public class ParticleSystemComponent : TRComponent
    {
        public List<Emitter> Emitters { get; set; }
    }
}
