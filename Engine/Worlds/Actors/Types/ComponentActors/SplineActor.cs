using TheraEngine.Worlds.Actors.Components;
using TheraEngine.Worlds.Actors.Components.Scene;

namespace TheraEngine.Worlds.Actors.Types.ComponentActors
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
