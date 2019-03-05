using TheraEngine.Components;
using TheraEngine.Components.Scene;

namespace TheraEngine.Actors.Types.ComponentActors
{
    public class SplineActor : Actor<Spline3DComponent>
    {
        public SplineActor()
            : base() { }
        public SplineActor(bool deferInitialization) 
            : base(deferInitialization) { }
        public SplineActor(Spline3DComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
}