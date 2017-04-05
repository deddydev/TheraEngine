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
            private set
            {
                _groundNormal = value;
                _upToGroupNormalRotation = Quat.BetweenVectors(Vec3.Up, GroundNormal);
            }
        }
        public MovementMode CurrentMovementMode
        {
            get => _currentMovementMode;
            set
            {
                if (_currentMovementMode == value)
                    return;
                _currentMovementMode = value;
                if (_currentMovementMode == MovementMode.Walking)
                    RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
                else
                    UnregisterTick();
            }
        }
        public PhysicsDriver CurrentWalkingSurface
        {
            get => _currentWalkingSurface;
            set => _currentWalkingSurface = value;
        }
        protected internal override void Tick(float delta)
        {
            CapsuleComponent root = Owner.RootComponent as CapsuleComponent;
            ClosestConvexResultCallback callback = new ClosestConvexResultCallback();
            BaseCapsule c = (BaseCapsule)root.CullingVolume;
            Vec3 down = Engine.World.Settings.State.Gravity.NormalizedFast();
            //Vec3 downScale = new Vec3(c.Radius * 2.0f, (c.HalfHeight + c.Radius) * 2.0f, c.Radius * 2.0f);
            Vec3 downScale = new Vec3(5.0f);
            Matrix4 downTransform = Matrix4.CreateTranslation(down * downScale);
            Engine.World.PhysicsScene.ConvexSweepTest((ConvexShape)c.GetCollisionShape(), root.WorldMatrix, root.WorldMatrix * downTransform, callback);
            if (callback.HasHit)
            {
                GroundNormal = callback.HitNormalWorld;
                _currentWalkingSurface = (PhysicsDriver)callback.HitCollisionObject.UserObject;
                root.WorldMatrix = Matrix4.CreateTranslation(callback.ConvexFromWorld.Lerp(callback.ConvexToWorld, callback.ClosestHitFraction));
            }
            else
            {
                CurrentMovementMode = MovementMode.Falling;
                _currentWalkingSurface = null;
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
        public void OnHit(IPhysicsDrivable other, ManifoldPoint point)
        {
            if (CurrentMovementMode == MovementMode.Falling && 
                IsSurfaceNormalWalkable(-point.NormalWorldOnB))
            {
                _currentWalkingSurface = other.PhysicsDriver;
                CurrentMovementMode = MovementMode.Walking;

                if (Owner.RootComponent is IPhysicsDrivable root)
                {
                    root.PhysicsDriver.SimulatingPhysics = false;
                }
            }
        }
    }
}
