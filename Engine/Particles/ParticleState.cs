using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models;

namespace CustomEngine.Particles
{
    public class ParticleState : ITransformable
    {
        FrameState _transform;
        public FrameState Transform
        {
            get { return _transform; }
            set { _transform = value; }
        }
    }
}
