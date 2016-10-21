using CustomEngine.Input;
using System;

namespace CustomEngine.Worlds.Actors
{
    public abstract class Pawn : Actor
    {
        private PhysicsState _physicsState;
        private PawnController _controller;

        public PawnController Controller { get { return _controller; } }

        public void OnPossessed(PawnController c)
        {
            _controller = c;
        }
        public void OnUnPossessed()
        {
            _controller = null;
        }

        protected override void SetupComponents()
        {
            
        }

        internal override void Tick(float delta)
        {
            Box bounds = Engine.World._settings.OriginRebaseBounds;
            if (!bounds.ContainsPoint(RootComponent.Transform.Translation))
                Engine.World.RebaseOrigin(RootComponent.Transform.Translation);
        }
    }
}
