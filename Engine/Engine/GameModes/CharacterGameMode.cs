﻿using System;
using TheraEngine.Actors;
using TheraEngine.Input;
using TheraEngine.Actors.Types;
using TheraEngine.Actors.Types.Pawns;

namespace TheraEngine.GameModes
{
    public interface ICharacterGameMode : IGameMode
    {
        bool FindSpawnPoint(PawnController c, out Matrix4 transform);
        void OnCharacterKilled(ICharacterPawn killed, ICharacterPawn instigator, IActor killer);
    }
    public class CharacterGameMode<PawnType, ControllerType> : GameMode<PawnType, ControllerType>, ICharacterGameMode 
        where PawnType : class, ICharacterPawn, new()
        where ControllerType : LocalPlayerController
    {
        private float _respawnTime = 3;

        public float RespawnTime
        {
            get => _respawnTime;
            set => _respawnTime = value;
        }

        public CharacterGameMode() : base()
        {

        }
        
        protected internal override void HandleLocalPlayerJoined(ControllerType item)
        {
            PawnType pawn = _pawnClass.CreateNew();

            BaseRenderPanel.WorldPanel?.GetOrAddViewport(item.LocalPlayerIndex)?.RegisterController(item);
            
            item.EnqueuePosession(pawn);
            pawn.QueueRespawn();
        }
        public virtual bool FindSpawnPoint(PawnController c, out Matrix4 transform)
        {
            foreach (IActor a in Engine.World.StateRef.File.SpawnedActors)
                if (a is CharacterSpawnPointActor p && p.CanSpawnPlayer(c))
                {
                    transform = p.RootComponent.WorldMatrix;
                    return true;
                }
            transform = Matrix4.Identity;
            return false;
        }
        public virtual void OnCharacterKilled(ICharacterPawn killed, ICharacterPawn instigator, IActor killer)
        {
            killed.QueueRespawn(RespawnTime);
        }
    }
}