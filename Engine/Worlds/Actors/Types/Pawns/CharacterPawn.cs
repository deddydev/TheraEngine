﻿using System;
using CustomEngine.Worlds.Actors.Components;
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

        private float
            _forward = 0.0f,
            _left = 0.0f,
            _right = 0.0f,
            _backward = 0.0f;
        private MovementClass _movement;
        private SingleFileRef<SkeletalMesh> _mesh;
        private SingleFileRef<Skeleton> _skeleton;
        private StateMachine _animationStateMachine = new StateMachine();
        private BoomComponent _tpCameraBoom;
        private CameraComponent _fpCameraComponent, _tpCameraComponent;
        private bool _firstPerson = false;
        private Rotator _viewRotation;

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
        internal override void Tick(float delta)
        {
            Vec3 movementInput = _movement.ConsumeInput();
            RootComponent.Translation.Raw += movementInput;
            base.Tick(delta);
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
        }

        private void MoveForward(bool pressed)
            => _forward = pressed ? 1.0f : 0.0f;
        private void MoveLeft(bool pressed)
            => _left = pressed ? 1.0f : 0.0f;
        private void MoveRight(bool pressed)
            => _right = pressed ? 1.0f : 0.0f;
        private void MoveBackward(bool pressed)
            => _backward = pressed ? 1.0f : 0.0f;
        
        private void Look(float x, float y)
        {
            RootComponent.Rotation.Yaw += x;
            _tpCameraBoom.Rotation.Pitch += y;
            //_fpCameraComponent.Camera.AddRotation(y, 0.0f);
        }
        private void Jump() => _movement.Jump();
        private void MoveRight(float value)
        {
            Vec3 dir = CurrentCameraComponent.Camera.GetRightVector();
            _movement.AddMovementInput(dir * value * Engine.RenderDelta);
        }
        private void MoveForward(float value)
        {
            Rotator r = CurrentCameraComponent.Camera.Rotation;
            r.Pitch = 0;
            Vec3 dir = r.TransformVector(Vec3.Forward);
            _movement.AddMovementInput(dir * value * Engine.RenderDelta);
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
        private void OnHit(IPhysicsDrivable other, ManifoldPoint point)
        {
            Debug.WriteLine(((ObjectBase)other).Name + " collided with " + Name);
        }
        protected override CapsuleComponent OnConstruct()
        {
            PhysicsDriverInfo info = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(
                    50.0f,
                    new DefaultMotionState(),
                    null)
                {
                    AngularDamping = 0.05f,
                    LinearDamping = 0.005f,
                    Restitution = 0.9f,
                    Friction = 0.01f,
                    RollingFriction = 0.01f,
                },
                CollisionEnabled = true,
                SimulatePhysics = false,
                Group = CustomCollisionGroup.Characters,
                CollidesWith = CustomCollisionGroup.StaticWorld,
            };

            CapsuleComponent rootCapsule = new CapsuleComponent(10.0f, 50.0f, info);
            rootCapsule.PhysicsDriver.OnHit += OnHit;
            rootCapsule.Translation.Raw = new Vec3(0.0f, 300.0f, 0.0f);

            SkeletalMeshComponent mesh = new SkeletalMeshComponent(_mesh, _skeleton);
            rootCapsule.ChildComponents.Add(mesh);

            //PerspectiveCamera FPCam = new PerspectiveCamera();
            //FPCam.VerticalFieldOfView = 30.0f;
            //FPCam.FarZ = 50.0f;
            //_fpCameraComponent = new CameraComponent(FPCam);
            //_fpCameraComponent.AttachTo(mesh, "Head");

            _tpCameraBoom = new BoomComponent();
            _tpCameraBoom.Translation.Raw = new Vec3(0.0f, 100.0f, 0.0f);
            rootCapsule.ChildComponents.Add(_tpCameraBoom);

            PerspectiveCamera TPCam = new PerspectiveCamera()
            {
                VerticalFieldOfView = 70.0f,
                FarZ = 100.0f
            };
            _tpCameraComponent = new CameraComponent(TPCam);
            _tpCameraBoom.ChildComponents.Add(_tpCameraComponent);

            _movement = Activator.CreateInstance<MovementClass>();
            LogicComponents.Add(_movement);
            
            rootCapsule.PhysicsDriver.SimulatingPhysics = true;
            CurrentCameraComponent = _tpCameraComponent;

            return rootCapsule;
        }
        
        protected override void PreConstruct()
        {
            
        }

        public virtual void Kill(ICharacterPawn instigator, IActor killer)
        {
            GameMode mode = Engine.World.State.GameMode;
        }
    }
}
