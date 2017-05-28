using System;
using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using CustomEngine.Input;
using CustomEngine.Worlds.Actors.Types;
using CustomEngine.Worlds;

namespace CustomEngine.GameModes
{
    public interface ICharacterGameMode
    {

    }
    public class CharacterGameMode<T> : GameMode<T>, ICharacterGameMode where T : class, ICharacterPawn, new()
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
                T pawn = _pawnClass.CreateNew();
                c.ControlledPawn = pawn;
                c.AwaitRespawn();
            }
        }
        public virtual bool FindSpawnPoint(PawnController c, out Matrix4 transform)
        {
            foreach (IActor a in Engine.World.State.SpawnedActors)
                if (a is CharacterSpawnPointActor p && p.CanSpawnPlayer(c))
                {
                    transform = p.RootComponent.WorldMatrix;
                    return true;
                }
            transform = Matrix4.Identity;
            return false;
        }
        public override void EndGameplay()
        {
            throw new NotImplementedException();
        }
        public virtual void OnKilled(T pawn)
        {

        }
    }
}