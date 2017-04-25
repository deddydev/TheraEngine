﻿using System;
using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using CustomEngine.Input;

namespace CustomEngine.Tests
{
    public class TestGameMode : CharacterGameMode
    {
        public TestGameMode()
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
        public override Matrix4 FindSpawnPoint()
        {
            return Matrix4.Identity;
        }
        public override void EndGameplay()
        {
            throw new NotImplementedException();
        }
        public override void OnKilled(CharacterPawn pawn)
        {

        }
    }
}