using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;

namespace CustomEngine.Input
{
    //This class will be used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to controller).
    public abstract class PawnController : ObjectBase
    {
        public PawnController()
        {
            _possessionQueue = new Queue<Pawn>();
        }
        public PawnController(Queue<Pawn> possessionQueue)
        {
            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0)
                ControlledPawn = _possessionQueue.Dequeue();
        }

        protected Queue<Pawn> _possessionQueue;
        protected Pawn _controlledPawn;
        public virtual Pawn ControlledPawn
        {
            get { return _controlledPawn; }
            set
            {
                _controlledPawn?.OnUnPossessed();
                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();
                
                _controlledPawn?.OnPossessed(this);
            }
        }
        public void EnqueuePosession(Pawn pawn)
        {
            if (ControlledPawn == null)
                ControlledPawn = pawn;
            else
                _possessionQueue.Enqueue(pawn);
        }
    }
}
