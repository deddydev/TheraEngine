using System;

namespace CustomEngine.Worlds.Actors
{
    public abstract class Pawn : Actor
    {
        private PhysicsState _physicsState;

        protected void OnPosessed()
        {

        }
        protected void OnUnPosessed()
        {

        }

        protected override void SetupComponents()
        {
            
        }

        public override void Update()
        {
            Box bounds = Engine.World._settings.OriginRebaseBounds;
            if (!bounds.ContainsPoint(RootComponent.Transform.Translation))
            {
                Engine.World.RebaseOrigin(RootComponent.Transform.Translation);
            }
        }
    }
}
