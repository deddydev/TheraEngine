using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds;

namespace CustomEngine.Rendering.Textures
{
    public class DecalActor : Actor
    {
        private DecalComponent _decalComponent;

        public DecalComponent DecalComponent
        {
            get { return _decalComponent; }
            set { _decalComponent = value; }
        }

        protected override void SetupComponents()
        {
            RootComponent = _decalComponent = new DecalComponent();
        }
    }
}
