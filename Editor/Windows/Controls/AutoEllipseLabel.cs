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
            TextFormatFlags tf = TextFormatFlags.SingleLine | TextFormatFlags.WordEllipsis;
            switch (TextAlign)
            {
                case ContentAlignment.BottomCenter:
                    tf |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.BottomLeft:
                    tf |= TextFormatFlags.Bottom | TextFormatFlags.Left;
                    break;
                case ContentAlignment.BottomRight:
                    tf |= TextFormatFlags.Bottom | TextFormatFlags.Right;
                    break;

                case ContentAlignment.MiddleCenter:
                    tf |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.MiddleLeft:
                    tf |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    tf |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
                    break;

                case ContentAlignment.TopCenter:
                    tf |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
                    break;
                case ContentAlignment.TopLeft:
                    tf |= TextFormatFlags.Top | TextFormatFlags.Left;
                    break;
                case ContentAlignment.TopRight:
                    tf |= TextFormatFlags.Top | TextFormatFlags.Right;
                    break;
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, this.GetPaddedRectangle(), ForeColor, tf);
        }
    }
}
