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
            //AddComponent();
        }
    }
}
