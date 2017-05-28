using CustomEngine.GameModes;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Worlds;
using Thera.GameModes;

namespace Thera.Worlds.Actors
{
    public class TheraCharacter : CharacterPawn
    {
        StaticPlayerTraits _staticPlayerTraits;
        InheritablePlayerTraits _inheritablePlayerTraits;

        public override void OnSpawned(World world)
        {
            base.OnSpawned(world);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
        }
    }
}
