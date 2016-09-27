using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.World.Actors
{
    public class PhysicsState
    {
        public Vector3 _velocity;
        public Vector3 _acceleration;
        public Vector3 _momentum;
        public float Inertia { get { return 0.0f; } }
        public float Speed { get { return 0.0f; } }
    }
}
