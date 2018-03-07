using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Text
{
    public class UIRichTextBoxComponent : UITextComponent
    {
        public UIRichTextBoxComponent() : base()
        {
            TextDrawer.Add(_string);
        }

        private UIString _string = new UIString();
        private bool 
            _horizontalScroll = false, 
            _verticalScroll = true;

        public UIString Text => _string;
        public bool AllowHorizontalScroll
        {
            get => _horizontalScroll;
            set => _horizontalScroll = value;
        }
        public bool AllowVerticalScroll
        {
            get => _verticalScroll;
            set => _verticalScroll = value;
        }
    }
}