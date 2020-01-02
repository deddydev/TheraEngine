using System.Drawing;
using System.Windows.Forms;

namespace Extensions
{
    public partial class Ext
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            int start = box.SelectionStart;
            int length = box.SelectionLength;

            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;

            box.SelectionStart = start;
            box.SelectionLength = length;
        }
    }
}
