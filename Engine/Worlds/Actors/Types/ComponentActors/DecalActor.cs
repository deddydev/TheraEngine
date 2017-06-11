using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds;

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
