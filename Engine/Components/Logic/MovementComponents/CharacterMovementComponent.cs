using Extensions;
using System;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Attributes;
using TheraEngine.Core.Maths;
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
            (ushort)ETheraCollisionGroup.Characters,
            (ushort)(ETheraCollisionGroup.StaticWorld | ETheraCollisionGroup.DynamicWorld));
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
        public bool AlignInputToGround { get; set; } = true;
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
                    TRigidBody body = root.CollisionObject as TRigidBody;
                    body.SimulatingPhysics = true;
                    switch (value)
                    {
                        case EMovementMode.Walking:

                            _justJumped = false;
                            //_velocity = root.PhysicsDriver.CollisionObject.LinearVelocity;
                            //body.SimulatingPhysics = false;
                            body.IsKinematic = true;
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

                            body.IsKinematic = false;
                            body.LinearVelocity = _velocity;

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

        protected override void OnSpawned()
        {
            if (OwningActor.RootComponent is IRigidBodyCollidable root)
            {

            }
            CurrentWalkingSurface = null;
            _subUpdateTick = TickFalling;
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input, MainUpdateTick);
            base.OnSpawned();
        }
        protected override void OnDespawned()
        {
            _subUpdateTick = null;
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Input, MainUpdateTick);
            base.OnDespawned();
        }
        private void FloorTransformChanged(ISceneComponent floor)
        {
            //TODO: change to falling if ground accelerates down with gravity faster than the character
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            ISceneComponent comp = (ISceneComponent)_currentWalkingSurface.Owner;
            //Matrix4 transformDelta = comp.PreviousInverseWorldMatrix * comp.WorldMatrix;
            root.WorldMatrix = root.WorldMatrix * comp.PreviousInverseWorldMatrix * comp.WorldMatrix;
            //Vec3 point = newWorldMatrix.Translation;

            //root.Translation = point;
            //root.Rotation.Yaw += transformDelta.ExtractRotation(true).ToYawPitchRoll().Yaw;
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
            if (OwningActor is null)
                return;

            Matrix4 inputTransform;
            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;
            TCollisionShape shape = root.RenderInfo.CullingVolume.GetCollisionShape();
            TRigidBody body = root.CollisionObject as TRigidBody;
            
            _prevPosition = root.Translation.Value;

            //Use gravity currently affecting this body
            Vec3 gravity = body.Gravity;

            Vec3 down = gravity;
            down.NormalizeFast();
            Vec3 stepUpVector = -VerticalStepUpHeight * down;
            Matrix4 stepUpMatrix = stepUpVector.AsTranslationMatrix();

            //Add input

            Quat groundRot = AlignInputToGround ? UpToGroundNormalRotation : Quat.Identity;

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
                        root.Translation.Value += finalInput * hitF;
                        
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
                        root.Translation.Value += finalInput;
                }
                break;
            }
            #endregion

            //Test for walkable ground

            #region Ground Test

            float centerToGroundDist = root.Shape.HalfHeight + root.Shape.Radius;
            float marginDist = (CurrentWalkingSurface?.CollisionShape?.Margin ?? 0.0f) + (body?.CollisionShape?.Margin ?? 0.0f);
            float groundTestDist = 2.0f;// centerToGroundDist + marginDist;
            down *= groundTestDist;
            inputTransform = down.AsTranslationMatrix();

            _closestTrace.Start = stepUpMatrix * root.WorldMatrix;
            _closestTrace.End = stepUpMatrix * inputTransform * root.WorldMatrix;

            bool traceSuccess = _closestTrace.Trace(OwningActor?.OwningWorld);
            bool walkSuccess = traceSuccess && IsSurfaceNormalWalkable(_closestTrace.HitNormalWorld);
            if (!walkSuccess)
            {
                //Engine.Out(traceSuccess ? "walk surface failed" : "walk trace failed");

                CurrentMovementMode = EMovementMode.Falling;
                return;
            }

            _worldGroundContactPoint = _closestTrace.HitPointWorld;
            //Vec3 diff = Vec3.Lerp(stepUpVector, down, _closestTrace.HitFraction);
            root.Translation.Y = _worldGroundContactPoint.Y + centerToGroundDist;

            GroundNormal = _closestTrace.HitNormalWorld;
            CurrentWalkingSurface = _closestTrace.CollisionObject as TRigidBody;

            #endregion

            root.CollisionObject.WorldTransform = root.WorldMatrix;

            _prevVelocity = _velocity;
            _position = root.Translation;
            _velocity = (_position - _prevPosition) / delta;
            _acceleration = (_velocity - _prevVelocity) / delta;
        }
        protected virtual void TickFalling(float delta, Vec3 movementInput)
        {
            if (OwningActor is null)
                return;

            CapsuleYComponent root = OwningActor.RootComponent as CapsuleYComponent;

            if (root?.CollisionObject is TRigidBody body)
            {
                Vec3 v = body.LinearVelocity;
                if (v.Xz.LengthFast < 8.667842f)
                    body.ApplyCentralForce((body.Mass * FallingMovementSpeed) * movementInput);
            }
        }
        public void Jump()
        {
            if (OwningActor is null)
                return;

            //Nothing to jump OFF of?
            if (_currentMovementMode != EMovementMode.Walking && !_postWalkAllowJump)
                return;

            //Get root component of the character
            IGenericCollidable root = OwningActor.RootComponent as IGenericCollidable;

            //If the root has no rigid body, the player can't jump
            if (!(root?.CollisionObject is TRigidBody chara))
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

            chara.SimulatingPhysics = true;
            _subUpdateTick = TickFalling;

            if (_currentWalkingSurface != null)
                chara.Translate(up * 2.0f);

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

            CurrentMovementMode = EMovementMode.Falling;
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
                    ((CapsuleYComponent)OwningActor.RootComponent).Translation.Value += normal * -point.Distance;
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
