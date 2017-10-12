using System;

namespace TheraEngine.Particles
{
    public class ParticleState
    {
        LocalRotTransform _transform;

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
        public LocalRotTransform LocalTransform
        {
            get { return _transform; }
            set { _transform = value; }
        }
    }
}
