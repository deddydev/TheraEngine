﻿using System;
using System.Windows.Forms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Text
{
    public class UITextBoxComponent : UITextRasterComponent
    {
        public UITextBoxComponent() : base()
        {
            _uiText = new UIString2D();
            _uiText.Format |= TextFormatFlags.TextBoxControl;

            TextDrawer.Text.Add(_uiText);
        }

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            base.OnResizeLayout(parentRegion);
            _uiText.Region.Extents = ActualSize.Raw;
        }

        private readonly UIString2D _uiText = new UIString2D();

        public string Text
        {
            get => _uiText.Text;
            set => _uiText.Text = value;
        }

        public TextFormatFlags TextFlags 
        {
            get => _uiText.Format;
            set => _uiText.Format = value;
        }
    }
}