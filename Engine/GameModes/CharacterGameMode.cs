using System;
using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using CustomEngine.Input;

namespace CustomEngine.Tests
{
    public class CharacterGameMode<T> : GameMode<T> where T : class, ICharacterPawn, new()
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
                ICharacterPawn pawn = _pawnClass.CreateNew();
                c.ControlledPawn = pawn;
                Engine.World.SpawnActor(pawn, FindSpawnPoint());
            }
        }
        public virtual Matrix4 FindSpawnPoint()
        {
            return Matrix4.Identity;
        }
        public override void EndGameplay()
        {
            throw new NotImplementedException();
        }
        public virtual void OnKilled(CharacterPawn pawn)
        {

        }
    }
}