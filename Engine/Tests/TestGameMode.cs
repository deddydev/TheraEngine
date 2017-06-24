﻿using System;
using TheraEngine.GameModes;
using TheraEngine.Worlds.Actors;
using TheraEngine.Input;

namespace TheraEngine.Tests
{
    public class TestGameMode : CharacterGameMode<TestCharacter, LocalPlayerController>
    {
        public TestGameMode() : base()
        {

        }
        public override void AbortGameplay()
        {
            
        }
        public override void EndGameplay()
        {

        }
    }
}