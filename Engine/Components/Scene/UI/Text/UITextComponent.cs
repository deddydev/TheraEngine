using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI
{
    public class UITextComponent : UIMaterialRectangleComponent
    {
        public UITextComponent() : base(TMaterial.CreateUnlitTextureMaterialForward(
            new TexRef2D("DrawSurface", 1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                MagFilter = ETexMagFilter.Linear,
                MinFilter = ETexMinFilter.LinearMipmapLinear,
                UWrap = ETexWrapMode.Clamp,
                VWrap = ETexWrapMode.Clamp,
            },
            new RenderingParameters()
            {
                BlendMode = new BlendMode()
                {
                    Enabled = true,
                    RgbSrcFactor = EBlendingFactor.SrcAlpha,
                    AlphaSrcFactor = EBlendingFactor.SrcAlpha,
                    RgbDstFactor = EBlendingFactor.OneMinusSrcAlpha,
                    AlphaDstFactor = EBlendingFactor.OneMinusSrcAlpha,
                    RgbEquation = EBlendEquationMode.FuncAdd,
                    AlphaEquation = EBlendEquationMode.FuncAdd,
                }
            }))
        {
            _textDrawer = new TextDrawer();
            _textDrawer.NeedsRedraw += Redraw;
        }

        private Vec2 _texScale = new Vec2(3.0f);
        private TextDrawer _textDrawer;

        public TexRef2D TextTexture => Texture<TexRef2D>(0);
        public TextDrawer TextDrawer => _textDrawer;

        public Vec2 TexScale
        {
            get => _texScale;
            set
            {
                _texScale = value;
                PerformResize();
            }
        }

        protected void Redraw() => TextDrawer.Draw(TextTexture, TexScale);

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 rect = base.Resize(parentBounds);
            TextTexture.Resize((int)(Width * TexScale.X), (int)(Height * TexScale.Y));
            Redraw();
            return rect;
        }
    }
}
