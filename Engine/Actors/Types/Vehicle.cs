using System.Collections.Generic;
using TheraEngine.Worlds.Actors.Components;
using TheraEngine.Worlds.Actors.Components.Scene.Mesh;
using TheraEngine.Worlds.Actors.Types.Pawns;

namespace TheraEngine.Worlds.Actors.Types
{
    public class Vehicle : Pawn<SkeletalMeshComponent>
    {
        private ICharacterPawn _driver;
        private List<ICharacterPawn> _passengers;
        private int _maxPassengers;

        public Vehicle()
        {
        }

        public Vehicle(bool deferInitialization)
            : base(deferInitialization)
        {
        }

        public Vehicle(bool deferInitialization, LocalPlayerIndex possessor)
            : base(deferInitialization, possessor)
        {
        }

        public Vehicle(SkeletalMeshComponent root, params LogicComponent[] logicComponents) 
            : base(root, logicComponents)
        {
        }

        public Vehicle(LocalPlayerIndex possessor, SkeletalMeshComponent root, params LogicComponent[] logicComponents)
            : base(possessor, root, logicComponents)
        {
        }
    }
}
