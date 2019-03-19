using System;
using System.ComponentModel;
using System.Drawing.Text;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI
{
    public class UITextComponent : UIMaterialRectangleComponent, IPreRendered
    {
        public UITextComponent() : base(TMaterial.CreateUnlitTextureMaterialForward(
            new TexRef2D("DrawSurface", 1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                MagFilter = ETexMagFilter.Nearest,
                MinFilter = ETexMinFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                Resizable = true
            }), true)
        {
            _textDrawer = new TextDrawer();
            _textDrawer.NeedsRedraw += WantsRedraw;
            
            RenderCommand.RenderPass = ERenderPass.TransparentForward;
            RenderCommand.Mesh.Material.RenderParams = new RenderingParameters(true);
        }

        private void WantsRedraw(bool forceFullRedraw)
        {
            NeedsRedraw = true;
            ForceFullRedraw = forceFullRedraw;
        }

        [TSerialize(nameof(TextQuality))]
        private TextRenderingHint _textQuality = TextRenderingHint.ClearTypeGridFit;
        [TSerialize(nameof(TextureResolutionMultiplier))]
        private Vec2 _texRes = new Vec2(3.0f);
        [TSerialize(nameof(TextDrawer))]
        private TextDrawer _textDrawer;

        public TexRef2D TextTexture => Texture<TexRef2D>(0);
        public TextDrawer TextDrawer => _textDrawer;

        public Vec2 TextureResolutionMultiplier
        {
            get => _texRes;
            set
            {
                _texRes = value;

                NeedsRedraw = true;
                ForceFullRedraw = true;
                PerformResize();
            }
        }
        public TextRenderingHint TextQuality
        {
            get => _textQuality;
            set
            {
                _textQuality = value;

                NeedsRedraw = true;
                ForceFullRedraw = true;
            }
        }

        public bool ForceFullRedraw { get; set; } = true;
        public IVec2? NeedsResize { get; set; } = null;
        public bool NeedsRedraw { get; set; } = false;

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 rect = base.Resize(parentBounds);

            int w = (int)(Width * TextureResolutionMultiplier.X);
            int h = (int)(Height * TextureResolutionMultiplier.Y);
            if (w != TextTexture.Width || h != TextTexture.Height)
            {
                NeedsResize = new IVec2(w, h);
                NeedsRedraw = true;
                ForceFullRedraw = true;
            }

            return rect;
        }
        [Browsable(false)]
        public bool PreRenderEnabled => NeedsResize != null || NeedsRedraw;
        public void PreRenderUpdate(Camera camera) { }
        public void PreRender(Viewport viewport, Camera camera) { }
        public void PreRenderSwap()
        {
            if (NeedsResize != null)
            {
                TextTexture.Resize(NeedsResize.Value.X, NeedsResize.Value.Y);
                NeedsResize = null;
            }

            if (NeedsRedraw)
            {
                TextDrawer.Draw(TextTexture, TextureResolutionMultiplier, TextQuality, ForceFullRedraw);
                NeedsRedraw = false;
            }
        }
    }
}
