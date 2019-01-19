using BorderlessForm;
using Core.Win32.Native;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms
{
    public partial class TheraForm : BorderlessFormBase
    {
        public TheraForm()
        {
            InitializeComponent();
            InitBorderless();
        }
        
        private FormWindowState _previousWindowState;
        
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color HoverTextColor { get; set; } = Color.FromArgb(62, 109, 181);
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color DownTextColor { get; set; } = Color.FromArgb(25, 71, 138);
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color HoverBackColor { get; set; } = Color.FromArgb(213, 225, 242);
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color DownBackColor { get; set; } = Color.FromArgb(163, 189, 227);
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color NormalWindowBackColor { get; set; } = Color.Transparent;

        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color ActiveBorderColor { get; set; } = Editor.TurquoiseColor;
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color InactiveBorderColor { get; set; } = Color.FromArgb(131, 131, 131);
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color ActiveTextColor { get; set; } = Color.FromArgb(224, 224, 224);
        [Category("Appearance")]
        [Description("")]
        [Browsable(true), EditorBrowsable]
        public Color InactiveTextColor { get; set; } = Color.FromArgb(177, 177, 177);

        public enum MouseState
        {
            Normal,
            Hover,
            Down
        }

        protected virtual void SetLabelColors(Control control, MouseState state)
        {
            if (!ContainsFocus) return;

            var textColor = ActiveTextColor;
            var backColor = Editor.TurquoiseColor;

            switch (state)
            {
                //case MouseState.Normal:
                //    textColor = ActiveTextColor;
                //    backColor = Color.Transparent;
                //    //Cursor = Cursors.Default;
                //    break;
                case MouseState.Hover:
                    textColor = HoverTextColor;
                    backColor = HoverBackColor;
                    //Cursor = Cursors.Hand;
                    break;
                case MouseState.Down:
                    textColor = DownTextColor;
                    backColor = DownBackColor;
                    //Cursor = Cursors.Hand;
                    break;
            }

            control.ForeColor = textColor;
            control.BackColor = backColor;
        }
        private void InitBorderless()
        {
            foreach (var control in new[] { MinimizeLabel, MaximizeLabel, CloseLabel })
            {
                control.MouseEnter += (s, e) => SetLabelColors((Control)s, MouseState.Hover);
                control.MouseLeave += (s, e) => SetLabelColors((Control)s, MouseState.Normal);
                control.MouseDown += (s, e) => SetLabelColors((Control)s, MouseState.Down);
            }

            TopLeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOPLEFT);
            TopRightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOPRIGHT);
            BottomLeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOMLEFT);
            BottomRightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOMRIGHT);

            TopBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTTOP);
            LeftBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTLEFT);
            RightBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTRIGHT);
            BottomBorderPanel.MouseDown += (s, e) => DecorationMouseDown(e, HitTestValues.HTBOTTOM);
            
            FormTitle.MouseDown += TitleBar_MouseDown;
            FormTitle.MouseUp += (s, e) => { if (e.Button == MouseButtons.Right && FormTitle.ClientRectangle.Contains(e.Location)) ShowSystemMenu(MouseButtons); };
            FormTitle.Text = Text;
            TextChanged += (s, e) => FormTitle.Text = Text;

            var marlett = new Font("Marlett", 8.5f);

            MinimizeLabel.Font = marlett;
            MaximizeLabel.Font = marlett;
            CloseLabel.Font = marlett;
            
            MinimizeLabel.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) WindowState = FormWindowState.Minimized; };
            MaximizeLabel.MouseClick += (s, e) => { if (e.Button == MouseButtons.Left) ToggleMaximize(); };
            _previousWindowState = MinMaxState;
            SizeChanged += FormSizeChanged;
            CloseLabel.MouseClick += (s, e) => Close(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LogoPanel.Visible = ShowIcon;
            if (!MaximizeBox || !MinimizeBox)
            {
                if (MinimizeLabel.Visible = MaximizeLabel.Visible = MaximizeBox || MinimizeBox)
                {
                    MinimizeLabel.Enabled = MinimizeBox;
                    MaximizeLabel.Enabled = MaximizeBox;
                }
            }
        }

        protected virtual void Close(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) Close();
        }

        protected virtual void DecorationMouseDown(MouseEventArgs e, HitTestValues h)
        {
            if (e.Button == MouseButtons.Left) DecorationMouseDown(h);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            Engine.FocusChanged();
            SetBorderColor(InactiveBorderColor);
            SetTextColor(InactiveTextColor);
            base.OnDeactivate(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            Engine.FocusChanged();
            SetBorderColor(ActiveBorderColor);
            SetTextColor(ActiveTextColor);
            base.OnActivated(e);
        }
        
        protected virtual void SetBorderColor(Color color)
        {
            TopLeftBorderPanel.BackColor = color;
            TopBorderPanel.BackColor = color;
            TopRightBorderPanel.BackColor = color;
            LeftBorderPanel.BackColor = color;
            RightBorderPanel.BackColor = color;
            BottomLeftBorderPanel.BackColor = color;
            BottomBorderPanel.BackColor = color;
            BottomRightBorderPanel.BackColor = color;
            MinimizeLabel.BackColor = color;
            MaximizeLabel.BackColor = color;
            CloseLabel.BackColor = color;
        }

        protected virtual void SetTextColor(Color color)
        {
            //SystemLabel.ForeColor = color;
            FormTitle.ForeColor = color;
            MinimizeLabel.ForeColor = color;
            MaximizeLabel.ForeColor = color;
            CloseLabel.ForeColor = color;
        }

        protected virtual void FormSizeChanged(object sender, EventArgs e)
        {
            var maximized = MinMaxState == FormWindowState.Maximized;
            MaximizeLabel.Text = maximized ? "2" : "1";

            var panels = new[]
            {
                TopLeftBorderPanel, TopRightBorderPanel, BottomLeftBorderPanel, BottomRightBorderPanel,
                TopBorderPanel, LeftBorderPanel, RightBorderPanel, BottomBorderPanel
            };

            foreach (var panel in panels)
                panel.Visible = !maximized;

            _previousWindowState = MinMaxState;
        }

        public FormWindowState ToggleMaximize()
        {
            return WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }
        
        private DateTime _titleClickTime = DateTime.MinValue;
        private Point _titleClickPosition = Point.Empty;

        protected virtual void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var clickTime = (DateTime.Now - _titleClickTime).TotalMilliseconds;
                if (clickTime < SystemInformation.DoubleClickTime && e.Location == _titleClickPosition)
                    ToggleMaximize();
                else
                {
                    _titleClickTime = DateTime.Now;
                    _titleClickPosition = e.Location;
                    DecorationMouseDown(HitTestValues.HTCAPTION);
                }
            }
        }
        
        public class TheraToolstripRenderer : ToolStripProfessionalRenderer
        {
            public TheraToolstripRenderer() : base(new TheraColorTable())
            {
                RoundedEdges = false;
            }
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = e.Item.Selected ? SystemColors.HighlightText : Editor.TextColor;
                base.OnRenderItemText(e);
            }
        }
        public class TheraColorTable : ProfessionalColorTable
        {
            public override Color ButtonSelectedHighlight => ButtonSelectedGradientMiddle;
            public override Color ButtonSelectedHighlightBorder => ButtonSelectedBorder;
            public override Color ButtonPressedHighlight => ButtonPressedGradientMiddle;
            public override Color ButtonPressedHighlightBorder => ButtonPressedBorder;
            public override Color ButtonCheckedHighlight => ButtonCheckedGradientMiddle;
            public override Color ButtonCheckedHighlightBorder => ButtonSelectedBorder;
            public override Color ButtonPressedBorder => ButtonSelectedBorder;
            public override Color ButtonSelectedBorder => Color.FromArgb(255, 98, 98, 98);
            public override Color ButtonCheckedGradientBegin => Color.FromArgb(255, 144, 144, 144);
            public override Color ButtonCheckedGradientMiddle => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonCheckedGradientEnd => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonSelectedGradientBegin => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonSelectedGradientMiddle => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonSelectedGradientEnd => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonPressedGradientBegin => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonPressedGradientMiddle => Color.FromArgb(255, 170, 170, 170);
            public override Color ButtonPressedGradientEnd => Color.FromArgb(255, 170, 170, 170);
            public override Color CheckBackground => Color.FromArgb(255, 173, 173, 173);
            public override Color CheckSelectedBackground => Color.FromArgb(255, 173, 173, 173);
            public override Color CheckPressedBackground => Color.FromArgb(255, 140, 140, 140);
            public override Color GripDark => Color.FromArgb(22, 22, 22);
            public override Color GripLight => Color.FromArgb(83, 83, 83);
            public override Color ImageMarginGradientBegin => Editor.TitleBarColor;
            public override Color ImageMarginGradientMiddle => Editor.TitleBarColor;
            public override Color ImageMarginGradientEnd => Editor.TitleBarColor;
            public override Color ImageMarginRevealedGradientBegin => Color.FromArgb(255, 68, 68, 68);
            public override Color ImageMarginRevealedGradientMiddle => Color.FromArgb(255, 68, 68, 68);
            public override Color ImageMarginRevealedGradientEnd => Color.FromArgb(255, 68, 68, 68);
            public override Color MenuStripGradientBegin => Color.FromArgb(255, 138, 138, 138);
            public override Color MenuStripGradientEnd => Color.FromArgb(255, 103, 103, 103);
            public override Color MenuItemSelected => Editor.TurquoiseColor;
            public override Color MenuItemBorder => Editor.TurquoiseColor;
            public override Color MenuBorder => Color.Transparent;
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(255, 170, 170, 170);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(255, 170, 170, 170);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(255, 125, 125, 125);
            public override Color MenuItemPressedGradientMiddle => Color.FromArgb(255, 125, 125, 125);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(255, 125, 125, 125);
            public override Color RaftingContainerGradientBegin => Color.FromArgb(255, 170, 170, 170);
            public override Color RaftingContainerGradientEnd => Color.FromArgb(255, 170, 170, 170);
            public override Color SeparatorDark => Color.FromArgb(255, 22, 22, 22);
            public override Color SeparatorLight => Color.FromArgb(255, 62, 62, 62);
            public override Color StatusStripGradientBegin => Color.FromArgb(255, 112, 112, 112);
            public override Color StatusStripGradientEnd => Color.FromArgb(255, 97, 97, 97);
            public override Color ToolStripBorder => Color.FromArgb(255, 22, 22, 22);
            public override Color ToolStripDropDownBackground => Color.FromArgb(55, 55, 60);
            public override Color ToolStripGradientBegin => Color.FromName("DimGray");
            public override Color ToolStripGradientMiddle => Color.FromArgb(255, 89, 89, 89);
            public override Color ToolStripGradientEnd => Color.FromArgb(255, 88, 88, 88);
            public override Color ToolStripContentPanelGradientBegin => Color.FromArgb(255, 68, 68, 68);
            public override Color ToolStripContentPanelGradientEnd => Color.FromArgb(255, 68, 68, 68);
            public override Color ToolStripPanelGradientBegin => Color.FromArgb(255, 103, 103, 103);
            public override Color ToolStripPanelGradientEnd => Color.FromArgb(255, 103, 103, 103);
            public override Color OverflowButtonGradientBegin => Color.FromArgb(255, 103, 103, 103);
            public override Color OverflowButtonGradientMiddle => Color.FromArgb(255, 103, 103, 103);
            public override Color OverflowButtonGradientEnd => Color.FromArgb(255, 79, 79, 79);
        }
    }
}
