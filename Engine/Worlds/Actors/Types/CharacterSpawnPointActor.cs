using System;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Worlds.Actors.Components.Scene.Transforms;

namespace TheraEngine.Worlds.Actors.Types
{
    public class CharacterSpawnPointActor : Actor<PositionComponent>
    {
        protected override PositionComponent OnConstruct()
        {
            PhysicsConstructionInfo info = new PhysicsConstructionInfo()
            {
                CollidesWith = CustomCollisionGroup.All,
                CollisionGroup = CustomCollisionGroup.StaticWorld,
                CollisionEnabled = false,
                SimulatePhysics = false
            };
            return new PositionComponent(Vec3.Zero);
        }
        //TODO: test player's variable-sized capsule against space directly, not fixed-size root component capsule
        public virtual bool CanSpawnPlayer(PawnController c)
        {
            return true;//!IsBlocked;
        }
        //public bool IsBlocked => RootComponent.PhysicsDriver.Overlapping.Count > 0;
    }
}
