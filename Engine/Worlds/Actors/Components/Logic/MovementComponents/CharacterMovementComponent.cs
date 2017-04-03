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
        private Vec3 _jumpVelocity = new Vec3(0.0f, 10.0f, 0.0f);
        private PhysicsDriver _currentWalkingSurface;
        private Vec3 _groundNormal;
        private Quat _upToGroupNormalRotation = Quat.Identity;

        public Vec3 GroundNormal
        {
            get => _groundNormal;
            set
            {
                _groundNormal = value;
                _upToGroupNormalRotation = Quat.BetweenVectors(Vec3.Up, GroundNormal);
            }
        }
        public override Vec3 ConsumeInput()
            => _upToGroupNormalRotation * base.ConsumeInput();
        public void Jump()
        {
            //Nothing to jump OFF of?
            if (_currentMovementMode != MovementMode.Walking)
                return;
            //Get root component of the character
            IPhysicsDrivable root = Owner.RootComponent as IPhysicsDrivable;
            //If the root isn't physics drivable, the player can't jump
            if (root == null)
                return;
            //Start physics simulation of the root
            PhysicsDriver driver = root.PhysicsDriver;
            driver.SimulatingPhysics = true;
            RigidBody character = driver.CollisionObject;
            if (_currentWalkingSurface.SimulatingPhysics)
            {
                //TODO: calculate push off force using ground's mass.
                //For example, you can't jump off a piece of debris.
                RigidBody surface = _currentWalkingSurface.CollisionObject;
                float surfaceMass = 1.0f / surface.InvMass;
                float characterMass = 1.0f / character.InvMass;

            }
            else
            {
                //The ground isn't movable, so just apply the jump force directly.
                character.ApplyForce(_jumpVelocity, Vec3.Zero);
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
        public bool IsSurfaceNormalWalkable(Vec3 normal)
        {
            //TODO: use friction between surfaces, not just a constant angle
            return CustomMath.AngleBetween(Vec3.Up, normal) <= _maxWalkAngle;
        }
    }
}
