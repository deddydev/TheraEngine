﻿using System;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Text
{
    public class UIRichTextBoxComponent : UITextComponent
    {
        public UIRichTextBoxComponent() : base()
        {
            TextDrawer.Text.Add(_uiText);
        }

        public override Vec2 OnResize(Vec2 parentBounds)
        {
            var resize = base.OnResize(parentBounds);
            _uiText.Region.Extents = Size;
            return resize;
        }

        private readonly UIString2D _uiText = new UIString2D();

        public string Text
        {
            get => _uiText.Text;
            set => _uiText.Text = value;
        }

        public bool AllowHorizontalScroll { get; set; } = false;
        public bool AllowVerticalScroll { get; set; } = true;

        public bool WordWrap 
        {
            get => (_uiText.Format.FormatFlags & System.Drawing.StringFormatFlags.NoWrap) == 0;
            set
            {
                if (value)
                    _uiText.Format.FormatFlags &= ~System.Drawing.StringFormatFlags.NoWrap;
                else
                    _uiText.Format.FormatFlags |= System.Drawing.StringFormatFlags.NoWrap;
            }
        }
    }
}