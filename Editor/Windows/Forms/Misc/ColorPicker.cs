using System.Drawing;
using System.Text;
using System.Drawing.Drawing2D;
using TheraEngine.Rendering.Models.Materials;

namespace System.Windows.Forms
{
    public partial class ColorPicker : UserControl
    {
        //private int _hue;
        private HSVPixel _hsv = new HSVPixel(0, 100, 100);

        //private int _alpha = 255;
        private ARGBPixel _rgba;

        bool _squareGrabbing;
        int _squareX, _squareY;

        bool _barGrabbing;
        int _barY;

        bool _alphaGrabbing;
        int _alphaY;

        bool _updating;

        private NumericUpDown[] _boxes;

        private int _brushH = -1;
        private PathGradientBrush _squareBrush;
        //private GraphicsPath _squarePath;
        private Color[] _boxColors = new Color[] { Color.Black, Color.White, Color.Black, Color.Black, Color.Black };

        private LinearGradientBrush _barBrush;
        private LinearGradientBrush _alphaBrush;

        public event EventHandler ColorChanged;

        private bool _showAlpha = true;
        public bool ShowAlpha
        {
            get => _showAlpha;
            set => panel2.Visible = numA.Visible = lblA.Visible = _showAlpha = value;
        }

        public Color ColorValue
        {
            get => _rgba;
            set
            {
                _rgba = value;
                OnColorChanged(false);
            }
        }

        public ColorPicker()
        {
            InitializeComponent();

            AutoScaleMode = AutoScaleMode.Font;

            _boxes = new NumericUpDown[] { numH, numS, numV, numR, numG, numB, numA };
            for (int i = 0; i < _boxes.Length; i++)
            {
                _boxes[i].ValueChanged += OnBoxChanged;
                _boxes[i].Tag = i;
            }

            Rectangle r = pnlColorBox.ClientRectangle;

            //_squarePath = new GraphicsPath();
            //_squarePath.AddRectangle(r);
            //_squareBrush = new PathGradientBrush(_squarePath);

            _squareBrush = new PathGradientBrush(new PointF[] {
                new PointF(r.Width, 0),
                new PointF(r.Width, r.Height),
                new PointF(0, r.Height),
                new PointF(0,0),
                new PointF(r.Width, 0)})
            {
                CenterPoint = new PointF(r.Width / 2, r.Height / 2)
            };
            float p = r.Height / 6.0f / r.Height;
            _barBrush = new LinearGradientBrush(new Rectangle(0, 0, r.Width, r.Height), Color.Red, Color.Red, LinearGradientMode.Vertical);

            ColorBlend blend = new ColorBlend()
            {
                Colors = new Color[] { Color.Red, Color.Yellow, Color.Lime, Color.Cyan, Color.Blue, Color.Magenta, Color.Red },
                Positions = new float[] { 0, p, p * 2, p * 3, p * 4, p * 5, 1.0f }
            };
            _barBrush.InterpolationColors = blend;

            _alphaBrush = new LinearGradientBrush(new Rectangle(0, 0, pnlAlpha.Width, pnlAlpha.Height), Color.White, Color.Black, LinearGradientMode.Vertical);
        }

        protected void OnBoxChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            NumericUpDown box = sender as NumericUpDown;
            int value = (int)box.Value;
            int index = (int)box.Tag;
            switch (index)
            {
                case 0: { _hsv.H = (ushort)value; break; }
                case 1: { _hsv.S = (byte)value; break; }
                case 2: { _hsv.V = (byte)value; break; }
                case 3: { _rgba.R = (byte)value; break; }
                case 4: { _rgba.G = (byte)value; break; }
                case 5: { _rgba.B = (byte)value; break; }
                case 6: { pnlAlpha.Invalidate(); break; }
                default: return;
            }

            if (index == 6)
            {
                _rgba.A = (byte)value;
                txtColorCode.Text = _rgba.ToRGBAColorCode();
                ColorChanged?.Invoke(this, null);
            }
            else
                OnColorChanged(index >= 0 && index < 3);
        }

