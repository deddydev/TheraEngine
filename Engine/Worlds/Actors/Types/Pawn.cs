using CustomEngine.Input;
using System;

namespace CustomEngine.Worlds.Actors
{
    public abstract class Pawn : Actor
    {
        private PhysicsState _physicsState;
        private PawnController _controller;

        public PawnController Controller { get { return _controller; } }

        public virtual void OnPossessed(PawnController c)
        {
            _controller = c;
        }
        public virtual void OnUnPossessed()
        {
            _controller = null;
        }

        protected override void SetupComponents()
        {
            
        }

        internal override void Tick(float delta)
        {
            if (Engine.World == null)
                return;

            Box bounds = Engine.World.Settings.OriginRebaseBounds;
            if (!bounds.ContainsPoint(RootComponent.Transform.Translation))
                Engine.World.RebaseOrigin(RootComponent.Transform.Translation);
        }
    }
}
