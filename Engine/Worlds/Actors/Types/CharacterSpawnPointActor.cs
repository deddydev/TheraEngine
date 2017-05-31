using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Input;
using BulletSharp;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Types
{
    public class CharacterSpawnPointActor : Actor<CapsuleComponent>
    {
        protected override CapsuleComponent OnConstruct()
        {
            PhysicsConstructionInfo info = new PhysicsConstructionInfo()
            {
                CollidesWith = CustomCollisionGroup.All,
                Group = CustomCollisionGroup.StaticWorld,
                CollisionEnabled = false,
                SimulatePhysics = false
            };
            return new CapsuleComponent(0.5f, 1.0f, info);
        }
        protected override void PostConstruct()
        {
            base.PostConstruct();
        }
        public bool CanSpawnPlayer(PawnController c)
        {
            return !IsBlocked;
        }
        public bool IsBlocked => RootComponent.PhysicsDriver.Overlapping.Count > 0;
    }
}
