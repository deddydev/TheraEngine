using CustomEngine.Worlds.Actors;
using CustomEngine.Worlds;

namespace CustomEngine.Worlds.Actors.Types
{
    public class DecalActor : Actor<DecalComponent>
    {
        protected override DecalComponent OnConstruct()
        {
            return new DecalComponent();
        }
    }
}
