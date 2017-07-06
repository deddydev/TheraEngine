using System.Drawing;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Textures;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.HUD
{
    public class TextHudComponent : MaterialHudComponent, I2DRenderable
    {
        public TextHudComponent() : base(Material.GetUnlitTextureMaterial())
        {
            _textDrawer = new TextDrawer();
            _textDrawer.NeedsRedraw += _textDrawer_NeedsRedraw;
        }

        public Texture2D TextTexture => Texture(0);

        private void _textDrawer_NeedsRedraw()
            => _textDrawer.Draw(TextTexture);
        
        private void WrapText()
        {
            Bitmap b = TextTexture.Data.Bitmap;
            using (Graphics g = Graphics.FromImage(b))
            {
                //_text.Split()
            }
        }

        public string _text;
        public bool _wordWrap = true;
        Font _font;
        private TextDrawer _textDrawer;
    }
}
