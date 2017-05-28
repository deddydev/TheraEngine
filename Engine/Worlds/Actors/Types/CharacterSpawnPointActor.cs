using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Input;

namespace CustomEngine.Worlds.Actors.Types
{
    public class CharacterSpawnPointActor : Actor<CapsuleComponent>
    {
        internal bool CanSpawnPlayer(PawnController c)
        {
            throw new NotImplementedException();
        }
    }
}
