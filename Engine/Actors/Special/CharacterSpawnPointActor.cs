using TheraEngine.Input;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Actors.Types
{
    public class CharacterSpawnPointActor : Actor<TransformComponent>
    {
        protected override TransformComponent OnConstructRoot()
        {
            //TRigidBodyConstructionInfo info = new TRigidBodyConstructionInfo()
            //{
            //    CollidesWith = TCollisionGroup.All,
            //    CollisionGroup = CustomCollisionGroup.StaticWorld,
            //    CollisionEnabled = false,
            //    SimulatePhysics = false
            //};
            return new TransformComponent();
        }
        //TODO: test player's variable-sized capsule against space directly, not fixed-size root component capsule
        public virtual bool CanSpawnPlayer(PawnController c)
        {
            return true;//!IsBlocked;
        }
        //public bool IsBlocked => RootComponent.PhysicsDriver.Overlapping.Count > 0;
    }
}
