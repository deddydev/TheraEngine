using CustomEngine.Worlds.Actors;
using System;

namespace CustomEngine.Input
{
    //This class will be used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to controller).
    public abstract class PawnController : ObjectBase
    {
        public Pawn _controlledPawn;

        public void Possess(Pawn p)
        {
            if (_controlledPawn != null)
                _controlledPawn.OnUnPossessed();
            _controlledPawn = p;
            _controlledPawn.OnPossessed(this);
        }
    }
}
