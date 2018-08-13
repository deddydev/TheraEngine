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
            Invalidate      = 0x001,
            InternalPaint   = 0x002,
            Erase           = 0x004,
            Validate        = 0x008,
            NoInternalPaint = 0x010,
            NoErase         = 0x020,
            NoChildren      = 0x040,
            AllChildren     = 0x080,
            UpdateNow       = 0x100,
            EraseNow        = 0x200,
            Frame           = 0x400,
            NoFrame         = 0x800
        }

        private const int WM_NCPAINT = 0x85;

        protected override CreateParams CreateParams
        {
            get
            {
                if (DesignMode)
                    return base.CreateParams;

                CreateParams cp = base.CreateParams;
                cp.ExStyle &= ~0x200; // WS_EX_CLIENTEDGE
                cp.Style |= 0x800000; // WS_BORDER
                return cp;
            }
        }

        public Color FocusedColor
        {
            get => _focusedPen.Color;
            set
            {
                _focusedPen.Color = value;
                Redraw();
            }
        }
        public Color RegularColor
        {
            get => _regularPen.Color;
            set
            {
                _regularPen.Color = value;
                Redraw();
            }
        }
        public Color HoveredColor
        {
            get => _hoverPen.Color;
            set
            {
                _hoverPen.Color = value;
                Redraw();
            }
        }

        private Pen _focusedPen = new Pen(Editor.TurquoiseColor);
        private Pen _regularPen = new Pen(Color.FromArgb(30, 30, 30));
        private Pen _hoverPen = new Pen(Color.FromArgb(120, 120, 0));
        private bool _hovered = false;

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            Redraw();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            Redraw();
        }
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Redraw();
        }
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Redraw();
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (BorderStyle != BorderStyle.None)
            {
                IntPtr hdc = GetWindowDC(Handle);
                using (Graphics g = Graphics.FromHdcInternal(hdc))
                    g.DrawRectangle(Focused ? _focusedPen : (_hovered ? _hoverPen : _regularPen), 0, 0, Width - 1, Height - 1);
                ReleaseDC(Handle, hdc);
            }
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Redraw();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (DesignMode)
                RecreateHandle();
            Redraw();
        }
        private void Redraw()
            => RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.Frame | RedrawWindowFlags.EraseNow | RedrawWindowFlags.UpdateNow | RedrawWindowFlags.Invalidate);
    }
}
