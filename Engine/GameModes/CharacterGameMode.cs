using System;
using TheraEngine.GameModes;
using TheraEngine.Worlds.Actors;
using TheraEngine.Input;
using TheraEngine.Worlds.Actors.Types;
using TheraEngine.Worlds;

namespace TheraEngine.GameModes
{
    public interface ICharacterGameMode : IGameMode
    {
        bool FindSpawnPoint(PawnController c, out Matrix4 transform);
        void OnCharacterKilled(ICharacterPawn killed, ICharacterPawn instigator, IActor killer);
    }
    public class CharacterGameMode<T> : GameMode<T>, ICharacterGameMode where T : class, ICharacterPawn, new()
    {
        public float _respawnTime = 3;

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
                pawn.QueueRespawn();
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
        public virtual void OnCharacterKilled(ICharacterPawn killed, ICharacterPawn instigator, IActor killer)
        {
            killed.QueueRespawn(_respawnTime);
        }
    }
}