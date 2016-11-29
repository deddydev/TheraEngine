using System;
using CustomEngine.Worlds.Actors.Components;

namespace CustomEngine.Worlds.Actors
{
    public class CharacterPawn : Pawn
    {
        public CharacterPawn() : base() { }
        public CharacterPawn(PlayerIndex possessor) : base(possessor) { }

        protected override SceneComponent SetupComponents()
        {
            return new CapsuleComponent();
        }
        protected override void SetDefaults()
        {
            
        }
    }
}
