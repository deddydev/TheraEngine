using System;
using TheraEngine.GameModes;
using TheraEngine.Input;

namespace Testris
{
    public class TetrisGameMode : GameMode<TetrisPawn, LocalPlayerController>
    {
        public override void AbortGameplay()
        {

        }

        public override void BeginGameplay()
        {
            base.BeginGameplay();
        }

        public override void EndGameplay()
        {

        }

        protected override void HandleLocalPlayerJoined(LocalPlayerController item)
        {
            base.HandleLocalPlayerJoined(item);
        }
    }
}