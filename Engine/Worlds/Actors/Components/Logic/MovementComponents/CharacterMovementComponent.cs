using BulletSharp;
using CustomEngine.Rendering;
using System;
using System.Diagnostics;

namespace CustomEngine.Worlds.Actors
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
        private float _walkingMovementSpeed = 0.2f;
        private float _fallingMovementSpeed = 1.0f;
        
        private float _jumpSpeed = 3.0f;

        private PhysicsDriver _currentWalkingSurface;
        private Vec3 _groundNormal;
        private Quat _upToGroundNormalRotation = Quat.Identity;
        private float _verticalStepUpHeight = 0.0f;
        private DelTick _subUpdateTick;

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
                        if (OwningActor.RootComponent is CapsuleComponent root1)
                        {
                            root1.PhysicsDriver.SimulatingPhysics = false;
                            root1.Translation = root1.WorldMatrix.GetPoint();
                        }
                        _subUpdateTick = TickWalking;
                        break;
                    case MovementMode.Falling:
                        if (OwningActor.RootComponent is CapsuleComponent root2)
                        {
                            root2.PhysicsDriver.SimulatingPhysics = true;
                            root2.PhysicsDriver.CollisionObject.LinearVelocity = _velocity;
                        }
                        _subUpdateTick = TickFalling;
                        break;
                }
            }
        }
        public PhysicsDriver CurrentWalkingSurface
        {
            get => _currentWalkingSurface;
            set
            {
                if (_currentWalkingSurface != null)
                    ((SceneComponent)_currentWalkingSurface.Owner).WorldTransformChanged -= FloorTransformChanged;
                _currentWalkingSurface = value;
                if (_currentWalkingSurface != null)
                    ((SceneComponent)_currentWalkingSurface.Owner).WorldTransformChanged += FloorTransformChanged;
            }
        }

        private void FloorTransformChanged()
        {
            SceneComponent comp = (SceneComponent)_currentWalkingSurface.Owner;
            Matrix4 transformDelta = comp.PreviousInverseWorldTransform * comp.WorldMatrix;
            CapsuleComponent root = OwningActor.RootComponent as CapsuleComponent;
            root.Translation += transformDelta.GetPoint();
        }

        public float VerticalStepUpHeight
        {
            get => _verticalStepUpHeight;
            set => _verticalStepUpHeight = value;
        }

        Vec3 _position, _prevPosition, _velocity, _prevVelocity, _acceleration;

        private void MainUpdateTick(float delta) => _subUpdateTick(delta);
        protected virtual void TickWalking(float delta)
        {
            ClosestConvexResultCallback callback;
            Matrix4 inputTransform;
            CapsuleComponent root = OwningActor.RootComponent as CapsuleComponent;
            BaseCapsule c = (BaseCapsule)root.CullingVolume;
            ConvexShape shape = (ConvexShape)c.GetCollisionShape();

            _prevPosition = root.Translation;

            //Use gravity currently affecting this body, not global gravity
            Vec3 gravity = root.PhysicsDriver.CollisionObject.Gravity; //Engine.World.Settings.State.Gravity

            Vec3 down = gravity.NormalizedFast();
            Vec3 stepUpVector = _verticalStepUpHeight * -down;
            Matrix4 stepUpMatrix = Matrix4.CreateTranslation(stepUpVector);
            Matrix4 invStepUpMatrix = Matrix4.CreateTranslation(-stepUpVector);

            //Test for walkable ground first
            inputTransform = Matrix4.CreateTranslation(down);
            callback = new ClosestConvexResultCallback()
            {
                CollisionFilterMask = CollisionFilterGroups.AllFilter,
                CollisionFilterGroup = CollisionFilterGroups.AllFilter
            };

            Engine.World.PhysicsScene.ConvexSweepTest(
                shape, root.WorldMatrix, inputTransform * root.WorldMatrix, callback, 1.0f);

            _prevVelocity = _velocity;

            _position = root.Translation;
            _velocity = (_position - _prevPosition) / delta;

            if (!callback.HasHit)
            {
                CurrentMovementMode = MovementMode.Falling;
                _currentWalkingSurface = null;
                return;
            }
            else
            {
                //root.Translation += -down * callback.ClosestHitFraction;
            }

            //Now add input
            Vec3 movementInput = ConsumeInput();
            Top:
            if (movementInput != Vec3.Zero)
            {
                Vec3 finalInput = _upToGroundNormalRotation * (movementInput * _walkingMovementSpeed);
                inputTransform = Matrix4.CreateTranslation(finalInput);
                
                callback = new ClosestConvexResultCallback()
                {
                    CollisionFilterMask = CollisionFilterGroups.AllFilter,
                    CollisionFilterGroup = CollisionFilterGroups.AllFilter
                };

                Engine.World.PhysicsScene.ConvexSweepTest(shape, stepUpMatrix * root.WorldMatrix, stepUpMatrix * inputTransform * root.WorldMatrix, callback, 1.0f);

                if (callback.HasHit)
                {
                    //Something is in the way
                    root.Translation.Raw += finalInput * callback.ClosestHitFraction;

                    Vec3 normal = callback.HitNormalWorld;
                    if (IsSurfaceNormalWalkable(normal))
                    {
                        GroundNormal = callback.HitNormalWorld;
                        _currentWalkingSurface = (PhysicsDriver)callback.HitCollisionObject.UserObject;

                        if (callback.ClosestHitFraction < 0.95f)
                        {
                            movementInput = finalInput * (1.0f - callback.ClosestHitFraction);
                            goto Top;
                        }
                    }
                }
                else
                    root.Translation.Raw += finalInput;

                root.PhysicsDriver.SetPhysicsTransform(root.WorldMatrix);

                //Debug.WriteLine(_velocity);
            }

            _acceleration = (_velocity - _prevVelocity) / delta;
        }
        protected virtual void TickFalling(float delta)
        {
            //CapsuleComponent root = Owner.RootComponent as CapsuleComponent;
            //root.PhysicsDriver.SetPhysicsTransform(ConsumeInput().AsTranslationMatrix() * root.WorldMatrix);
            //root.Translation.Raw += ConsumeInput() * _fallingMovementSpeed;
        }
        
        public void Jump()
        {
            //Nothing to jump OFF of?
            if (_currentMovementMode != MovementMode.Walking)
                return;
            //Get root component of the character
            IPhysicsDrivable root = OwningActor.RootComponent as IPhysicsDrivable;
            //If the root isn't physics drivable, the player can't jump
            if (root == null)
                return;
            //Start physics simulation of the root
            CurrentMovementMode = MovementMode.Falling;
            PhysicsDriver driver = root.PhysicsDriver;
            RigidBody character = driver.CollisionObject;
            if (_currentWalkingSurface.SimulatingPhysics && 
                _currentWalkingSurface.LinearFactor != Vec3.Zero)
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
                character.LinearVelocity = _velocity + (-(Vec3)driver.CollisionObject.Gravity).NormalizedFast() * _jumpSpeed;
            }
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
            _subUpdateTick = TickFalling;
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, MainUpdateTick);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _subUpdateTick = null;
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, MainUpdateTick);
            base.OnDespawned();
        }
    }
}
