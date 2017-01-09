using System;
using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors
{
    public class CharacterPawn : Pawn
    {
        public CharacterPawn() : base() { }
        public CharacterPawn(PlayerIndex possessor) : base(possessor) { }

        protected override SceneComponent SetupComponents()
        {
            PhysicsDriverInfo info = new PhysicsDriverInfo();
            CapsuleComponent root = new CapsuleComponent(0.2f, 0.8f, info);
            SkeletalMeshComponent mesh = new SkeletalMeshComponent();
            CameraComponent firstPersonCamera = new CameraComponent();
            firstPersonCamera.AttachTo(mesh, "HeadCameraSocket");

            CharacterMovementComponent movement = new CharacterMovementComponent();
            LogicComponents.Add(movement);

            return root;
        }
        protected override void SetDefaults()
        {
            
        }
    }
}
