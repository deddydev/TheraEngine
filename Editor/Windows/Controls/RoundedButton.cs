using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public class RoundedButton : Button
    {
        public float BorderWidth { get; set; } = 3.0f;
        public Color BorderColor { get; set; } = Color.Black;

        //public RoundedButton() : base()
        //{
        //    SizeChanged += UpdateRegion;
        //}
        //~RoundedButton()
        //{
        //    SizeChanged -= UpdateRegion;
        //}
        //private void UpdateRegion(object sender, EventArgs e)
        //{
        //    int radius = Height;
        //    Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, radius, radius));
        //}

        //[DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        //public static extern IntPtr CreateRoundRectRgn
        //    (
        //       int nLeftRect,
        //       int nTopRect,
        //       int nRightRect,
        //       int nBottomRect,
        //       int nWidthEllipse,
        //       int nHeightEllipse
        //    );
        
        private GraphicsPath GetRoundPath(RectangleF Rect, int diameter)
        {
            float radius = diameter / 2.0f;

            GraphicsPath GraphPath = new GraphicsPath();

            GraphPath.AddArc(Rect.X, Rect.Y, diameter, diameter, 180, 90);
            GraphPath.AddLine(Rect.X + radius, Rect.Y, Rect.Width - radius, Rect.Y);

            GraphPath.AddArc(Rect.X + Rect.Width - diameter, Rect.Y, diameter, diameter, 270, 90);
            GraphPath.AddLine(Rect.Width, Rect.Y + radius, Rect.Width, Rect.Height - radius);

            GraphPath.AddArc(Rect.X + Rect.Width - diameter, Rect.Y + Rect.Height - diameter, diameter, diameter, 0, 90);
            GraphPath.AddLine(Rect.Width - radius, Rect.Height, Rect.X + radius, Rect.Height);

            GraphPath.AddArc(Rect.X, Rect.Y + Rect.Height - diameter, diameter, diameter, 90, 90);
            GraphPath.AddLine(Rect.X, Rect.Height - radius, Rect.X, Rect.Y + radius);

            GraphPath.CloseFigure();

            return GraphPath;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            RectangleF rect = new RectangleF(0, 0, Width, Height);
            GraphicsPath path = GetRoundPath(rect, Height);

            Region = new Region(path);

            using (Pen pen = new Pen(BorderColor, BorderWidth))
            {
                pen.Alignment = PenAlignment.Inset;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.DrawPath(pen, path);
            }
        }
    }
}
