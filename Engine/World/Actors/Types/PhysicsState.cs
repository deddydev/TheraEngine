using OpenTK;

namespace CustomEngine.Worlds.Actors
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
