using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds;

namespace CustomEngine.Worlds.Actors.Types
{
    public class DecalActor : Actor<DecalComponent>
    {
        protected override DecalComponent SetupComponents()
        {
            return new DecalComponent();
        }
    }
}
