using System.Collections.Generic;
using TheraEngine.Input;
using TheraEngine.Actors;

namespace Thera.GameModes
{
    public class TheraCharacterController : LocalPlayerController
    {
        public TheraCharacterController(ELocalPlayerIndex index) : base(index) { }
        public TheraCharacterController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue) : base(index, possessionQueue) { }
    }
}