using System;
using System.Collections.Generic;
using TheraEngine.Actors;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.ComponentModel;
using TheraEngine.Input;

namespace TheraEngine.GameModes
{
    public interface ICharacterGameMode : IGameMode
    {
        bool FindSpawnPoint(PawnController c, out Matrix4 transform);
        void OnCharacterKilled(ICharacterPawn killed, ICharacterPawn instigator, IActor killer);
    }
    public class CharacterPlayerController : LocalPlayerController
    {
        public CharacterPlayerController(ELocalPlayerIndex index)
            : base(index) { }
        public CharacterPlayerController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) { }

        [TSerialize]
        public float RespawnTime { get; set; } = 3;
    }
    public class CharacterGameMode<PawnType, ControllerType> 
        : GameMode<PawnType, ControllerType>, ICharacterGameMode 
        where PawnType : BaseActor, ICharacterPawn, new()
        where ControllerType : CharacterPlayerController
    {
        public CharacterGameMode() : base() { }
        
        public virtual bool FindSpawnPoint(PawnController c, out Matrix4 transform)
        {
            var spawned = Engine.World.StateRef.File.SpawnedActors;
            foreach (IActor a in spawned)
                if (a is CharacterSpawnPointActor p && p.CanSpawnPlayer(c))
                {
                    transform = p.RootComponent.WorldMatrix;
                    return true;
                }

            transform = Matrix4.Identity;
            return false;
        }
        public virtual void OnCharacterKilled(ICharacterPawn killed, ICharacterPawn instigator, IActor killer)
            => killed.QueueRespawn(((CharacterPlayerController)killed.LocalPlayerController).RespawnTime);
    }
}