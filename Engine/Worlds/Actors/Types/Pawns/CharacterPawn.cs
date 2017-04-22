using System;
using CustomEngine.Worlds.Actors;
using CustomEngine.Rendering;
using CustomEngine.GameModes;
using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using System.ComponentModel;
using System.Activities.Statements;
using CustomEngine.Input.Devices;
using CustomEngine.Rendering.Cameras;
using System.Diagnostics;
using CustomEngine.Input;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Worlds.Actors
{
    public struct ZoomLevel
    {
        public ZoomLevel(float fovY, float aimAssistDistance)
        {
            _fovY = fovY;
            _aimAssistDistance = aimAssistDistance;
        }

        private float _fovY;
        private float _aimAssistDistance;

        public float FovY
        {
            get => _fovY;
            set => _fovY = value;
        }
        public float Distance
        {
            get => _aimAssistDistance;
            set => _aimAssistDistance = value;
        }

        public static ZoomLevel DefaultNonZoomed => new ZoomLevel(78.0f, 1000.0f);
        public static ZoomLevel DefaultZoomed => new ZoomLevel(45.0f, 2000.0f);
    }
    /// <summary>
    /// Use this interface for interaction with pawns.
    /// </summary>
    public interface ICharacterPawn : IActor
    {

    }
    /// <summary>
    /// Use this character pawn type for default functionality.
    /// DO NOT USE IN GENERIC CODE! Use ICharacterPawn
    /// </summary>
    public class CharacterPawn : CharacterPawn<CharacterMovementComponent>
    {
        public CharacterPawn()
            : base() { }
        public CharacterPawn(PlayerIndex possessor)
            : base(possessor) { }
        public CharacterPawn(PlayerIndex possessor, SkeletalMesh mesh, Skeleton skeleton)
            : base(possessor, mesh, skeleton) { }
    }
    /// <summary>
    /// Use this character pawn type to specify your own derivation of the CharacterMovementComponent.
    /// </summary>
    /// <typeparam name="MovementClass"></typeparam>
    public class CharacterPawn<MovementClass> : Pawn<CapsuleComponent>, ICharacterPawn
        where MovementClass : CharacterMovementComponent
    {
        public CharacterPawn()
            : base() { }
        public CharacterPawn(PlayerIndex possessor)
            : base(false, possessor) { }
        public CharacterPawn(PlayerIndex possessor, SkeletalMesh mesh, Skeleton skeleton)
            : base(true, possessor)
        {
            _skeleton = skeleton;
            _mesh = mesh;
            Initialize();
        }

        private SkeletalMeshComponent _meshComp;
        private MovementClass _movement;
        private SingleFileRef<SkeletalMesh> _mesh;
        private SingleFileRef<Skeleton> _skeleton;
        private AnimStateMachineComponent _animationStateMachine;
        private BoomComponent _tpCameraBoom;
        private CameraComponent _fpCameraComponent, _tpCameraComponent;
        private bool _firstPerson = false;
        private Rotator _viewRotation;
        private Vec2 _keyboardMovementInput = Vec2.Zero;
        private Vec2 _gamepadMovementInput = Vec2.Zero;
        
        [Category("Reference Files")]
        public SingleFileRef<SkeletalMesh> Mesh
        {
            get => _mesh;
            set => _mesh = value;
        }
        [Category("Reference Files")]
        public SingleFileRef<Skeleton> Skeleton
        {
            get => _skeleton;
            set => _skeleton = value;
        }
        public bool FirstPerson
        {
            get => _firstPerson;
            set
            {
                _firstPerson = value;
            }
        }
        public override void OnSpawned(World world)
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Input);
            base.OnSpawned(world);
        }
        public override void OnDespawned()
        {
            UnregisterTick();
            base.OnDespawned();
        }
        public override void OnPossessed(PawnController possessor)
        {
            base.OnPossessed(possessor);
        }
        public override void OnUnPossessed()
        {
            base.OnUnPossessed();
        }
        protected internal override void Tick(float delta)
        {
            base.Tick(delta);
            Vec3 forward = Vec3.TransformVector(Vec3.Forward, _tpCameraBoom.Rotation.GetYawMatrix());
            Vec3 right = forward ^ Vec3.Up;
            if (_keyboardMovementInput.X != 0.0f || _keyboardMovementInput.Y != 0.0f)
            {
                Vec3 finalInput = forward * _keyboardMovementInput.Y + right * _keyboardMovementInput.X;
                finalInput.Y = 0.0f;
                _movement.AddMovementInput(finalInput);
            }
            if (_gamepadMovementInput.X != 0.0f || _gamepadMovementInput.Y != 0.0f)
            {
                Vec3 finalInput = forward * _gamepadMovementInput.Y + right * _gamepadMovementInput.X;
                finalInput.Y = 0.0f;
                finalInput *= delta;
                _movement.AddMovementInput(finalInput);
            }
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, MoveRight, false);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, MoveForward, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickX, LookRight, false);
            input.RegisterAxisUpdate(GamePadAxis.RightThumbstickY, LookUp, false);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, Jump);

            input.RegisterMouseMove(Look, true);
            input.RegisterButtonPressed(EKey.W, MoveForward);
            input.RegisterButtonPressed(EKey.A, MoveLeft);
            input.RegisterButtonPressed(EKey.S, MoveBackward);
            input.RegisterButtonPressed(EKey.D, MoveRight);
            input.RegisterButtonEvent(EKey.Space, ButtonInputType.Pressed, Jump);
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
            //RootComponent.Rotation.Yaw += x;
            _tpCameraBoom.Rotation.Pitch -= y;
            _tpCameraBoom.Rotation.Yaw -= x;
            float yaw = _tpCameraBoom.Rotation.Yaw.RemapToRange(0.0f, 360.0f);
            if (yaw < 45.0f || yaw >= 315.0f)
            {
                _meshComp.Rotation.Yaw = 180.0f;
            }
            else if (yaw < 135.0f)
            {
                _meshComp.Rotation.Yaw = 270.0f;
            }
            else if (yaw < 225.0f)
            {
                _meshComp.Rotation.Yaw = 0.0f;
            }
            else if (yaw < 315.0f)
            {
                _meshComp.Rotation.Yaw = 90.0f;
            }
            //_fpCameraComponent.Camera.AddRotation(y, 0.0f);
        }
        private void LookRight(float value)
        {
            //_tpCameraBoom.Yaw += value;
            RootComponent.Rotation.Yaw += value * Engine.RenderDelta;
            //CurrentCameraComponent.Camera.AddRotation(0.0f, value);
        }
        private void LookUp(float value)
        {
            value *= Engine.RenderDelta;
            _tpCameraBoom.Rotation.Pitch += value;
            //_fpCameraComponent.Camera.AddRotation(value, 0.0f);
            //RootComponent.Rotation.Yaw += value;
            //CurrentCameraComponent.Camera.AddRotation(value, 0.0f);
        }
        protected void OnHit(IPhysicsDrivable other, ManifoldPoint point)
        {
            Debug.WriteLine(((ObjectBase)other).Name + " collided with " + Name);
            _movement.OnHit(other, point);
        }
        protected override void PreConstruct()
        {
            _movement = Activator.CreateInstance<MovementClass>();
            LogicComponents.Add(_movement);
            LogicComponents.Add(_animationStateMachine = new AnimStateMachineComponent());
        }
        protected override CapsuleComponent OnConstruct()
        {
            float characterHeight = 1.72f; //5'8" in m = 1.72f
            float radius = 0.172f;
            float capsuleTotalHalfHeight = characterHeight / 2.0f;
            float halfHeight = capsuleTotalHalfHeight - radius;

            PhysicsConstructionInfo info = new PhysicsConstructionInfo()
            {
                Mass = 14.0f,
                AngularDamping = 0.05f,
                LinearDamping = 0.005f,
                Restitution = 0.9f,
                Friction = 0.01f,
                RollingFriction = 0.01f,
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.Characters,
                CollidesWith = CustomCollisionGroup.StaticWorld,
            };

            CapsuleComponent rootCapsule = new CapsuleComponent(radius, halfHeight, info);
            rootCapsule.PhysicsDriver.OnHit += _movement.OnHit;
            rootCapsule.PhysicsDriver.AngularFactor = Vec3.Zero;
            rootCapsule.Translation.Raw = new Vec3(0.0f, capsuleTotalHalfHeight + 11.0f, 0.0f);

            _meshComp = new SkeletalMeshComponent(_mesh, _skeleton);
            _meshComp.Translation.Raw = new Vec3(0.0f, -capsuleTotalHalfHeight, 0.0f);
            rootCapsule.ChildComponents.Add(_meshComp);

            //PerspectiveCamera FPCam = new PerspectiveCamera();
            //FPCam.VerticalFieldOfView = 30.0f;
            //FPCam.FarZ = 50.0f;
            //_fpCameraComponent = new CameraComponent(FPCam);
            //_fpCameraComponent.AttachTo(mesh, "Head");

            _tpCameraBoom = new BoomComponent();
            _tpCameraBoom.Translation.Raw = new Vec3(0.4f, 0.2f, 0.0f);
            _tpCameraBoom.Rotation.Yaw = 180.0f;
            _tpCameraBoom.MaxLength = 2.0f;
            rootCapsule.ChildComponents.Add(_tpCameraBoom);

            PerspectiveCamera TPCam = new PerspectiveCamera()
            {
                //VerticalFieldOfView = 70.0f,
                //FarZ = 100.0f
            };

            _tpCameraComponent = new CameraComponent(TPCam);
            _tpCameraBoom.ChildComponents.Add(_tpCameraComponent);
            
            rootCapsule.PhysicsDriver.SimulatingPhysics = true;
            //CurrentCameraComponent = _tpCameraComponent;

            return rootCapsule;
        }

        public virtual void Kill(ICharacterPawn instigator, IActor killer)
        {
            GameMode mode = Engine.World.Settings.GameMode;
            
        }
    }
}
