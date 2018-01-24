using TheraEngine.Components;
using TheraEngine.Components.Scene;

namespace TheraEngine.Actors.Types.ComponentActors
{
    public class SplineActor : Actor<SplineComponent>
    {
        public SplineActor()
        {
        }

        public SplineActor(bool deferInitialization) 
            : base(deferInitialization)
        {
        }

        public SplineActor(SplineComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents)
        {
        }
    }
}