        protected virtual void OnColorChanged(bool hsvToRgb)
        {
            _updating = true;

            if (hsvToRgb)
            {
                _rgba = (ARGBPixel)_hsv;
                _rgba.A = (byte)numA.Value;
            }
            else
                _hsv = (HSVPixel)_rgba;

            numH.Value = _hsv.H;
            numS.Value = _hsv.S;
            numV.Value = _hsv.V;
            numR.Value = _rgba.R;
            numG.Value = _rgba.G;
            numB.Value = _rgba.B;
            numA.Value = _rgba.A;

            txtColorCode.Text = _rgba.ToRGBAColorCode();

            _updating = false;

            pnlColorBox.Invalidate();
            pnlColorBar.Invalidate();

            ColorChanged?.Invoke(this, null);
        }

        #region ColorBox
        private void pnlColorBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _squareGrabbing = true;
                pnlColorBox_MouseMove(sender, e);
            }
        }
        private void pnlColorBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _squareGrabbing = false;
        }
        private void pnlColorBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (_squareGrabbing)
            {
                int x = Math.Min(Math.Max(e.X, 0), pnlColorBox.Width);
                int y = Math.Min(Math.Max(e.Y, 0), pnlColorBox.Height);
                if ((x != _squareX) || (y != _squareY))
                {
                    _squareX = x;
                    _squareY = y;

                    _hsv.V = (byte)((float)x / pnlColorBox.Width * 100);
                    _hsv.S = (byte)((float)(pnlColorBox.Height - y) / pnlColorBox.Height * 100);

                    OnColorChanged(true);
                }
            }
        }
        private void pnlColorBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //Update brush if color changed
            if (_brushH != _hsv.H)
            {
                _boxColors[0] = _boxColors[4] = (Color)(new HSVPixel(_hsv.H, 100, 100));
                _squareBrush.SurroundColors = _boxColors;
                _squareBrush.CenterColor = (Color)(new HSVPixel(_hsv.H, 50, 50));
                _brushH = _hsv.H;
            }

            //Draw square
            //g.FillPath(_squareBrush, _squarePath);
            g.FillRectangle(_squareBrush, pnlColorBox.ClientRectangle);

            //Draw indicator
            int x = (int)(_hsv.V / 100.0f * pnlColorBox.Width);
            int y = (int)((100 - _hsv.S) / 100.0f * pnlColorBox.Height);
            Rectangle r = new Rectangle(x - 3, y - 3, 6, 6);
            ARGBPixel p = _rgba.Inverse();
            p.A = 255;

            using (Pen pen = new Pen((Color)p))
                g.DrawEllipse(pen, r);

            r.X -= 1;
            r.Y -= 1;
            r.Width += 2;
            r.Height += 2;
            p = p.Lighten(64);

            using (Pen pen = new Pen((Color)p))
                g.DrawEllipse(pen, r);
        }
        #endregion

        #region HueBar
        private void pnlColorBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _barGrabbing = true;
                pnlColorBar_MouseMove(sender, e);
            }
        }
        private void pnlColorBar_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _barGrabbing = false;
        }
        private void pnlColorBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (_barGrabbing)
            {
                int y = Math.Max(Math.Min(e.Y, (pnlColorBar.Height - 1)), 0);
                if (y != _barY)
                {
                    _barY = y;

                    _hsv.H = (ushort)((float)y / (pnlColorBar.Height - 1) * 360);
                    OnColorChanged(true);
                }
            }
        }
        private void pnlColorBar_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //Draw bar
            g.FillRectangle(_barBrush, pnlColorBar.ClientRectangle);

            //Draw indicator
            ARGBPixel p = ((ARGBPixel)(new HSVPixel(_hsv.H, 100, 100))).Inverse();
            int y = (int)(_hsv.H / 360.0f * (pnlColorBar.Height - 1));
            Rectangle r = new Rectangle(-1, y - 2, pnlColorBar.Width + 1, 4);

            using (Pen pen = new Pen((Color)p))
                g.DrawRectangle(pen, r);

            r.Y += 1;
            r.Height -= 2;
            p = p.Lighten(64);

            using (Pen pen = new Pen((Color)p))
                g.DrawRectangle(pen, r);
        }
        #endregion

        #region AlphaBar

        private void pnlAlpha_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _alphaGrabbing = true;
                pnlAlpha_MouseMove(sender, e);
            }
        }
        private void pnlAlpha_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                _alphaGrabbing = false;
        }
        private void pnlAlpha_MouseMove(object sender, MouseEventArgs e)
        {
            if (_alphaGrabbing)
            {
                int y = Math.Max(Math.Min(e.Y, (pnlAlpha.Height - 1)), 0);
                if (y != _alphaY)
                {
                    _alphaY = y;
                    numA.Value = (byte)(255 - ((float)y / (pnlAlpha.Height - 1) * 255));
                    _updating = true;
                    txtColorCode.Text = _rgba.ToRGBAColorCode();
                    _updating = false;
                    ColorChanged?.Invoke(this, null);
                }
            }
        }
        private void pnlAlpha_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            //Draw bar
            g.FillRectangle(_alphaBrush, pnlAlpha.ClientRectangle);

            //Draw indicator
            byte col = (byte)(255 - _rgba.A);
            ARGBPixel p = new ARGBPixel(255, col, col, col);
            int y = (int)(col / 255.0f * (pnlAlpha.Height - 1));
            Rectangle r = new Rectangle(-1, y - 2, pnlAlpha.Width + 1, 4);

            using (Pen pen = new Pen((Color)p))
                g.DrawRectangle(pen, r);

            p.Lighten(64);

            r.Y += 1;
            r.Height -= 2;

            using (Pen pen = new Pen((Color)p))
                g.DrawRectangle(pen, r);
        }

        #endregion

        readonly string _allowed = "0123456789abcdefABCDEF";
        private void txtColorCode_TextChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            string s = "";
            foreach (char c in txtColorCode.Text)
                if (_allowed.IndexOf(c) >= 0)
                    s += c;
            s = s.Substring(0, s.Length.Clamp(0, 8));

            bool focused = txtColorCode.Focused;
            int start = txtColorCode.SelectionStart;
            int len = txtColorCode.SelectionLength;

            _updating = true;
            if (txtColorCode.Text != s)
                txtColorCode.Text = s;
            _rgba.R = s.Length >= 2 ? byte.Parse(s.Substring(0, 2), Globalization.NumberStyles.HexNumber) : (byte)0;
            _rgba.G = s.Length >= 4 ? byte.Parse(s.Substring(2, 2), Globalization.NumberStyles.HexNumber) : (byte)0;
            _rgba.B = s.Length >= 6 ? byte.Parse(s.Substring(4, 2), Globalization.NumberStyles.HexNumber) : (byte)0;
            _rgba.A = s.Length >= 8 ? byte.Parse(s.Substring(6, 2), Globalization.NumberStyles.HexNumber) : (byte)0xFF;
            _updating = false;

            OnColorChanged(false);

            txtColorCode.SelectionStart = start;
            txtColorCode.SelectionLength = len;
            if (focused)
                txtColorCode.Select();
        }

        private void txtColorCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            char c = e.KeyChar;
            TextBox box = txtColorCode;

            if (e.KeyChar == (char)Keys.Back && box.SelectionStart > 0)
            {
                int start = box.SelectionStart;
                StringBuilder sb = new StringBuilder(box.Text);
                sb[start - 1] = '0';
                box.Text = sb.ToString();
                box.SelectionStart = start - 1;
                e.Handled = true;
            }
            else if ((!Char.IsControl(c) || e.KeyChar == (char)Keys.Delete) && box.SelectionStart < box.TextLength)
            {
                if (_allowed.IndexOf(c) >= 0 || e.KeyChar == (char)Keys.Delete)
                {
                    int start = box.SelectionStart;
                    StringBuilder sb = new StringBuilder(box.Text);
                    sb[start] = e.KeyChar == (char)Keys.Delete ? '0' : e.KeyChar;
                    box.Text = sb.ToString();
                    box.SelectionStart = start + 1;
                }
                e.Handled = true;
            }
        }
    }
}
