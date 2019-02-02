using System;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Attributes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;

namespace TheraEngine.Components.Logic.Movement
{
    public enum EMovementMode
    {
        Walking,
        Falling,
        Swimming,
        Flying,
    }
    public class CharacterMovement3DComponent : MovementComponent
    {
        #region Fields
        private EMovementMode _currentMovementMode = EMovementMode.Falling;
        private Vec3 _worldGroundContactPoint;
        private TCollisionObject _currentWalkingSurface;
        private Vec3 _groundNormal;
        private Action<float, Vec3> _subUpdateTick;
        private Vec3 
            _position, _prevPosition,
            _velocity, _prevVelocity,
            _acceleration;

        public Vec3 CurrentPosition => _position;
        public Vec3 CurrentVelocity => _velocity;
        public Vec3 CurrentAcceleration => _acceleration;

        private bool _postWalkAllowJump = false, _justJumped = false;
        private ShapeTraceClosest _closestTrace = new ShapeTraceClosest(
            null, Matrix4.Identity, Matrix4.Identity,
            (ushort)TCollisionGroup.Characters,
            (ushort)(TCollisionGroup.StaticWorld | TCollisionGroup.DynamicWorld));
        #endregion

        #region Properties
        [TNumericPrefixSuffix("", "m")]
        public float VerticalStepUpHeight { get; set; } = 1.0f;
        [TNumericPrefixSuffix("", "°")]
        public float MaxWalkAngle { get; set; } = 50.0f;
        [TNumericPrefixSuffix("", "m/s")]
        public float WalkingMovementSpeed { get; set; } = 0.17f;
        [TNumericPrefixSuffix("", "m/s")]
        public float JumpSpeed { get; set; } = 8.0f;
        [TNumericPrefixSuffix("", "m/s")]
        public float FallingMovementSpeed { get; set; } = 10.0f;
        public bool IsCrouched { get; set; } = false;
        public Quat UpToGroundNormalRotation { get; set; } = Quat.Identity;
        public float AllowJumpTimeDelta { get; set; }
        public float AllowJumpExtraTime { get; set; } = 1.0f;
        public Vec3 GroundNormal
        {
            get => _groundNormal;
            private set
            {
                _groundNormal = value;
                UpToGroundNormalRotation = Quat.BetweenVectors(Vec3.Up, GroundNormal);
            }
        }
        public EMovementMode CurrentMovementMode
        {
            get => _currentMovementMode;
            protected set
            {
                if (_currentMovementMode == value)
                    return;
                if (OwningActor.RootComponent is CapsuleYComponent root)
                {
                    root.RigidBodyCollision.SimulatingPhysics = true;
                    switch (value)
                    {
                        case EMovementMode.Walking:

                            _justJumped = false;
                            //_velocity = root.PhysicsDriver.CollisionObject.LinearVelocity;
                            //root.RigidBodyCollision.SimulatingPhysics = false;
                            root.RigidBodyCollision.IsKinematic = true;
                            //Physics simulation updates the world matrix, but not its components (translation, for example)
                            //Do that now
                            root.Translation = root.WorldPoint;
                            
                            _subUpdateTick = TickWalking;
                            break;
                        case EMovementMode.Falling:

                            if (_postWalkAllowJump = _currentMovementMode == EMovementMode.Walking && !_justJumped)
                            {
                                AllowJumpTimeDelta = 0.0f;
                                _velocity.Y = 0.0f;
                            }
                            
                            root.RigidBodyCollision.IsKinematic = false;
                            root.RigidBodyCollision.LinearVelocity = _velocity;
                            CurrentWalkingSurface = null;

                            _subUpdateTick = TickFalling;
                            break;
                    }
                }
                _currentMovementMode = value;
            }
        }
        public TCollisionObject CurrentWalkingSurface
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
        #endregion

