namespace CustomEngine.Worlds.Actors.Components
{
    public enum MovementMode
    {
        Walking,
        Falling,
        Swimming,
        Flying,
    }
    public class CharacterMovementComponent : MovementComponent
    {
        private MovementMode _currentMovementMode = MovementMode.Falling;
        private bool _isCrouched = false;

        public void Jump()
        {

        }
        public void ToggleCrouch()
        {

        }
        public void SetCrouched()
        {

        }
    }
}
