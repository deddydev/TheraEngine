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
        protected override void PostConstruct()
        {
            base.PostConstruct();
        }
        
        public bool CanSpawnPlayer(PawnController c)
        {
            return true;
        }
        public bool IsBlocked => RootComponent.PhysicsDriver.Overlapping.Count > 0;
    }
}
