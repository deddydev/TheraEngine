using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Text;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.HUD
{
    public class TextHudComponent : MaterialHudComponent
    {
        public TextHudComponent() : base(Material.GetUnlitTextureMaterialForward(new TextureReference()))
        {
            _textDrawer = new TextDrawer();
            //_textDrawer.NeedsRedraw += Redraw;
        }

        private TextDrawer _textDrawer;

        //public Texture2D TextTexture => Texture(0);
        public TextDrawer TextDrawer => _textDrawer;

        //private void Redraw() => TextDrawer.Draw(TextTexture);

        public override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            BoundingRectangle rect = base.Resize(parentRegion);
            //TextTexture.Resize(Region.IntWidth, Region.IntHeight);
            //Redraw();
            return rect;
        }
    }
}
