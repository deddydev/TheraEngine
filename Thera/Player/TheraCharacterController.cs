using System.Collections.Generic;
using TheraEngine.Input;
using TheraEngine.Worlds.Actors;

namespace Thera.GameModes
{
    public class TheraCharacterController : LocalPlayerController
    {
        public TheraCharacterController(LocalPlayerIndex index) : base(index) { }
        public TheraCharacterController(LocalPlayerIndex index, Queue<IPawn> possessionQueue) : base(index, possessionQueue) { }
    }
}