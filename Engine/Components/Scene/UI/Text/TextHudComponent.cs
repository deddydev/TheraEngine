using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials.Textures;
using System;

namespace TheraEngine.Rendering.UI
{
    public class TextHudComponent : UIMaterialRectangleComponent
    {
        public TextHudComponent() : base(TMaterial.CreateUnlitTextureMaterialForward(
            new TexRef2D("DrawSurface", 1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb)))
        {
            _textDrawer = new TextDrawer();
            _textDrawer.NeedsRedraw += Redraw;
        }

        private TextDrawer _textDrawer;

        public RenderTex2D TextTexture => Texture<RenderTex2D>(0);
        public TextDrawer TextDrawer => _textDrawer;

        private void Redraw() => TextDrawer.Draw(TextTexture);

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 rect = base.Resize(parentBounds);
            TextTexture.Resize((int)Width, (int)Height);
            Redraw();
            return rect;
        }
    }
}
