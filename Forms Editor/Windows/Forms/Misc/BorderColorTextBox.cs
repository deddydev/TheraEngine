using System.Drawing;
using System.Runtime.InteropServices;
using TheraEditor.Windows.Forms;

namespace System.Windows.Forms
{
    public class BorderColorTextBox : TextBox
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        [Flags()]
        public enum RedrawWindowFlags : uint
        {
            Invalidate = 0X1,
            InternalPaint = 0X2,
            Erase = 0X4,
            Validate = 0X8,
            NoInternalPaint = 0X10,
            NoErase = 0X20,
            NoChildren = 0X40,
            AllChildren = 0X80,
            UpdateNow = 0X100,
            EraseNow = 0X200,
            Frame = 0X400,
            NoFrame = 0X800
        }

        private const int WM_NCPAINT = 0x85;

        protected override CreateParams CreateParams
        {
            get
            {
                if (DesignMode)
                    return base.CreateParams;

                CreateParams cp = base.CreateParams;
                cp.ExStyle &= (~0x00000200); // WS_EX_CLIENTEDGE
                cp.Style |= 0x00800000; // WS_BORDER
                return cp;
            }
        }

        public Color FocusedColor
        {
            get => _focusedPen.Color;
            set
            {
                _focusedPen.Color = value;
                RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Invalidate);
            }
        }
        public Color RegularColor
        {
            get => _regularPen.Color;
            set
            {
                _regularPen.Color = value;
                RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Invalidate);
            }
        }

        private Pen _focusedPen = new Pen(Editor.TurquoiseColor);
        private Pen _regularPen = new Pen(Color.FromArgb(30, 30, 30));

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCPAINT)
            {
                if (BorderStyle == BorderStyle.None)
                    return;
                
                IntPtr hdc = GetWindowDC(Handle);
                using (Graphics g = Graphics.FromHdcInternal(hdc))
                {
                    g.DrawRectangle(Focused ? _focusedPen : _regularPen, 0, 0, Width - 1, Height - 1);
                }
                ReleaseDC(Handle, hdc);
            }
            else
                base.WndProc(ref m);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode)
                RecreateHandle();
            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Invalidate);
        }
    }
}
