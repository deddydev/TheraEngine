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

        public Matrix4 WorldMatrix
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public Matrix4 InverseWorldMatrix
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        public FrameState LocalTransform
        {
            get { return _transform; }
            set { _transform = value; }
        }
    }
}
