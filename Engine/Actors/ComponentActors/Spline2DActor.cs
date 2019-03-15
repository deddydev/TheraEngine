using TheraEngine.Components;
using TheraEngine.Components.Scene;

namespace TheraEngine.Actors.Types.ComponentActors
{
    public class Spline2DActor : Actor<Spline2DComponent>
    {
        public Spline2DActor()
            : base() { }
        public Spline2DActor(bool deferInitialization) 
            : base(deferInitialization) { }
        public Spline2DActor(Spline2DComponent root, params LogicComponent[] logicComponents)
            : base(root, logicComponents) { }
    }
}