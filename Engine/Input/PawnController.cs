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
            _possessionQueue = new Queue<IPawn>();
        }
        public PawnController(Queue<IPawn> possessionQueue)
        {
            _possessionQueue = possessionQueue;
            if (_possessionQueue.Count != 0)
                ControlledPawn = _possessionQueue.Dequeue();
        }

        protected Queue<IPawn> _possessionQueue;
        protected IPawn _controlledPawn;
        public virtual IPawn ControlledPawn
        {
            get => _controlledPawn;
            set
            {
                _controlledPawn?.OnUnPossessed();
                _controlledPawn = value;

                if (_controlledPawn == null && _possessionQueue.Count != 0)
                    _controlledPawn = _possessionQueue.Dequeue();
                
                _controlledPawn?.OnPossessed(this);
            }
        }
        public void EnqueuePosession(IPawn pawn)
        {
            if (ControlledPawn == null)
                ControlledPawn = pawn;
            else
                _possessionQueue.Enqueue(pawn);
        }
    }
}
