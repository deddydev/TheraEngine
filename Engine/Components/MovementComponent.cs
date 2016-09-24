using OpenTK;

namespace CustomEngine.Components
{
    public class MovementComponent : Component
    {
        protected Vector3 _positionOffset;

        public void AddMovementInput(Vector3 offset)
        {
            _positionOffset += offset;
        }
    }
}
