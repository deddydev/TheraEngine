using System.Drawing;
using System.Windows.Forms;
using TheraEditor.Core.Extensions;

namespace TheraEditor.Windows.Forms
{
    public class AutoEllipsisLabel : Label
    {
        public AutoEllipsisLabel()
            => AutoEllipsis = true;
        
        protected override void OnPaint(PaintEventArgs e)
        {
            using (Pen p = new Pen(new SolidBrush(BackColor)))
                e.Graphics.DrawRectangle(p, ClientRectangle);
            TextFormatFlags tf = TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis;
            TextRenderer.DrawText(e.Graphics, Text, Font, this.GetPaddedRectangle(), ForeColor, tf);
        }
    }
}
