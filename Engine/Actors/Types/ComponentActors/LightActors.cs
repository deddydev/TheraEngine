using TheraEngine.Components.Scene.Lights;

namespace TheraEngine.Actors.Types.Lights
{
    public class DirectionalLightActor : Actor<DirectionalLightComponent>
    {
        public DirectionalLightActor() : base() { }
    }
    public class PointLightActor : Actor<PointLightComponent>
    {
        public PointLightActor() : base() { }
    }
    public class SpotLightActor : Actor<SpotLightComponent>
    {
        public SpotLightActor() : base() { }
    }
}
