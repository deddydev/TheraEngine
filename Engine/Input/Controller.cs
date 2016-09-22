using CustomEngine.Components;

namespace CustomEngine.Input
{
    //This class will be used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to controller).
    public abstract class Controller
    {
        public MovementComponent _controlledComp;
    }
}
