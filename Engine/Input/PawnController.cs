using TheraEngine.Worlds.Actors;
using System.Collections.Generic;

namespace TheraEngine.Input
{
    public interface IPawnController
    {
        IPawn ControlledPawn { get; set; }
        void EnqueuePosession(IPawn pawn);
    }
    //This base class is used to send input information to a movement component for an actor.
    //Input can come from a player's gamepad or an AI (these are subclasses to pawn controller).
    public abstract class PawnController : ObjectBase, IPawnController
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
        /// <summary>
        /// Queues the given pawn for possession.
        /// If the currently possessed pawn is null, possesses the given pawn immediately.
        /// </summary>
        /// <param name="pawn">The pawn to possess.</param>
        public void EnqueuePosession(IPawn pawn)
        {
            if (ControlledPawn == null)
                ControlledPawn = pawn;
            else
                _possessionQueue.Enqueue(pawn);
        }
    }
}
