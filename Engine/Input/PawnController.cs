using CustomEngine.World.Actors;
using CustomEngine.World.Actors.Components;
using System;

namespace CustomEngine.Input
{
    //This class will be used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to controller).
    public abstract class PawnController : ObjectBase
    {
        public PawnController()
        {
            Engine.ActiveControllers.Add(this);
        }
        ~PawnController()
        {
            if (Engine.ActiveControllers.Contains(this))
                Engine.ActiveControllers.Remove(this);
        }

        public Pawn _controlledComp;
    }
}
