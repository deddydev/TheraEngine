﻿using System;
using TheraEngine.Components.Logic.Animation;
using TheraEngine.Components.Logic.Movement;
using TheraEngine.Components.Scene;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.GameModes;
using TheraEngine.Input.Devices;
using TheraEngine.Physics;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Timers;

namespace TheraEngine.Actors.Types.Pawns
{
    /// <summary>
    /// Use this interface for interaction with pawns.
    /// </summary>
    public interface ICharacterPawn : IPawn
    {
        void QueueRespawn(float respawnTimeInSeconds = 0);
    }
    /// <summary>
    /// Use this character pawn type for default functionality.
    /// DO NOT USE IN GENERIC CODE! Use ICharacterPawn
    /// </summary>
    public class CharacterPawn : CharacterPawn<CharacterMovement3DComponent>
    {
        public CharacterPawn()
            : base() { }
        public CharacterPawn(ELocalPlayerIndex possessor)
            : base(possessor) { }
        public CharacterPawn(ELocalPlayerIndex possessor, SkeletalModel mesh, Skeleton skeleton)
            : base(possessor, mesh, skeleton) { }
    }
    /// <summary>
    /// Use this character pawn type to specify your own derivation of the CharacterMovementComponent.
    /// </summary>
    /// <typeparam name="MovementClass"></typeparam>
    public class CharacterPawn<MovementClass> : Pawn<CapsuleYComponent>, ICharacterPawn
        where MovementClass : CharacterMovement3DComponent
    {
        public CharacterPawn()
            : base() { }
        public CharacterPawn(ELocalPlayerIndex possessor)
            : base(false, possessor) { }
        public CharacterPawn(ELocalPlayerIndex possessor, SkeletalModel mesh, Skeleton skeleton)
            : base(false, possessor)
        {
            _meshComp.SkeletonOverrideRef = skeleton;
            _meshComp.ModelRef = mesh;

            _viewRotation.Changed += _viewRotation_Changed;
        }

        private void _viewRotation_Changed()
        {
            _tpCameraBoom.Rotation = _viewRotation.ToQuaternion();
        }

        private GameTimer _respawnTimer = new GameTimer();
        protected SkeletalMeshComponent _meshComp;
        protected MovementClass _movement;
        protected AnimStateMachineComponent _animationStateMachine;
        protected BoomComponent _tpCameraBoom;
        protected CameraComponent _fpCameraComponent, _tpCameraComponent;
        private bool _firstPerson = false;
        private Rotator _viewRotation = Rotator.GetZero(ERotationOrder.YPR);
        float _gamePadMovementInputMultiplier = 51.0f;
        float _keyboardMovementInputMultiplier = 51.0f;
        float _mouseXLookInputMultiplier = 0.5f;
        float _mouseYLookInputMultiplier = 0.5f;
        float _gamePadXLookInputMultiplier = 1.0f;
        float _gamePadYLookInputMultiplier = 1.0f;

        protected Vec2 _keyboardMovementInput = Vec2.Zero;
        protected Vec2 _gamepadMovementInput = Vec2.Zero;
        
        public bool FirstPerson
        {
            get => _firstPerson;
            set => Set(ref _firstPerson, value);
        }
        public float KeyboardMovementInputMultiplier 
        {
            get => _keyboardMovementInputMultiplier; 
            set => Set(ref _keyboardMovementInputMultiplier, value);
        }
        public float GamePadMovementInputMultiplier 
        { 
            get => _gamePadMovementInputMultiplier;
            set => Set(ref _gamePadMovementInputMultiplier, value);
        }
        public float MouseXLookInputMultiplier
        {
            get => _mouseXLookInputMultiplier;
            set => Set(ref _mouseXLookInputMultiplier, value);
        }
        public float MouseYLookInputMultiplier 
        {
            get => _mouseYLookInputMultiplier; 
            set => Set(ref _mouseYLookInputMultiplier, value);
        }
        public float GamePadXLookInputMultiplier
        {
            get => _gamePadXLookInputMultiplier;
            set => Set(ref _gamePadXLookInputMultiplier, value);
        }
        public float GamePadYLookInputMultiplier 
        {
            get => _gamePadYLookInputMultiplier;
            set => Set(ref _gamePadYLookInputMultiplier, value);
        }

        public virtual void Kill(ICharacterPawn instigator, IActor killer)
        {
            ICharacterGameMode mode = OwningWorld?.GameMode as ICharacterGameMode;
            _meshComp.SetAllSimulatingPhysics(true);
            mode?.OnCharacterKilled(this, instigator, killer);
        }
        public void QueueRespawn(float respawnTimeInSeconds = 0)
            => _respawnTimer.StartSingleFire(WantsRespawn, respawnTimeInSeconds);
        protected virtual void WantsRespawn()
            => _respawnTimer.StartMultiFire(AttemptSpawn, 0.1f);
        private void AttemptSpawn(float totalElapsed, int fireNumber)
        {
            ICharacterGameMode mode = OwningWorld?.GameMode as ICharacterGameMode;
            if (!mode.FindSpawnPoint(Controller, out Matrix4 transform))
                return;
            
            _respawnTimer.Stop();

            if (IsSpawned)
                Engine.World.DespawnActor(this);

            RootComponent.WorldMatrix.Value = transform;
            Engine.World.SpawnActor(this);
        }
        protected override void OnSpawnedPostComponentSpawn()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, TickMovementInput);
            //RootComponent.PhysicsDriver.SimulatingPhysics = true;
        }
        protected override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, TickMovementInput);
            base.OnDespawned();
        }
        protected virtual void TickMovementInput(float delta)
        {
            Vec3 forward = _tpCameraBoom.Transform.GetForwardVector();
            Vec3 right = forward ^ Vec3.Up;

            bool keyboardMovement = _keyboardMovementInput.X != 0.0f || _keyboardMovementInput.Y != 0.0f;
            bool gamepadMovement = _gamepadMovementInput.X != 0.0f || _gamepadMovementInput.Y != 0.0f;

            Vec3 input;
            if (keyboardMovement)
            {
                input = forward * _keyboardMovementInput.Y + right * _keyboardMovementInput.X;
                input.NormalizeFast();
                _movement.AddMovementInput(input * delta * KeyboardMovementInputMultiplier);
            }
            if (gamepadMovement)
            {
                input = forward * _gamepadMovementInput.Y + right * _gamepadMovementInput.X;
                _movement.AddMovementInput(input * delta * GamePadMovementInputMultiplier);
            }
            //if (gamepadMovement || keyboardMovement)
            //    _meshComp.Rotation.Yaw = _movement.TargetFrameInputDirection.LookatAngles().Yaw + 180.0f;
        }
        public override void RegisterInput(InputInterface input)
        {
            //input.Mouse.WrapCursorWithinClip = input.IsRegistering;
            input.RegisterMouseMove(Look, EMouseMoveType.Relative);

            input.RegisterAxisUpdate(EGamePadAxis.LeftThumbstickX, MoveRight, true);
            input.RegisterAxisUpdate(EGamePadAxis.LeftThumbstickY, MoveForward, true);

            input.RegisterAxisUpdate(EGamePadAxis.RightThumbstickX, LookRight, true);
            input.RegisterAxisUpdate(EGamePadAxis.RightThumbstickY, LookUp, true);

            input.RegisterButtonEvent(EGamePadButton.FaceDown, EButtonInputType.Pressed, Jump);

            input.RegisterKeyPressed(EKey.W, MoveForward);
            input.RegisterKeyPressed(EKey.A, MoveLeft);
            input.RegisterKeyPressed(EKey.S, MoveBackward);
            input.RegisterKeyPressed(EKey.D, MoveRight);

            input.RegisterKeyEvent(EKey.Space, EButtonInputType.Pressed, Jump);
        }
        
        protected virtual void MoveForward(bool pressed)
            => _keyboardMovementInput.Y += pressed ? 1.0f : -1.0f;
        protected virtual void MoveLeft(bool pressed)
            => _keyboardMovementInput.X += pressed ? -1.0f : 1.0f;
        protected virtual void MoveRight(bool pressed)
            => _keyboardMovementInput.X += pressed ? 1.0f : -1.0f;
        protected virtual void MoveBackward(bool pressed)
            => _keyboardMovementInput.Y += pressed ? -1.0f : 1.0f;

        protected virtual void Jump()
            => _movement.Jump();
        protected virtual void MoveRight(float value)
            => _gamepadMovementInput.X = value;
        protected virtual void MoveForward(float value)
            => _gamepadMovementInput.Y = value;

        protected virtual void Look(float x, float y)
        {
            _viewRotation.Pitch += y * MouseYLookInputMultiplier;
            _viewRotation.Yaw -= x * MouseXLookInputMultiplier;

            //float yaw = _viewRotation.Yaw.RemapToRange(0.0f, 360.0f);
            //if (yaw < 45.0f || yaw >= 315.0f)
            //{
            //    _meshComp.Rotation.Yaw = 180.0f;
            //}
            //else if (yaw < 135.0f)
            //{
            //    _meshComp.Rotation.Yaw = 270.0f;
            //}
            //else if (yaw < 225.0f)
            //{
            //    _meshComp.Rotation.Yaw = 0.0f;
            //}
            //else if (yaw < 315.0f)
            //{
            //    _meshComp.Rotation.Yaw = 90.0f;
            //}

            //_fpCameraComponent.Camera.AddRotation(y, 0.0f);
        }
        protected virtual void LookRight(float value)
        {
            _viewRotation.Yaw -= value * GamePadXLookInputMultiplier;
        }
        protected virtual void LookUp(float value)
        {
            _viewRotation.Pitch += value * GamePadYLookInputMultiplier;
        }
        protected override void PreConstruct()
        {
            _movement = Activator.CreateInstance<MovementClass>();
            _animationStateMachine = new AnimStateMachineComponent();
            LogicComponents.Clear();
            LogicComponents.Add(_movement);
            LogicComponents.Add(_animationStateMachine);
        }
        protected override CapsuleYComponent OnConstructRoot()
        {
            //5'8" in m = 1.72f
            float characterHeight = new FeetInches(5, 8.0f).ToMeters();

            float radius = 0.172f;
            float capsuleTotalHalfHeight = characterHeight / 2.0f;
            float halfHeight = capsuleTotalHalfHeight - radius;

            TRigidBodyConstructionInfo info = new TRigidBodyConstructionInfo()
            {
                Mass = 59.0f,
                AdditionalDamping = false,
                AngularDamping = 0.0f,
                LinearDamping = 0.0f,
                Restitution = 0.0f,
                Friction = 1.0f,
                RollingFriction = 0.01f,
                CollisionEnabled = true,
                SimulatePhysics = true,
                SleepingEnabled = false,
                CollisionGroup = (ushort)ETheraCollisionGroup.Characters,
                CollidesWith = (ushort)(ETheraCollisionGroup.StaticWorld | ETheraCollisionGroup.DynamicWorld),
            };

            CapsuleYComponent rootCapsule = new CapsuleYComponent(radius, halfHeight, info);
            TRigidBody body = rootCapsule.CollisionObject as TRigidBody;
            body.Collided += RigidBodyCollision_Collided;
            body.AngularFactor = Vec3.Zero;
            rootCapsule.Translation = new Vec3(0.0f, capsuleTotalHalfHeight + 11.0f, 0.0f);

            _meshComp = new SkeletalMeshComponent();
            _meshComp.Translation = new Vec3(0.0f, -capsuleTotalHalfHeight, 0.0f);
            rootCapsule.ChildSockets.Add(_meshComp);

            //PerspectiveCamera FPCam = new PerspectiveCamera()
            //{
            //    VerticalFieldOfView = 30.0f,
            //    FarZ = 50.0f
            //};
            //FPCam.LocalRotation.SyncFrom(_viewRotation);
            //_fpCameraComponent = new CameraComponent(FPCam);
            //_fpCameraComponent.AttachTo(_meshComp, "Head");

            PositionLagComponent lagComp = new PositionLagComponent(50.0f, 0.0f);
            rootCapsule.ChildSockets.Add(lagComp);

            _tpCameraBoom = new BoomComponent() { IgnoreCast = rootCapsule.CollisionObject };
            _tpCameraBoom.Translation = new Vec3(0.0f, 0.3f, 0.0f);
            //_tpCameraBoom.Rotation.SyncFrom(_viewRotation);
            _tpCameraBoom.MaxLength = 5.0f;
            lagComp.ChildSockets.Add(_tpCameraBoom);

            PerspectiveCamera TPCam = new PerspectiveCamera()
            {
                NearZ = 0.1f,
                HorizontalFieldOfView = 90.0f,
                //FarZ = 100.0f
            };

            _tpCameraComponent = new CameraComponent(TPCam);
            _tpCameraBoom.ChildSockets.Add(_tpCameraComponent);
            
            CurrentCameraComponent = _tpCameraComponent;

            _viewRotation.Yaw = 180.0f;

            return rootCapsule;
        }

        private void RigidBodyCollision_Collided(TCollisionObject @this, TCollisionObject other, TContactInfo info, bool thisIsA)
        {
            //Engine.DebugPrint(((ObjectBase)other).Name + " collided with " + Name);
            _movement.OnHit(other, info, thisIsA);
        }
    }
}
