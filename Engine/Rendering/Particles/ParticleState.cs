using System;

namespace TheraEngine.Particles
{
    public class ParticleState
    {
        Transform _transform;

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
        public Transform LocalTransform
        {
            get { return _transform; }
            set { _transform = value; }
        }
    }
}
