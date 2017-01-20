using BulletSharp;
using CustomEngine.Rendering;
using System;

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
        private float _maxWalkAngle = 50.0f;
        private Vec3 _jumpVelocity = Vec3.Up * 10.0f;
        private PhysicsDriver _currentWalkingSurface;
        
        public void Jump()
        {
            //Nothing to jump OFF of?
            if (_currentMovementMode != MovementMode.Walking)
                return;
            IPhysicsDrivable root = Owner.RootComponent as IPhysicsDrivable;
            if (root == null)
                return;
            PhysicsDriver driver = root.PhysicsDriver;
            driver.SimulatingPhysics = true;
            if (_currentWalkingSurface.SimulatingPhysics)
            {
                //TODO: calculate push off force using ground's mass.
                //For example, you can't jump off a piece of debris.
            }
            else
            {
                //The ground isn't movable, so just apply the jump force directly.

            }
            _currentMovementMode = MovementMode.Falling;
        }
        public void EndJump()
        {

        }
        public void ToggleCrouch()
        {

        }
        public void SetCrouched()
        {

        }
        public void IsSurfaceNormalWalkable(Vec3 normal)
        {

        }
    }
}