        public override void OnSpawned()
        {
            if (OwningActor.RootComponent is IRigidBodyCollidable root)
            {

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
        private void FloorTransformChanged()
        {
            //TODO: change to falling if ground accelerates down with gravity faster than the character
            SceneComponent comp = (SceneComponent)_currentWalkingSurface.Owner;
            Matrix4 transformDelta = comp.PreviousInverseWorldTransform * comp.WorldMatrix;
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            Matrix4 moved = root.WorldMatrix * comp.PreviousInverseWorldTransform * comp.WorldMatrix;
            Vec3 point = moved.Translation;

            root.Translation = point;
            root.Rotation.Yaw += transformDelta.ExtractRotation(true).ToYawPitchRoll().Yaw;
        }
        private void MainUpdateTick(float delta)
        {
            if (_postWalkAllowJump)
            {
                AllowJumpTimeDelta += delta;
                if (AllowJumpTimeDelta > AllowJumpExtraTime)
                    _postWalkAllowJump = false;
            }
            _subUpdateTick(delta, ConsumeInput());
        }
        protected virtual void TickWalking(float delta, Vec3 movementInput)
        {
            if (OwningActor == null)
                return;

            Matrix4 inputTransform;
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            TCollisionShape shape = root.RenderInfo.CullingVolume.GetCollisionShape();
            TRigidBody body = root.RigidBodyCollision;
            
            _prevPosition = root.Translation.Raw;

            //Use gravity currently affecting this body
            Vec3 gravity = body.Gravity;

            Vec3 down = gravity;
            down.NormalizeFast();
            Vec3 stepUpVector = -VerticalStepUpHeight * down;
            Matrix4 stepUpMatrix = stepUpVector.AsTranslationMatrix();

            //Add input

            Quat groundRot = UpToGroundNormalRotation;

            _closestTrace.Shape = shape;

            #region Movement input
            while (true)
            {
                if (movementInput != Vec3.Zero)
                {
                    Vec3 finalInput = groundRot * (movementInput * WalkingMovementSpeed);
                    groundRot = Quat.Identity;
                    inputTransform = finalInput.AsTranslationMatrix();

                    _closestTrace.Start = stepUpMatrix * root.WorldMatrix;
                    _closestTrace.End = stepUpMatrix * inputTransform * root.WorldMatrix;
                    
                    if (_closestTrace.Trace(OwningActor?.OwningWorld))
                    {
                        if (_closestTrace.HitFraction.IsZero())
                        {
                            Vec3 hitNormal = _closestTrace.HitNormalWorld;
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

                        float hitF = _closestTrace.HitFraction;

                        //Something is in the way
                        root.Translation.Raw += finalInput * hitF;
                        
                        Vec3 normal = _closestTrace.HitNormalWorld;
                        if (IsSurfaceNormalWalkable(normal))
                        {
                            GroundNormal = normal;
                            groundRot = UpToGroundNormalRotation;

                            TRigidBody rigidBody = _closestTrace.CollisionObject as TRigidBody;
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

            float groundTestDist = CurrentWalkingSurface.CollisionShape.Margin + body.CollisionShape.Margin + 0.1f;
            down *= groundTestDist;
            inputTransform = down.AsTranslationMatrix();

            _closestTrace.Start = stepUpMatrix * root.WorldMatrix;
            _closestTrace.End = stepUpMatrix * inputTransform * root.WorldMatrix;

            if (!_closestTrace.Trace(OwningActor?.OwningWorld) || !IsSurfaceNormalWalkable(_closestTrace.HitNormalWorld))
            {
                CurrentMovementMode = EMovementMode.Falling;
                return;
            }

            _worldGroundContactPoint = _closestTrace.HitPointWorld;
            Vec3 diff = Vec3.Lerp(stepUpVector, down, _closestTrace.HitFraction);
            root.Translation.Raw += diff;

            GroundNormal = _closestTrace.HitNormalWorld;
            CurrentWalkingSurface = _closestTrace.CollisionObject as TRigidBody;

            #endregion

            root.RigidBodyCollision.WorldTransform = root.WorldMatrix;

            _prevVelocity = _velocity;
            _position = root.Translation;
            _velocity = (_position - _prevPosition) / delta;
            _acceleration = (_velocity - _prevVelocity) / delta;
        }
        protected virtual void TickFalling(float delta, Vec3 movementInput)
        {
            if (OwningActor == null)
                return;

            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            Vec3 v = root.RigidBodyCollision.LinearVelocity;

            if (v.Xz.LengthFast < 8.667842f)
                root.RigidBodyCollision.ApplyCentralForce((root.RigidBodyCollision.Mass * FallingMovementSpeed) * movementInput);
        }
        public void Jump()
        {
            if (OwningActor == null)
                return;

            //Nothing to jump OFF of?
            if (_currentMovementMode != EMovementMode.Walking && !_postWalkAllowJump)
                return;

            //Get root component of the character
            IRigidBodyCollidable root = OwningActor.RootComponent as IRigidBodyCollidable;
            TRigidBody chara = root?.RigidBodyCollision;

            //If the root has no rigid body, the player can't jump
            if (chara == null)
                return;
            
            _postWalkAllowJump = false;
            _justJumped = true;
            
            Vec3 up = chara.Gravity;
            up.NormalizeFast();
            up = -up;

            if (_postWalkAllowJump = _currentMovementMode == EMovementMode.Walking && !_justJumped)
            {
                AllowJumpTimeDelta = 0.0f;
                _velocity.Y = 0.0f;
            }

            root.RigidBodyCollision.SimulatingPhysics = true;
            _subUpdateTick = TickFalling;

            if (_currentWalkingSurface != null)
                chara.Translate(up * _currentWalkingSurface.CollisionShape.Margin);
            chara.LinearVelocity = _velocity;

            if (_currentWalkingSurface != null && 
                _currentWalkingSurface is TRigidBody rigid &&
                rigid.SimulatingPhysics &&
                rigid.LinearFactor != Vec3.Zero)
            {
                //TODO: calculate push off force using ground's mass.
                //For example, you can't jump off a piece of debris.
                float surfaceMass = rigid.Mass;
                float charaMass = chara.Mass;
                Vec3 surfaceVelInitial = rigid.LinearVelocity;
                Vec3 charaVelInitial = chara.LinearVelocity;

                Vec3 charaVelFinal = up * JumpSpeed;
                Vec3 surfaceVelFinal = (surfaceMass * surfaceVelInitial + charaMass * charaVelInitial - charaMass * charaVelFinal) / surfaceMass;

                Vec3 surfaceImpulse = (surfaceVelFinal - surfaceVelInitial) * surfaceMass;
                rigid.ApplyImpulse(surfaceImpulse, Vec3.TransformPosition(_worldGroundContactPoint, rigid.WorldTransform.Inverted()));
                chara.ApplyCentralImpulse(charaVelFinal * (1.0f / chara.Mass));
            }
            else
            {
                //The ground isn't movable, so just apply the jump force directly.
                //impulse = mass * velocity change
                chara.ApplyCentralImpulse(up * (JumpSpeed * chara.Mass));
            }

            _currentMovementMode = EMovementMode.Falling;
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
            return TMath.AngleBetween(Vec3.Up, normal) <= MaxWalkAngle;
        }
        public void OnHit(TCollisionObject other, TContactInfo point, bool thisIsA)
        {
            Vec3 normal;
            if (thisIsA)
            {
                _worldGroundContactPoint = point.PositionWorldOnB;
                normal = point.NormalWorldOnB;
            }
            else
            {
                _worldGroundContactPoint = point.PositionWorldOnA;
                normal = -point.NormalWorldOnB;
            }
            normal.NormalizeFast();
            if (CurrentMovementMode == EMovementMode.Falling)
            {
                if (IsSurfaceNormalWalkable(normal))
                {
                    CurrentWalkingSurface = other;
                    CurrentMovementMode = EMovementMode.Walking;
                    ((CapsuleYComponent)OwningActor.RootComponent).Translation.Raw += normal * -point.Distance;
                }
            }
            else if (CurrentMovementMode == EMovementMode.Walking)
            {
                //other.Activate();
            }
        }
        public void OnContactEnded(TRigidBody other)
        {

        }
    }
}
