using CustomEngine.Input;
using CustomEngine.Input.Devices;
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
            if (c == null)
                OnUnPossessed();

            _controller = c;
        }
        public virtual void OnUnPossessed()
        {
            if (_controller == null)
                return;

            _controller = null;
        }

        protected override void SetupComponents()
        {

        }
        protected virtual void RegisterInput(InputInterface input) { }
        
        internal override void Tick(float delta)
        {
            if (Engine.World == null)
                return;

            Box bounds = Engine.World.Settings.OriginRebaseBounds;
            if (!bounds.Contains(RootComponent.LocalTransform.Translation))
                Engine.World.RebaseOrigin(RootComponent.LocalTransform.Translation);
        }
    }
}
