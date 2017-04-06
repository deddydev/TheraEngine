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
        private Quat _upToGroundNormalRotation = Quat.Identity;
        private float _verticalStepUpHeight = 10.0f;
        private DelTick _tick;

        public Vec3 GroundNormal
        {
            get => _groundNormal;
            private set
            {
                _groundNormal = value;
                _upToGroundNormalRotation = Quat.BetweenVectors(Vec3.Up, GroundNormal);
            }
        }
        public MovementMode CurrentMovementMode
        {
            get => _currentMovementMode;
            protected set
            {
                if (_currentMovementMode == value)
                    return;
                _currentMovementMode = value;
                switch (_currentMovementMode)
                {
                    case MovementMode.Walking:
                        if (Owner.RootComponent is IPhysicsDrivable root)
                            root.PhysicsDriver.SimulatingPhysics = false;
                        _tick = TickWalking;
                        break;
                }
            }
        }
        public PhysicsDriver CurrentWalkingSurface
        {
            get => _currentWalkingSurface;
            set => _currentWalkingSurface = value;
        }
        public float VerticalStepUpHeight
        {
            get => _verticalStepUpHeight;
            set => _verticalStepUpHeight = value;
        }

        protected void TickWalking(float delta)
        {
            Vec3 down = Engine.World.Settings.State.Gravity;
            Vec3 movementInput = ConsumeInput();
            Vec3 normalAdjustedInput = _upToGroundNormalRotation * movementInput;
            Vec3 finalInput = normalAdjustedInput + new Vec3(0.0f, _verticalStepUpHeight, 0.0f);

            CapsuleComponent root = Owner.RootComponent as CapsuleComponent;
            ClosestConvexResultCallback callback = new ClosestConvexResultCallback();
            BaseCapsule c = (BaseCapsule)root.CullingVolume;

            ////Vec3 downScale = new Vec3(c.Radius * 2.0f, (c.HalfHeight + c.Radius) * 2.0f, c.Radius * 2.0f);
            //Vec3 downScale = new Vec3(5.0f);

            ConvexShape shape = (ConvexShape)c.GetCollisionShape();

            Matrix4 inputTransform = Matrix4.CreateTranslation(finalInput);
            Engine.World.PhysicsScene.ConvexSweepTest(shape, root.WorldMatrix, root.WorldMatrix * inputTransform, callback);

            if (callback.HasHit)
            {
                //Something is in the way
                Vec3 normal = callback.HitNormalWorld;
                if (IsSurfaceNormalWalkable(normal))
                {
                    GroundNormal = callback.HitNormalWorld;
                    _currentWalkingSurface = (PhysicsDriver)callback.HitCollisionObject.UserObject;
                }
                root.Translation.Raw += finalInput * callback.ClosestHitFraction;
            }
            else
                root.Translation.Raw += finalInput;

            inputTransform = Matrix4.CreateTranslation(down);
            callback = new ClosestConvexResultCallback();
            Engine.World.PhysicsScene.ConvexSweepTest(shape, root.WorldMatrix, root.WorldMatrix * inputTransform, callback);

            if (!callback.HasHit)
            {
                CurrentMovementMode = MovementMode.Falling;
                _currentWalkingSurface = null;
            }
        }
        protected void TickFalling(float delta)
        {
            CapsuleComponent root = Owner.RootComponent as CapsuleComponent;
            root.Translation.Raw += ConsumeInput() * 0.1f;
        }
        private delegate void DelTick(float delta);
        protected internal override void Tick(float delta)
        {
            _tick(delta);
        }
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
            }
        }
        public override void OnSpawned()
        {
            _tick = TickFalling;
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _tick = null;
            UnregisterTick();
            base.OnDespawned();
        }
    }
}
