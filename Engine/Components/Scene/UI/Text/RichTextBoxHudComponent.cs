using System.Drawing;

namespace TheraEngine.Rendering.UI.Text
{
    public class RichTextBoxHudComponent : UITextComponent
    {
        private string _text;
        private bool _wordWrap = true;
        private Font _font;

        public bool WordWrap { get => _wordWrap; set => _wordWrap = value; }
        public Font Font { get => _font; set => _font = value; }
        public string Text { get => _text; set => _text = value; }

        public void WrapText()
        {
            //Bitmap b = TextTexture.Data.Bitmap;
            //using (Graphics g = Graphics.FromImage(b))
            //{
            //    //_text.Split()
            //}
        }
    }
}