using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class TearOffToolStrip : ToolStrip
    {
        private bool _disableMouseCapture;

        [Browsable(false)]
        public bool IsFloating { get; private set; }

        [Browsable(false)]
        public DockingHostToolStripPanel LastDockedPanel { get; private set; }

        [Browsable(true)]
        [Description("Determines whether the control is visible or hidden.")]
        public new bool Visible
        {
            get => base.Visible;
            set
            {
                if (!IsFloating)
                    base.Visible = value;
                else
                {
                    FloatingForm form = (FloatingForm)Parent;
                    if (form != null)
                    {
                        if (value == true) form.Show();
                        else if (value == false) form.Hide();
                    }
                }
            }
        }

        [Browsable(true)]
        [Description("Reference to the DockingHostToolStripPanel on left of the main form")]
        public DockingHostToolStripPanel TopToolStripPanel { get; set; }

        [Browsable(true)]
        [Description("Reference to the DockingHostToolStripPanel on bottom of the main form")]
        public DockingHostToolStripPanel BottomToolStripPanel { get; set; }

        [Browsable(true)]
        [Description("Reference to the DockingHostToolStripPanel on left of the main form")]
        public DockingHostToolStripPanel LeftToolStripPanel { get; set; }

        [Browsable(true)]
        [Description("Reference to the DockingHostToolStripPanel on right of the main form")]
        public DockingHostToolStripPanel RightToolStripPanel { get; set; }

        public TearOffToolStrip()
        {
            TopToolStripPanel = null;
            BottomToolStripPanel = null;
            LeftToolStripPanel = null;
            RightToolStripPanel = null;

            IsFloating = false;
            _disableMouseCapture = false;

            LastDockedPanel = null;

            InitializeComponent();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);            
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (!IsFloating)
                MakeFloating();
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);

            if (_disableMouseCapture == true)
            {
                _disableMouseCapture = false;
                return;
            }

            if (DesignMode == true)
                return;

            if (TopToolStripPanel is null
                && BottomToolStripPanel is null
                && LeftToolStripPanel is null
                && RightToolStripPanel is null)
                return;

            if (!TopToolStripPanel.IsPointWithinMe(MousePosition)
                && !RightToolStripPanel.IsPointWithinMe(MousePosition)
                && !BottomToolStripPanel.IsPointWithinMe(MousePosition)
                && !LeftToolStripPanel.IsPointWithinMe(MousePosition))
            {
                MakeFloating();
            }
            else if (
                TopToolStripPanel.IsPointWithinMe(MousePosition)
                || BottomToolStripPanel.IsPointWithinMe(MousePosition)
                || LeftToolStripPanel.IsPointWithinMe(MousePosition)
                || RightToolStripPanel.IsPointWithinMe(MousePosition)
                )
            {
                if (TopToolStripPanel.IsPointWithinMe(MousePosition))
                    DockTo(TopToolStripPanel);
                else if (BottomToolStripPanel.IsPointWithinMe(MousePosition))
                    DockTo(BottomToolStripPanel);
                else if (LeftToolStripPanel.IsPointWithinMe(MousePosition))
                    DockTo(LeftToolStripPanel);
                else if (RightToolStripPanel.IsPointWithinMe(MousePosition))
                    DockTo(RightToolStripPanel);
            }
        }

        public void MakeFloating()
        {
            if (!(Parent is DockingHostToolStripPanel parentPanel))
                return;

            _disableMouseCapture = true;

            // set the flag which says we are floating
            IsFloating = true;

            Orientation currentOrientation = Orientation;

            FloatingForm floatingForm = new FloatingForm
            {
                Text = Text,
                Bounds = new Rectangle(MousePosition, Size),
                Location = new Point(MousePosition.X + 25, MousePosition.Y + 25)
            };

            // ok, remove ourselves from our parent panel and dock to the form
            LastDockedPanel = parentPanel;
            parentPanel.Controls.Remove(this);
            floatingForm.Controls.Add(this);

            // change our properties
            Stretch = true;
            Dock = DockStyle.Fill;
            GripStyle = ToolStripGripStyle.Hidden;

            // and set up events
            floatingForm.MouseCaptureChanged += new EventHandler(FloatingForm_Dropped);
            floatingForm.DockingRequested += new EventHandler(FloatingForm_DockingRequested);

            // finally show the floating toolbar
            floatingForm.Show(parentPanel.Parent);
        }

        void FloatingForm_DockingRequested(object sender, EventArgs e)
        {
            // always dock a double-clicked toolbar to the panel
            // from whence it came, or else the top tool strip panel by default
            if (LastDockedPanel is null)
                LastDockedPanel = TopToolStripPanel;

            DockTo(LastDockedPanel);
        }

        void FloatingForm_Dropped(object sender, EventArgs e)
        {
            OnMouseCaptureChanged(e);
        }

        public void DockTo(DockingHostToolStripPanel panel)
        {
            if (!IsFloating)
                return;

            // if NULL is specified for the panel to which to dock
            // of if our Parent is a ToolStripPanel (i.e., we're already docked)
            // stop and return to the caller
            if ((panel is null)
                || (Parent is ToolStripPanel))
                return;

            IsFloating = false;
            FloatingForm floatingForm = (FloatingForm)Parent;

            floatingForm.Hide();

            Stretch = false; GripStyle = ToolStripGripStyle.Hidden;
            floatingForm.Controls.Remove(this);
            panel.Controls.Add(this);

            //LayoutStyle = ToolStripLayoutStyle.StackWithOverflow;
            //AutoSize = true;

            floatingForm.Close();
        }

        private void TearOffToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            // this is done to prevent the action of the user clicking a button triggering a tearoff
            _disableMouseCapture = true;
        }
    }
}
