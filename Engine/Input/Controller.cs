using CustomEngine.Components;
using System.Collections.Generic;
using System;

namespace CustomEngine.Input
{
    //This class will be used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to controller).
    public abstract class Controller : ObjectBase
    {
        public Controller()
        {
            Form._activeControllers.Add(this);
        }
        ~Controller()
        {
            if (Form._activeControllers.Contains(this))
                Form._activeControllers.Remove(this);
        }

        public MovementComponent _controlledComp;

        public virtual void UpdateTick(double deltaTime) { }
    }
}
