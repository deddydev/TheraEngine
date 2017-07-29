using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    public delegate void ColorChanged(Color c);
    public partial class ColorControl : UserControl
    {
        internal static HatchBrush TransparentCheckerboardBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.GhostWhite);

        public delegate void ColorChangedEvent(Color selection);
        public event ColorChangedEvent OnColorChanged;

        private Color _color;
        private Color _newColor;
        public Color Color
        {
            get => _color;
            set
            {
                goodColorControl1.Color = _color = _newColor = value;
                pnlOld.Invalidate();
                pnlNew.Invalidate();
            }
        }

        public bool EditAlpha
        {
            get { return chkAlpha.Visible; }
            set
            {
                if (chkAlpha.Visible = goodColorControl1.ShowAlpha = value)
                    this.Height = 287;
                else
                    this.Height = 287 - 20;

                pnlOld.Invalidate();
                pnlNew.Invalidate();
            }
        }

        private bool _showOld = false;
        public bool ShowOldColor
        {
            get => _showOld;
            set => lblOld.Visible = lblNew.Visible = pnlOld.Visible = _showOld = value;
        }

        public ColorControl() => InitializeComponent();

        public event EventHandler Closed;
        public event ColorChanged ColorChanged;

        public DialogResult DialogResult;

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            _color = _newColor;
            Closed?.Invoke(this, EventArgs.Empty);
        }

        private void pnlOld_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color c = _color;

            //Draw hatch
            if (chkAlpha.Checked && chkAlpha.Visible)
                g.FillRectangle(TransparentCheckerboardBrush, pnlOld.ClientRectangle);
            else
                c = Color.FromArgb(c.R, c.G, c.B);

            //Draw background
            using (Brush b = new SolidBrush(c))
                g.FillRectangle(b, pnlOld.ClientRectangle);
        }

        private void pnlNew_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color c = _newColor;

            //Draw hatch
            if (chkAlpha.Checked && chkAlpha.Visible)
                g.FillRectangle(TransparentCheckerboardBrush, pnlNew.ClientRectangle);
            else
                c = Color.FromArgb(c.R, c.G, c.B);

            //Draw background
            using (Brush b = new SolidBrush(c))
                g.FillRectangle(b, pnlNew.ClientRectangle);
        }

        private void goodColorControl1_ColorChanged(object sender, EventArgs e)
        {
            _newColor = goodColorControl1.Color;
            pnlNew.Invalidate();
            if (OnColorChanged != null)
                OnColorChanged(_newColor);
        }

        private void chkAlpha_CheckedChanged(object sender, EventArgs e)
        {
            pnlNew.Invalidate();
            pnlOld.Invalidate();
        }
    }

    internal class PropertyGridColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        IWindowsFormsEditorService _service = null;

        public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value)
        {
            if (provider != null)
                _service =
                    provider.GetService(
                    typeof(IWindowsFormsEditorService))
                    as IWindowsFormsEditorService;

            if (_service != null)
            {
                ColorControl selectionControl = new ColorControl();
                selectionControl.Closed += selectionControl_Closed;
                
                if (value is IColor)
                    selectionControl.Color = ((IColor)value).Color;

                _service.DropDownControl(selectionControl);

                if (selectionControl.DialogResult == DialogResult.OK)
                {
                    if (value is IColor)
                        ((IColor)value).Color = selectionControl.Color;
                }
                _service = null;
            }

            return value;
        }

        void selectionControl_Closed(object sender, EventArgs e)
        {
            if (_service != null)
                _service.CloseDropDown();
        }
    }
}
