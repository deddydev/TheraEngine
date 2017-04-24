using System;
using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using CustomEngine.Input;

namespace CustomEngine.Tests
{
    public class CharacterGameMode : GameMode<CharacterPawn>
    {
        public CharacterGameMode()
        {

        }

        public override void AbortGameplay()
        {
            
        }

        public override void BeginGameplay()
        {
            foreach (LocalPlayerController c in Engine.ActivePlayers)
            {
                CharacterPawn pawn = _pawnClass.CreateNew();
                c.ControlledPawn = pawn;
                Engine.World.SpawnActor(pawn, FindSpawnPoint());
            }
        }
        public Matrix4 FindSpawnPoint()
        {
            return Matrix4.Identity;
        }
        public override void EndGameplay()
        {
            throw new NotImplementedException();
        }
        public void OnKilled(CharacterPawn pawn)
        {

        }
    }
}