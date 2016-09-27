using OpenTK;

namespace CustomEngine.World.Actors.Components
{
    public class MovementComponent : InstanceComponent
    {
        protected Vector3 _positionOffset;

        public void AddMovementInput(Vector3 offset)
        {
            _positionOffset += offset;
        }
    }
}
