using CustomEngine.Worlds.Actors.Components;
using CustomEngine.Worlds;

namespace CustomEngine.Worlds.Actors.Types
{
    public class DecalActor : Actor
    {
        private DecalComponent _decalComponent;
        public DecalComponent DecalComponent
        {
            get { return _decalComponent; }
            set { _decalComponent = value; }
        }

        protected override SceneComponent SetupComponents()
        {
            return _decalComponent = new DecalComponent();
        }
    }
}
