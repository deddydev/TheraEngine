using TheraEngine.Worlds.Actors.Components.Scene;

namespace TheraEngine.Worlds.Actors.Types
{
    public class DecalActor : Actor<DecalComponent>
    {
        protected override DecalComponent OnConstruct()
        {
            return new DecalComponent();
        }
    }
}
