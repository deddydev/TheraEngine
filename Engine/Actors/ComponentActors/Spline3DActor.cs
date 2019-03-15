using TheraEngine.Components;
using TheraEngine.Components.Scene;

namespace TheraEngine.Actors.Types.ComponentActors
{
    public class Spline3DActor : Actor<Spline3DComponent>
    {
        public Spline3DActor()
            : base() { }
        public Spline3DActor(bool deferInitialization) 
            : base(deferInitialization) { }
        public Spline3DActor(Spline3DComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
}