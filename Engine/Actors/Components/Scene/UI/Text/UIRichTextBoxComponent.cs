using System;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Text
{
    public class UIRichTextBoxComponent : UITextComponent
    {
        public UIRichTextBoxComponent() : base()
        {
            TextDrawer.Add(Text);
        }

        public override Vec2 Resize(Vec2 parentBounds)
        {
            var resize = base.Resize(parentBounds);
            Text.Region.Extents = Size;
            return resize;
        }

        public IVec2 CursorPosition { get; set; }
        public UIString2D Text { get; } = new UIString2D();
        public bool AllowHorizontalScroll { get; set; } = false;
        public bool AllowVerticalScroll { get; set; } = true;
        public bool WordWrap { get; set; }
    }
}