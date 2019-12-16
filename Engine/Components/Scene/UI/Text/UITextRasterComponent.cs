using System;
using System.ComponentModel;
using System.Drawing.Text;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI
{
    /// <summary>
    /// Renders text to a texture and binds the texure to a material rendered on a quad.
    /// If any text in the drawer is modified, the portion of the teture it inhabits will be redrawn.
    /// This component is best for text that does not change often or has a LOT of characters in it.
    /// </summary>
    public class UITextRasterComponent : UIInteractableComponent, IPreRendered
    {
        public UITextRasterComponent() : base(TMaterial.CreateUnlitTextureMaterialForward(MakeDrawSurface()), true) => Init();
        public UITextRasterComponent(TMaterial material, bool appendDrawSurfaceTexture = true) : base(material, true)
        {
            if (appendDrawSurfaceTexture)
                material.Textures.Add(MakeDrawSurface());

            Init();
        }
        private void Init()
        {
            _textDrawer = new TextRasterizer();
            _textDrawer.NeedsRedraw += WantsRedraw;

            RenderCommand.RenderPass = ERenderPass.TransparentForward;
            RenderCommand.Mesh.Material.RenderParams = new RenderingParameters(true);
        }

        public static TexRef2D MakeDrawSurface()
            => new TexRef2D("DrawSurface", 1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                MagFilter = ETexMagFilter.Nearest,
                MinFilter = ETexMinFilter.Nearest,
                UWrap = ETexWrapMode.ClampToEdge,
                VWrap = ETexWrapMode.ClampToEdge,
                Resizable = true
            };

        private void WantsRedraw(bool forceFullRedraw)
        {
            NeedsRedraw = true;
            ForceFullRedraw = forceFullRedraw;
        }

        [TSerialize(nameof(TextQuality))]
        private TextRenderingHint _textQuality = TextRenderingHint.AntiAlias;
        [TSerialize(nameof(TextureResolutionMultiplier))]
        private Vec2 _texRes = new Vec2(3.0f);
        [TSerialize(nameof(TextDrawer))]
        private TextRasterizer _textDrawer;

        public TexRef2D TextTexture => Texture<TexRef2D>(0);
        public TextRasterizer TextDrawer => _textDrawer;

        public Vec2 TextureResolutionMultiplier
        {
            get => _texRes;
            set
            {
                _texRes = value;

                NeedsRedraw = true;
                ForceFullRedraw = true;
                Resize();
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

        public override void ArrangeChildren(Vec2 translation, Vec2 parentBounds)
        {
            base.ArrangeChildren(translation, parentBounds);

            int w = (int)(Width * TextureResolutionMultiplier.X);
            int h = (int)(Height * TextureResolutionMultiplier.Y);
            if (w != TextTexture.Width || h != TextTexture.Height)
            {
                NeedsResize = new IVec2(w, h);
                NeedsRedraw = true;
                ForceFullRedraw = true;
            }
        }
        [Browsable(false)]
        public bool PreRenderEnabled => NeedsResize != null || NeedsRedraw;
        public void PreRenderUpdate(ICamera camera) { }
        public void PreRender(Viewport viewport, ICamera camera) { }
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
