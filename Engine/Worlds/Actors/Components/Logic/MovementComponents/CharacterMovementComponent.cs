using BulletSharp;
using TheraEngine.Rendering;
using System;
using System.Diagnostics;

namespace TheraEngine.Worlds.Actors
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
        private float _walkingMovementSpeed = 0.17f;
        private float _fallingMovementSpeed = 10.0f;
        private float _jumpSpeed = 8.0f;
        private Vec3 _worldGroundContactPoint;

        private PhysicsDriver _currentWalkingSurface;
        private Vec3 _groundNormal;
        private Quat _upToGroundNormalRotation = Quat.Identity;
        private float _verticalStepUpHeight = 0.5f;
        private Action<float, Vec3> _subUpdateTick;
        Vec3 _position, _prevPosition, _velocity, _prevVelocity, _acceleration;
        private bool _postWalkAllowJump = false, _justJumped = false;
        float _allowJumpExtraTime = 1.0f;
        float _allowJumpTimeDelta;

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
                if (OwningActor.RootComponent is CapsuleComponent root)
                {
                    switch (value)
                    {
                        case MovementMode.Walking:

                            _justJumped = false;
                            //_velocity = root.PhysicsDriver.CollisionObject.LinearVelocity;
                            root.PhysicsDriver.SimulatingPhysics = false;
                            //root.PhysicsDriver.Kinematic = true;
                            //Physics simulation updates the world matrix, but not its components (translation, for example)
                            //Do that now
                            root.Translation = root.GetWorldPoint();
                            
                            _subUpdateTick = TickWalking;
                            break;
                        case MovementMode.Falling:

                            if (_postWalkAllowJump = _currentMovementMode == MovementMode.Walking && !_justJumped)
                            {
                                _allowJumpTimeDelta = 0.0f;
                                _velocity.Y = 0.0f;
                            }

                            //root.PhysicsDriver.Kinematic = false;
                            root.PhysicsDriver.SimulatingPhysics = true;
                            root.PhysicsDriver.CollisionObject.LinearVelocity = _velocity;
                            CurrentWalkingSurface = null;

                            _subUpdateTick = TickFalling;
                            break;
                    }
                }
                _currentMovementMode = value;
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
            //TODO: change to falling if ground accelerates down with gravity faster than the character
            SceneComponent comp = (SceneComponent)_currentWalkingSurface.Owner;
            Matrix4 transformDelta = comp.PreviousInverseWorldTransform * comp.WorldMatrix;
            CapsuleComponent root = OwningActor.RootComponent as CapsuleComponent;
            Matrix4 moved = root.WorldMatrix * comp.PreviousInverseWorldTransform * comp.WorldMatrix;
            Vec3 point = moved.GetPoint();

            root.Translation = point;
            root.Rotation.Yaw += transformDelta.ExtractRotation(true).ToYawPitchRoll().Yaw;
        }

        public float VerticalStepUpHeight
        {
            get => _verticalStepUpHeight;
            set => _verticalStepUpHeight = value;
        }
        public override void OnSpawned()
        {
            if (OwningActor.RootComponent is IPhysicsDrivable root)
            {
                //root.PhysicsDriver.Kinematic = false;
                root.PhysicsDriver.SimulatingPhysics = true;
                root.PhysicsDriver.CollisionObject.LinearVelocity = Vec3.Zero;
            }
            CurrentWalkingSurface = null;
            _subUpdateTick = TickFalling;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Scene, MainUpdateTick);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            _subUpdateTick = null;
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Scene, MainUpdateTick);
            base.OnDespawned();
        }
        private void MainUpdateTick(float delta)
        {
            if (_postWalkAllowJump)
            {
                _allowJumpTimeDelta += delta;
                if (_allowJumpTimeDelta > _allowJumpExtraTime)
                    _postWalkAllowJump = false;
            }
            _subUpdateTick(delta, ConsumeInput());
        }
        protected virtual void TickWalking(float delta, Vec3 movementInput)
        {
            ClosestNotMeConvexResultCallback callback;
            Matrix4 inputTransform;
            CapsuleComponent root = OwningActor.RootComponent as CapsuleComponent;
            ConvexShape shape = (ConvexShape)root.CullingVolume.GetCollisionShape();
            RigidBody body = root.PhysicsDriver.CollisionObject;
            
            _prevPosition = root.Translation.Raw;

            //Use gravity currently affecting this body
            Vec3 gravity = body.Gravity;

            Vec3 down = gravity;
            down.NormalizeFast();
            Vec3 stepUpVector = -_verticalStepUpHeight * down;
            Matrix4 stepUpMatrix = stepUpVector.AsTranslationMatrix();

            //Add input

            Quat groundRot = _upToGroundNormalRotation;

            #region Movement input
            while (true)
            {
                if (movementInput != Vec3.Zero)
                {
                    Vec3 finalInput = groundRot * (movementInput * _walkingMovementSpeed);
                    groundRot = Quat.Identity;
                    inputTransform = finalInput.AsTranslationMatrix();

                    callback = new ClosestNotMeConvexResultCallback(body)
                    {
                        CollisionFilterMask = (CollisionFilterGroups)(short)(CustomCollisionGroup.StaticWorld | CustomCollisionGroup.DynamicWorld),
                        CollisionFilterGroup = (CollisionFilterGroups)(short)CustomCollisionGroup.Characters,
                    };

                    Engine.ShapeCastClosest(shape, stepUpMatrix * root.WorldMatrix, stepUpMatrix * inputTransform * root.WorldMatrix, callback);

                    if (callback.HasHit)
                    {
                        //if (callback.ClosestHitFraction.IsZero())
                        //    break;

                        float hitF = callback.ClosestHitFraction;

                        //Something is in the way
                        root.Translation.Raw += finalInput * hitF;
                        
                        Vec3 normal = callback.HitNormalWorld;
                        if (IsSurfaceNormalWalkable(normal))
                        {
                            GroundNormal = normal;
                            groundRot = _upToGroundNormalRotation;

                            PhysicsDriver d = (PhysicsDriver)callback.HitCollisionObject.UserObject;
                            //if (CurrentWalkingSurface == d)
                            //    break;

                            CurrentWalkingSurface = d;
                            if (!(hitF - 1.0f).IsZero())
                            {
                                float invHitF = 1.0f - hitF;
                                movementInput = movementInput * invHitF;
                                continue;
                            }
                        }
                        else
                        {
                            finalInput.Normalize();
                            float dot = normal | finalInput;
                            if (dot < 0.0f)
                            {
                                //running left is up, right is down
                                Vec3 up = finalInput ^ normal;
                                movementInput = normal ^ up;
                                continue;
                            }
                        }
                    }
                    else
                        root.Translation.Raw += finalInput;
                }
                break;
            }
            #endregion

            //Test for walkable ground

            #region Ground Test
            float groundTestDist = CurrentWalkingSurface.CollisionObject.CollisionShape.Margin + body.CollisionShape.Margin + 0.01f;
            down *= groundTestDist;
            inputTransform = down.AsTranslationMatrix();
            callback = new ClosestNotMeConvexResultCallback(body)
            {
                CollisionFilterMask = (CollisionFilterGroups)(short)(CustomCollisionGroup.StaticWorld | CustomCollisionGroup.DynamicWorld),
                CollisionFilterGroup = (CollisionFilterGroups)(short)CustomCollisionGroup.Characters,
            };

            Engine.ShapeCastClosest(shape, stepUpMatrix * root.WorldMatrix, inputTransform * root.WorldMatrix, callback);

            if (!callback.HasHit || !IsSurfaceNormalWalkable(callback.HitNormalWorld))
            {
                CurrentMovementMode = MovementMode.Falling;
                return;
            }

            _worldGroundContactPoint = callback.HitPointWorld;
            Vec3 diff = Vec3.Lerp(stepUpVector, down, callback.ClosestHitFraction);
            //if (diff.Length > 1.232488E-06f)
            //    return;
            root.Translation.Raw += diff;

            GroundNormal = callback.HitNormalWorld;
            CurrentWalkingSurface = callback.HitCollisionObject.UserObject as PhysicsDriver;

            #endregion

            root.PhysicsDriver.SetPhysicsTransform(root.WorldMatrix);

            _prevVelocity = _velocity;
            _position = root.Translation;
            _velocity = (_position - _prevPosition) / delta;
            _acceleration = (_velocity - _prevVelocity) / delta;

            //Engine.DebugPrint(_velocity.Xz.LengthFast);

            body.LinearVelocity = _velocity;
        }
        protected virtual void TickFalling(float delta, Vec3 movementInput)
        {
            CapsuleComponent root = OwningActor.RootComponent as CapsuleComponent;
            Vec3 v = root.PhysicsDriver.CollisionObject.LinearVelocity;
            //Engine.DebugPrint(v.Xz.LengthFast);
            if (v.Xz.LengthFast < 8.667842f)
                root.PhysicsDriver.CollisionObject.ApplyCentralForce(((1.0f / root.PhysicsDriver.CollisionObject.InvMass) * _fallingMovementSpeed) * movementInput);
        }
        
        public void Jump()
        {
            //Nothing to jump OFF of?
            if (_currentMovementMode != MovementMode.Walking && !_postWalkAllowJump)
                return;

            //Get root component of the character
            IPhysicsDrivable root = OwningActor.RootComponent as IPhysicsDrivable;

            //If the root isn't physics drivable, the player can't jump
            if (root == null)
                return;
            
            _postWalkAllowJump = false;
            _justJumped = true;

            //Start physics simulation of the root
            PhysicsDriver driver = root.PhysicsDriver;
            RigidBody character = driver.CollisionObject;

            Vec3 up = driver.CollisionObject.Gravity;
            up.NormalizeFast();
            up = -up;

            if (_postWalkAllowJump = _currentMovementMode == MovementMode.Walking && !_justJumped)
            {
                _allowJumpTimeDelta = 0.0f;
                _velocity.Y = 0.0f;
            }
            //root.PhysicsDriver.Kinematic = false;
            root.PhysicsDriver.SimulatingPhysics = true;
            _subUpdateTick = TickFalling;

            if (_currentWalkingSurface != null)
                character.Translate(up * _currentWalkingSurface.CollisionObject.CollisionShape.Margin);
            character.LinearVelocity = _velocity;

            if (_currentWalkingSurface != null && 
                _currentWalkingSurface.SimulatingPhysics && 
                _currentWalkingSurface.LinearFactor != Vec3.Zero)
            {
                //TODO: calculate push off force using ground's mass.
                //For example, you can't jump off a piece of debris.
                RigidBody surface = _currentWalkingSurface.CollisionObject;

                float surfaceMass = 1.0f / surface.InvMass;
                float characterMass = 1.0f / character.InvMass;
                Vec3 surfaceVelInitial = surface.LinearVelocity;
                Vec3 charaVelInitial = character.LinearVelocity;

                Vec3 charaVelFinal = up * _jumpSpeed;
                Vec3 surfaceVelFinal = (surfaceMass * surfaceVelInitial + characterMass * charaVelInitial - characterMass * charaVelFinal) / surfaceMass;

                Vec3 surfaceImpulse = (surfaceVelFinal - surfaceVelInitial) * surfaceMass;
                surface.ApplyImpulse(surfaceImpulse, Vector3.TransformCoordinate(_worldGroundContactPoint, Matrix.Invert(surface.WorldTransform)));
                character.ApplyCentralImpulse(charaVelFinal * (1.0f / driver.CollisionObject.InvMass));
            }
            else
            {
                //The ground isn't movable, so just apply the jump force directly.
                character.ApplyCentralImpulse(up * (_jumpSpeed * (1.0f / driver.CollisionObject.InvMass)));
            }

            _currentMovementMode = MovementMode.Falling;
            CurrentWalkingSurface = null;
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
            //A is the ground, B is the character
            _worldGroundContactPoint = point.PositionWorldOnA;
            Vec3 normal = -point.NormalWorldOnB;
            normal.NormalizeFast();
            if (CurrentMovementMode == MovementMode.Falling)
            {
                if (IsSurfaceNormalWalkable(normal))
                {
                    CurrentWalkingSurface = other.PhysicsDriver;
                    CurrentMovementMode = MovementMode.Walking;
                    ((CapsuleComponent)OwningActor.RootComponent).Translation.Raw += normal * -point.Distance;
                }
            }
            else if (CurrentMovementMode == MovementMode.Walking)
            {
                other.PhysicsDriver.CollisionObject.Activate();
            }
        }
        public void OnContactEnded(IPhysicsDrivable other)
        {

        }
    }
}
