using CustomEngine.Rendering.Textures;

namespace CustomEngine.Worlds.Actors.Components
{
    public class DecalComponent : SceneComponent
    {
        private Texture _texture;

        public override void Load()
        {
            //_texture = Texture.FromFile("");
            base.Load();
        }

        protected override void OnRender(float delta)
        {
            base.OnRender(delta);
        }
    }
}
