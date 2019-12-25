using System;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Text
{
    public class UIRichTextBoxComponent : UITextRasterComponent
    {
        public UIRichTextBoxComponent() : base()
        {
            TextDrawer.Text.Add(_uiText);
        }

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            base.OnResizeLayout(parentRegion);
            _uiText.Region.Extents = ActualSize;
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