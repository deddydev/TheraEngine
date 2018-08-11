using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Text
{
    public class UIRichTextBoxComponent : UITextComponent
    {
        public UIRichTextBoxComponent() : base()
        {
            TextDrawer.Add(Text);
        }

        public UIString2D Text { get; } = new UIString2D();
        public bool AllowHorizontalScroll { get; set; } = false;
        public bool AllowVerticalScroll { get; set; } = true;
    }
}