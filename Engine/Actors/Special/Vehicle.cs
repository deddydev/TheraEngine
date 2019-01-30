using System.Collections.Generic;
using TheraEngine.Components;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Actors.Types
{
    public class Vehicle : Pawn<TRComponent>
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

        public Vehicle(bool deferInitialization, ELocalPlayerIndex possessor)
            : base(deferInitialization, possessor)
        {
        }

        public Vehicle(SkeletalMeshComponent root, params LogicComponent[] logicComponents) 
            : base(root, logicComponents)
        {
        }

        public Vehicle(ELocalPlayerIndex possessor, SkeletalMeshComponent root, params LogicComponent[] logicComponents)
            : base(possessor, root, logicComponents)
        {
        }
    }
}
