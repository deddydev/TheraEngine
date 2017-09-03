using BorderlessForm;
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
        public Color ActiveBorderColor { get; set; } = Color.FromArgb(150, 192, 192);
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
            var backColor = Color.FromArgb(150, 192, 192);

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
            
            FormTitle.MouseDown += TitleLabel_MouseDown;
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
            Program.FocusChanged();
            SetBorderColor(InactiveBorderColor);
            SetTextColor(InactiveTextColor);
            base.OnDeactivate(e);
        }
        protected override void OnActivated(EventArgs e)
        {
            Program.FocusChanged();
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
        
        private DateTime titleClickTime = DateTime.MinValue;
        private Point titleClickPosition = Point.Empty;

        protected virtual void TitleLabel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var clickTime = (DateTime.Now - titleClickTime).TotalMilliseconds;
                if (clickTime < SystemInformation.DoubleClickTime && e.Location == titleClickPosition)
                    ToggleMaximize();
                else
                {
                    titleClickTime = DateTime.Now;
                    titleClickPosition = e.Location;
                    DecorationMouseDown(HitTestValues.HTCAPTION);
                }
            }
        }
        
        protected class TheraToolstripRenderer : ToolStripProfessionalRenderer
        {
            public TheraToolstripRenderer() : base(new TheraColorTable())
            {
                RoundedEdges = false;
            }
            protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
            {
                e.TextColor = Color.FromArgb(224, 224, 224);
                base.OnRenderItemText(e);
            }
        }
        public class TheraColorTable : ProfessionalColorTable
        {
            public override Color ButtonSelectedHighlight
            {
                get { return ButtonSelectedGradientMiddle; }
            }
            public override Color ButtonSelectedHighlightBorder
            {
                get { return ButtonSelectedBorder; }
            }
            public override Color ButtonPressedHighlight
            {
                get { return ButtonPressedGradientMiddle; }
            }
            public override Color ButtonPressedHighlightBorder
            {
                get { return ButtonPressedBorder; }
            }
            public override Color ButtonCheckedHighlight
            {
                get { return ButtonCheckedGradientMiddle; }
            }
            public override Color ButtonCheckedHighlightBorder
            {
                get { return ButtonSelectedBorder; }
            }
            public override Color ButtonPressedBorder
            {
                get { return ButtonSelectedBorder; }
            }
            public override Color ButtonSelectedBorder
            {
                get { return Color.FromArgb(255, 98, 98, 98); }
            }
            public override Color ButtonCheckedGradientBegin
            {
                get { return Color.FromArgb(255, 144, 144, 144); }
            }
            public override Color ButtonCheckedGradientMiddle
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonCheckedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonSelectedGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonSelectedGradientMiddle
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonSelectedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonPressedGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonPressedGradientMiddle
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color ButtonPressedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color CheckBackground
            {
                get { return Color.FromArgb(255, 173, 173, 173); }
            }
            public override Color CheckSelectedBackground
            {
                get { return Color.FromArgb(255, 173, 173, 173); }
            }
            public override Color CheckPressedBackground
            {
                get { return Color.FromArgb(255, 140, 140, 140); }
            }
            public override Color GripDark
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color GripLight
            {
                get { return Color.FromArgb(255, 83, 83, 83); }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return Color.FromArgb(255, 85, 85, 85); }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginRevealedGradientBegin
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginRevealedGradientMiddle
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ImageMarginRevealedGradientEnd
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color MenuStripGradientBegin
            {
                get { return Color.FromArgb(255, 138, 138, 138); }
            }
            public override Color MenuStripGradientEnd
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuItemBorder
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuBorder
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color MenuItemPressedGradientMiddle
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color RaftingContainerGradientBegin
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color RaftingContainerGradientEnd
            {
                get { return Color.FromArgb(255, 170, 170, 170); }
            }
            public override Color SeparatorDark
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color SeparatorLight
            {
                get { return Color.FromArgb(255, 62, 62, 62); }
            }
            public override Color StatusStripGradientBegin
            {
                get { return Color.FromArgb(255, 112, 112, 112); }
            }
            public override Color StatusStripGradientEnd
            {
                get { return Color.FromArgb(255, 97, 97, 97); }
            }
            public override Color ToolStripBorder
            {
                get { return Color.FromArgb(255, 22, 22, 22); }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return Color.FromArgb(255, 125, 125, 125); }
            }
            public override Color ToolStripGradientBegin
            {
                get { return Color.FromName("DimGray"); }
            }
            public override Color ToolStripGradientMiddle
            {
                get { return Color.FromArgb(255, 89, 89, 89); }
            }
            public override Color ToolStripGradientEnd
            {
                get { return Color.FromArgb(255, 88, 88, 88); }
            }
            public override Color ToolStripContentPanelGradientBegin
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ToolStripContentPanelGradientEnd
            {
                get { return Color.FromArgb(255, 68, 68, 68); }
            }
            public override Color ToolStripPanelGradientBegin
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color ToolStripPanelGradientEnd
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color OverflowButtonGradientBegin
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color OverflowButtonGradientMiddle
            {
                get { return Color.FromArgb(255, 103, 103, 103); }
            }
            public override Color OverflowButtonGradientEnd
            {
                get { return Color.FromArgb(255, 79, 79, 79); }
            }
        }
    }
}
