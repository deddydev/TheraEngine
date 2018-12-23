using System;
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
        public CharacterPawn(LocalPlayerIndex possessor)
            : base(possessor) { }
        public CharacterPawn(LocalPlayerIndex possessor, SkeletalModel mesh, Skeleton skeleton)
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
        public CharacterPawn(LocalPlayerIndex possessor)
            : base(false, possessor) { }
        public CharacterPawn(LocalPlayerIndex possessor, SkeletalModel mesh, Skeleton skeleton)
            : base(false, possessor)
        {
            _meshComp.SkeletonOverrideRef = skeleton;
            _meshComp.ModelRef = mesh;
        }

        private GameTimer _respawnTimer = new GameTimer();
        protected SkeletalMeshComponent _meshComp;
        protected MovementClass _movement;
        protected AnimStateMachineComponent _animationStateMachine;
        protected BoomComponent _tpCameraBoom;
        protected CameraComponent _fpCameraComponent, _tpCameraComponent;
        private bool _firstPerson = false;
        private Rotator _viewRotation = Rotator.GetZero(RotationOrder.YPR);
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
            set
            {
                _firstPerson = value;
            }
        }
        public virtual void Kill(ICharacterPawn instigator, IActor killer)
        {
            ICharacterGameMode mode = Engine.World.GetGameMode<ICharacterGameMode>();
            _meshComp.SetAllSimulatingPhysics(true);
            mode.OnCharacterKilled(this, instigator, killer);
        }
        public void QueueRespawn(float respawnTimeInSeconds = 0)
        {
            _respawnTimer.StartSingleFire(WantsRespawn, respawnTimeInSeconds);
        }
        protected virtual void WantsRespawn()
        {
            _respawnTimer.StartMultiFire(AttemptSpawn, 0.0f);
        }
        private void AttemptSpawn(float totalElapsed, int fireNumber)
        {
            ICharacterGameMode mode = Engine.World.GetGameMode<ICharacterGameMode>();
            if (mode.FindSpawnPoint(Controller, out Matrix4 transform))
            {
                _respawnTimer.Stop();

                if (IsSpawned)
                    Engine.World.DespawnActor(this);

                RootComponent.WorldMatrix = transform;
                Engine.World.SpawnActor(this);
            }
        }
        public override void OnSpawnedPostComponentSpawn()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, TickMovementInput);
            //RootComponent.PhysicsDriver.SimulatingPhysics = true;
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, TickMovementInput);
            base.OnDespawned();
        }
        protected void TickMovementInput(float delta)
        {
            Vec3 forward = Vec3.TransformVector(Vec3.Forward, _tpCameraBoom.Rotation.GetYawMatrix());
            Vec3 right = forward ^ Vec3.Up;

            bool keyboardMovement = _keyboardMovementInput.X != 0.0f || _keyboardMovementInput.Y != 0.0f;
            bool gamepadMovement = _gamepadMovementInput.X != 0.0f || _gamepadMovementInput.Y != 0.0f;

            Vec3 input;
            if (keyboardMovement)
            {
                input = forward * _keyboardMovementInput.Y + right * _keyboardMovementInput.X;
                input.NormalizeFast();
                _movement.AddMovementInput(input * delta * _keyboardMovementInputMultiplier);
            }
            if (gamepadMovement)
            {
                input = forward * _gamepadMovementInput.Y + right * _gamepadMovementInput.X;
                _movement.AddMovementInput(input * delta * _gamePadMovementInputMultiplier);
            }
            if (gamepadMovement || keyboardMovement)
            {
                _meshComp.Rotation.Yaw = _movement.FrameInputDirection.LookatAngles().Yaw + 180.0f;
            }
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterAxisUpdate(EGamePadAxis.LeftThumbstickX, MoveRight, true);
            input.RegisterAxisUpdate(EGamePadAxis.LeftThumbstickY, MoveForward, true);
            input.RegisterAxisUpdate(EGamePadAxis.RightThumbstickX, LookRight, true);
            input.RegisterAxisUpdate(EGamePadAxis.RightThumbstickY, LookUp, true);
            input.RegisterButtonEvent(EGamePadButton.FaceDown, EButtonInputType.Pressed, Jump);

            input.RegisterMouseMove(Look, EMouseMoveType.Relative);
            input.RegisterKeyPressed(EKey.W, MoveForward);
            input.RegisterKeyPressed(EKey.A, MoveLeft);
            input.RegisterKeyPressed(EKey.S, MoveBackward);
            input.RegisterKeyPressed(EKey.D, MoveRight);
            input.RegisterKeyEvent(EKey.Space, EButtonInputType.Pressed, Jump);
        }
        
        private void MoveForward(bool pressed)
            => _keyboardMovementInput.Y += pressed ? 1.0f : -1.0f;
        private void MoveLeft(bool pressed)
            => _keyboardMovementInput.X += pressed ? -1.0f : 1.0f;
        private void MoveRight(bool pressed)
            => _keyboardMovementInput.X += pressed ? 1.0f : -1.0f;
        private void MoveBackward(bool pressed)
            => _keyboardMovementInput.Y += pressed ? -1.0f : 1.0f;

        private void Jump()
            => _movement.Jump();
        private void MoveRight(float value)
            => _gamepadMovementInput.X = value;
        private void MoveForward(float value)
            => _gamepadMovementInput.Y = value;

        private void Look(float x, float y)
        {
            _viewRotation.Pitch += y * _mouseYLookInputMultiplier;
            _viewRotation.Yaw -= x * _mouseXLookInputMultiplier;

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
        private void LookRight(float value)
        {
            _viewRotation.Yaw -= value * _gamePadXLookInputMultiplier;
        }
        private void LookUp(float value)
        {
            _viewRotation.Pitch += value * _gamePadYLookInputMultiplier;
        }
        protected override void PreConstruct()
        {
            _movement = Activator.CreateInstance<MovementClass>();
            _animationStateMachine = new AnimStateMachineComponent();
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
                CollisionGroup = (ushort)TCollisionGroup.Characters,
                CollidesWith = (ushort)(TCollisionGroup.StaticWorld | TCollisionGroup.DynamicWorld),
            };

            CapsuleYComponent rootCapsule = new CapsuleYComponent(radius, halfHeight, info);
            rootCapsule.RigidBodyCollision.Collided += RigidBodyCollision_Collided;
            rootCapsule.RigidBodyCollision.AngularFactor = Vec3.Zero;
            rootCapsule.Translation.Raw = new Vec3(0.0f, capsuleTotalHalfHeight + 11.0f, 0.0f);

            _meshComp = new SkeletalMeshComponent();
            _meshComp.Translation.Raw = new Vec3(0.0f, -capsuleTotalHalfHeight, 0.0f);
            rootCapsule.ChildComponents.Add(_meshComp);

            //PerspectiveCamera FPCam = new PerspectiveCamera()
            //{
            //    VerticalFieldOfView = 30.0f,
            //    FarZ = 50.0f
            //};
            //FPCam.LocalRotation.SyncFrom(_viewRotation);
            //_fpCameraComponent = new CameraComponent(FPCam);
            //_fpCameraComponent.AttachTo(_meshComp, "Head");

            PositionLagComponent lagComp = new PositionLagComponent(50.0f, 0.0f);
            rootCapsule.ChildComponents.Add(lagComp);

            _tpCameraBoom = new BoomComponent() { IgnoreCast = rootCapsule.RigidBodyCollision };
            _tpCameraBoom.Translation.Raw = new Vec3(0.0f, 0.3f, 0.0f);
            _tpCameraBoom.Rotation.SyncFrom(_viewRotation);
            _tpCameraBoom.MaxLength = 5.0f;
            lagComp.ChildComponents.Add(_tpCameraBoom);

            PerspectiveCamera TPCam = new PerspectiveCamera()
            {
                NearZ = 0.1f,
                HorizontalFieldOfView = 90.0f,
                //FarZ = 100.0f
            };

            _tpCameraComponent = new CameraComponent(TPCam);
            _tpCameraBoom.ChildComponents.Add(_tpCameraComponent);
            
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
