using System;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Rendering;
using CustomEngine.GameModes;
using BulletSharp;
using CustomEngine.Files;
using CustomEngine.Rendering.Models;
using System.ComponentModel;
using System.Activities.Statements;

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
    public class CharacterPawn : Pawn<CapsuleComponent>
    {
        public CharacterPawn() : base() { }
        public CharacterPawn(PlayerIndex possessor) : base(possessor) { }

        private MultiFileRef<SkeletalMesh> mesh;
        StateMachine _animationStateMachine = new StateMachine();
        
        [Category("")]
        public MultiFileRef<SkeletalMesh> Mesh
        {
            get => mesh;
            set => mesh = value;
        }

        protected override CapsuleComponent OnConstruct()
        {
            PhysicsDriverInfo info = new PhysicsDriverInfo()
            {
                BodyInfo = new RigidBodyConstructionInfo(50.0f, new DefaultMotionState(), null),
            };
            CapsuleComponent root = new CapsuleComponent(0.2f, 0.8f, info);
            SkeletalMeshComponent mesh = new SkeletalMeshComponent();
            root.ChildComponents.Add(mesh);
            CameraComponent firstPersonCamera = new CameraComponent();
            firstPersonCamera.AttachTo(mesh, "HeadCameraSocket");
            BoomComponent cameraBoom = new BoomComponent();
            root.ChildComponents.Add(cameraBoom);

            CharacterMovementComponent movement = new CharacterMovementComponent();
            LogicComponents.Add(movement);

            return root;
        }
        protected override void SetDefaults()
        {
            
        }

        public virtual void Kill(CharacterPawn instigator, IActor killer)
        {
            GameMode mode = Engine.World.State.GameMode;
        }
    }
}
