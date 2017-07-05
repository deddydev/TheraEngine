using System.Collections.Generic;
using TheraEngine.Input;
using TheraEngine.Worlds.Actors;

namespace Thera.GameModes
{
    public class TheraCharacterController : LocalPlayerController
    {
        public TheraCharacterController(PlayerIndex index) : base(index) { }
        public TheraCharacterController(PlayerIndex index, Queue<IPawn> possessionQueue) : base(index, possessionQueue) { }
    }
}