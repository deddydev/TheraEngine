using OpenTK;

namespace CustomEngine.Worlds.Actors.Components
{
    public class MovementComponent : LogicComponent
    {
        protected Vector3 _positionOffset;

        public void AddMovementInput(Vector3 offset)
        {
            _positionOffset += offset;
        }
    }
}
