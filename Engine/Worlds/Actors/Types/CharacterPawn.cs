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
            return new CapsuleComponent(0.2f, 0.8f, info);
        }
        protected override void SetDefaults()
        {
            
        }
    }
}
