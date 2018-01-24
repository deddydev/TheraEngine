using System;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Components.Scene.Shapes;

namespace TheraEngine.Components.Logic.Movement
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

        private TRigidBody _currentWalkingSurface;
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
                if (OwningActor.RootComponent is CapsuleYComponent root)
                {
                    switch (value)
                    {
                        case MovementMode.Walking:

                            _justJumped = false;
                            //_velocity = root.PhysicsDriver.CollisionObject.LinearVelocity;
                            root.RigidBodyCollision.SimulatingPhysics = false;
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
                            root.RigidBodyCollision.SimulatingPhysics = true;
                            root.RigidBodyCollision.LinearVelocity = _velocity;
                            CurrentWalkingSurface = null;

                            _subUpdateTick = TickFalling;
                            break;
                    }
                }
                _currentMovementMode = value;
            }
        }
        public TRigidBody CurrentWalkingSurface
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
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
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
            if (OwningActor.RootComponent is IRigidCollidable root)
            {
                //root.RigidBodyCollision.IsKinematic = true;
                root.RigidBodyCollision.SimulatingPhysics = true;
                root.RigidBodyCollision.SleepingEnabled = false;
                root.RigidBodyCollision.CollisionEnabled = true;
                root.RigidBodyCollision.LinearVelocity = Vec3.Zero;
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
            ShapeTraceClosest result;
            Matrix4 inputTransform;
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            TCollisionShape shape = root.CullingVolume.GetCollisionShape();
            TRigidBody body = root.RigidBodyCollision;
            
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

                    result = new ShapeTraceClosest(shape,
                        stepUpMatrix * root.WorldMatrix, stepUpMatrix * inputTransform * root.WorldMatrix,
                        (ushort)TCollisionGroup.Characters,
                        (ushort)(TCollisionGroup.StaticWorld | TCollisionGroup.DynamicWorld));
                    
                    if (Engine.ShapeTrace(result))
                    {
                        if (result.HitFraction.IsZero())
                        {
                            Vec3 hitNormal = result.HitNormalWorld;
                            finalInput.Normalize();
                            float dot = hitNormal | finalInput;
                            if (dot < 0.0f)
                            {
                                //running left is up, right is down
                                Vec3 up = finalInput ^ hitNormal;
                                Vec3 newMovement = hitNormal ^ up;
                                if (!newMovement.Equals(movementInput, 0.0001f))
                                {
                                    movementInput = newMovement;
                                    continue;
                                }
                            }
                            break;
                        }

                        float hitF = result.HitFraction;

                        //Something is in the way
                        root.Translation.Raw += finalInput * hitF;
                        
                        Vec3 normal = result.HitNormalWorld;
                        if (IsSurfaceNormalWalkable(normal))
                        {
                            GroundNormal = normal;
                            groundRot = _upToGroundNormalRotation;

                            TRigidBody rigidBody = result.CollisionObject as TRigidBody;
                            //if (CurrentWalkingSurface == d)
                            //    break;

                            CurrentWalkingSurface = rigidBody;
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
            float groundTestDist = CurrentWalkingSurface.CollisionShape.Margin + body.CollisionShape.Margin + 0.01f;
            down *= groundTestDist;
            inputTransform = down.AsTranslationMatrix();
            
            result = new ShapeTraceClosest(shape,
                stepUpMatrix * root.WorldMatrix, stepUpMatrix * inputTransform * root.WorldMatrix, 
                (ushort)TCollisionGroup.Characters, 
                (ushort)(TCollisionGroup.StaticWorld | TCollisionGroup.DynamicWorld));
            
            if (!Engine.ShapeTrace(result) || !IsSurfaceNormalWalkable(result.HitNormalWorld))
            {
                CurrentMovementMode = MovementMode.Falling;
                return;
            }

            _worldGroundContactPoint = result.HitPointWorld;
            Vec3 diff = Vec3.Lerp(stepUpVector, down, result.HitFraction);
            //if (diff.Length > 1.232488E-06f)
            //    return;
            root.Translation.Raw += diff;

            GroundNormal = result.HitNormalWorld;
            CurrentWalkingSurface = result.CollisionObject as TRigidBody;

            #endregion

            root.RigidBodyCollision.WorldTransform = root.WorldMatrix;

            _prevVelocity = _velocity;
            _position = root.Translation;
            _velocity = (_position - _prevPosition) / delta;
            _acceleration = (_velocity - _prevVelocity) / delta;

            //Engine.DebugPrint(_velocity.Xz.LengthFast);

            body.LinearVelocity = _velocity;
        }
        protected virtual void TickFalling(float delta, Vec3 movementInput)
        {
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            Vec3 v = root.RigidBodyCollision.LinearVelocity;
            //Engine.DebugPrint(v.Xz.LengthFast);
            if (v.Xz.LengthFast < 8.667842f)
                root.RigidBodyCollision.ApplyCentralForce((root.RigidBodyCollision.Mass * _fallingMovementSpeed) * movementInput);
        }
        
        public void Jump()
        {
            //Nothing to jump OFF of?
            if (_currentMovementMode != MovementMode.Walking && !_postWalkAllowJump)
                return;

            //Get root component of the character
            IRigidCollidable root = OwningActor.RootComponent as IRigidCollidable;
            TRigidBody chara = root?.RigidBodyCollision;

            //If the root has no rigid body, the player can't jump
            if (chara == null)
                return;
            
            _postWalkAllowJump = false;
            _justJumped = true;
            
            Vec3 up = chara.Gravity;
            up.NormalizeFast();
            up = -up;

            if (_postWalkAllowJump = _currentMovementMode == MovementMode.Walking && !_justJumped)
            {
                _allowJumpTimeDelta = 0.0f;
                _velocity.Y = 0.0f;
            }
            //root.PhysicsDriver.Kinematic = false;
            root.RigidBodyCollision.SimulatingPhysics = true;
            _subUpdateTick = TickFalling;

            if (_currentWalkingSurface != null)
                chara.Translate(up * _currentWalkingSurface.CollisionShape.Margin);
            chara.LinearVelocity = _velocity;

            if (_currentWalkingSurface != null && 
                _currentWalkingSurface.SimulatingPhysics && 
                _currentWalkingSurface.LinearFactor != Vec3.Zero)
            {
                //TODO: calculate push off force using ground's mass.
                //For example, you can't jump off a piece of debris.
                TRigidBody surface = _currentWalkingSurface;

                float surfaceMass = surface.Mass;
                float charaMass = chara.Mass;
                Vec3 surfaceVelInitial = surface.LinearVelocity;
                Vec3 charaVelInitial = chara.LinearVelocity;

                Vec3 charaVelFinal = up * _jumpSpeed;
                Vec3 surfaceVelFinal = (surfaceMass * surfaceVelInitial + charaMass * charaVelInitial - charaMass * charaVelFinal) / surfaceMass;

                Vec3 surfaceImpulse = (surfaceVelFinal - surfaceVelInitial) * surfaceMass;
                surface.ApplyImpulse(surfaceImpulse, Vec3.TransformPosition(_worldGroundContactPoint, surface.WorldTransform.Inverted()));
                chara.ApplyCentralImpulse(charaVelFinal * (1.0f / chara.Mass));
            }
            else
            {
                //The ground isn't movable, so just apply the jump force directly.
                chara.ApplyCentralImpulse(up * (_jumpSpeed * chara.Mass));
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
            return TMath.AngleBetween(Vec3.Up, normal) <= _maxWalkAngle;
        }
        public void OnHit(TRigidBody other, TCollisionInfo point)
        {
            //A is the ground, B is the character
            _worldGroundContactPoint = point.PositionWorldOnA;
            Vec3 normal = -point.NormalWorldOnB;
            normal.NormalizeFast();
            if (CurrentMovementMode == MovementMode.Falling)
            {
                if (IsSurfaceNormalWalkable(normal))
                {
                    CurrentWalkingSurface = other;
                    CurrentMovementMode = MovementMode.Walking;
                    ((CapsuleYComponent)OwningActor.RootComponent).Translation.Raw += normal * -point.Distance;
                }
            }
            else if (CurrentMovementMode == MovementMode.Walking)
            {
                //other.Activate();
            }
        }
        public void OnContactEnded(TRigidBody other)
        {

        }
    }
}
