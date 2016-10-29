using CustomEngine.Worlds.Actors;
using System;

namespace CustomEngine.Input
{
    //This class will be used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to controller).
    public abstract class PawnController : ObjectBase
    {
        private Pawn _controlledPawn;
        public Pawn ControlledPawn
        {
            get { return _controlledPawn; }
            set
            {
                _controlledPawn?.OnUnPossessed();
                _controlledPawn = value;
                _controlledPawn?.OnPossessed(this);
            }
        }
    }
}
