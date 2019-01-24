﻿using System;
using System.ComponentModel;
using System.Drawing.Text;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI
{
    public class UITextComponent : UIMaterialRectangleComponent
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
            _quad.Material.RenderParams = new RenderingParameters(true);
            _textDrawer = new TextDrawer();
            _textDrawer.NeedsRedraw += Redraw;
            RenderCommand.RenderPass = ERenderPass.TransparentForward;
        }

        [TSerialize(nameof(TextQuality))]
        private TextRenderingHint _textQuality = TextRenderingHint.AntiAliasGridFit;
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
                PerformResize();
            }
        }
        public TextRenderingHint TextQuality
        {
            get => _textQuality;
            set
            {
                _textQuality = value;
                Redraw(true);
            }
        }

        public void Redraw(bool forceFullRedraw)
            => TextDrawer.Draw(TextTexture, TextureResolutionMultiplier, TextQuality, forceFullRedraw);

        public override Vec2 Resize(Vec2 parentBounds)
        {
            Vec2 rect = base.Resize(parentBounds);

            int w = (int)(Width * TextureResolutionMultiplier.X);
            int h = (int)(Height * TextureResolutionMultiplier.Y);
            if (w != TextTexture.Width || h != TextTexture.Height)
            {
                TextTexture.Resize(w, h);
                Redraw(true);
            }

            return rect;
        }
    }
}
