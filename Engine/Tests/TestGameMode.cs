using System;
using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using CustomEngine.Input;

namespace CustomEngine.Tests
{
    public class TestGameMode : GameMode<CharacterPawn>
    {
        public TestGameMode()
        {

        }
        public override void BeginGameplay()
        {
            foreach (LocalPlayerController c in Engine.ActivePlayers)
            {
                CharacterPawn pawn = _pawnClass.CreateNew();
                c.ControlledPawn = pawn;
                Engine.World.SpawnActor(pawn, pawn.FindSpawnPoint());
            }
        }
    }
}