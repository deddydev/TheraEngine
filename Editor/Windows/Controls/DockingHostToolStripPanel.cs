using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    /// <summary>
    /// Implements a ToolStripPanel capable of hosting and interacting with floating and docked ToolStrips.
    /// </summary>
    /// <remarks>
    /// To use, start with a form, say, your Main window.  Then, without adding a ToolStripContainer, add four
    /// DockingHostToolStripPanels to the form, one for the top, one for the bottom, one for the left, and
    /// one for the right of the window --- or as however your design dictates.  For each ToolStrip you add thusly
    /// to your application, you will need to then fill in the TopToolStripPanel, BottomToolStripPanel, 
    /// LeftToolStripPanel, and RightToolStripPanel of each FloatingDockingToolStrip you add to the form.  This
    /// is part of our functionality -- the ToolStrips that interact with these panels need to know about their
    /// existence.  Otherwise docking, floating, and tear-off functionality will be disabled.
    /// </remarks>
    public partial class DockingHostToolStripPanel : ToolStripPanel
    {
        // Internal member variables
        Rectangle _dockingRectangle; // the default rectangle for receiving a floating ToolStrip
        Form _mainForm; // the main window of the application

        /// <summary>
        /// Gets a reference to the effective rectangular area taken by this panel.
        /// </summary>
        /// <remarks>
        /// When a DockingHostToolStripPanel contains no docked ToolStrips, its height/width is zero, depending on
        /// the panel's orientation.  We maintain this property to provide the 'effective' rectangle of a panel,
        /// which is identical to the ClientRectangle property, except if a panel contains no toolbars, the dimension
        /// which would normally be zero in ClientRectangle is in fact set to be identical to the value of the
        /// DefaultToolStripInferiorDimension property.  In order for a floating FloatingDockingToolStrip to snap 
        /// to a DockingHostToolStripPanel, the user must drag the toolbar such that the mouse cursor is within the 
        /// DockingRectangle
        /// </remarks>
        [Browsable(false)]
        public Rectangle DockingRectangle => _dockingRectangle;

        /// <summary>
        /// Gets a reference to the list of docked ToolStrips maintained by this panel dynamically.
        /// </summary>
        [Browsable(false)]
        public List<ToolStrip> DockedToolStrips { get; }

        /// <summary>
        /// Gets or sets a reference to the default smaller dimension for ToolStrips hosted by this panel.
        /// </summary>
        /// <remarks>
        /// This dimension is the height of a horizontally-oriented ToolStrip or the
        /// width of a vertically-oriented ToolStrip.
        /// </remarks>
        [Browsable(true)]
        [Description("Specifies the default smaller dimension for ToolStrips hosted by this panel")]
        [DefaultValue(25)]
        public int DefaultToolStripInferiorDimension { get; set; }

        /// <summary>
        /// Constructs a new DockingHostToolStripPanel object.
        /// </summary>
        public DockingHostToolStripPanel() : base()
        {
            DefaultToolStripInferiorDimension = 25;
            DockedToolStrips = new List<ToolStrip>();
            _mainForm = null;

            InitializeComponent();

            ContextMenuStrip = new ContextMenuStrip();
            ContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);
        }

        void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            InitializeToolStripContextMenu();
        }
        private void ToolStrip_Toggle(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            ToolStrip toolStrip = (ToolStrip)menuItem.Tag;
            toolStrip.Visible = !toolStrip.Visible;
        }
        /// <summary>
        /// Refreshes the DockingRectangle property to contain the outline of the area 
        /// available to floating ToolStrips.
        /// </summary>
        /// <remarks>
        /// The docking rectangle is based off of the parameters that ClientRectangle has
        /// when one ToolStrip is docked.
        /// </remarks>
        void InitializeDockingReceptionRectangle()
        {
            _dockingRectangle = ClientRectangle;

            if (Orientation == Orientation.Horizontal && Height == 0)
                _dockingRectangle.Height = DefaultToolStripInferiorDimension;
            else if (Orientation == Orientation.Vertical && _dockingRectangle.Width == 0)
                _dockingRectangle.Width = DefaultToolStripInferiorDimension;
        }
        /// <summary>
        /// Returns a value that indicates whether the specified point is located within the dockable area 
        /// of this panel.
        /// </summary>
        /// <param name="point">Point to test, in screen coordinates.</param>
        /// <returns></returns>
        public bool IsPointWithinMe(Point point)
        {
            // Convert the DockingRectangle into screen coordinates
            Rectangle rectangle = RectangleToScreen(DockingRectangle);

            // Assuming point is in screen coordinates, such as mouse position
            // Say whether our DockingRectangle contains the point
            return rectangle.Contains(point);
        }
        /// <summary>
        /// Returns a value specifying whether the indicated ToolStrip is docked in this panel.
        /// </summary>
        /// <param name="toolStrip">ToolStrip about which you want to inquire</param>
        /// <returns></returns>
        public bool IsToolStripDocked(ToolStrip toolStrip)
        {
            if (toolStrip is null)
                throw new ArgumentException("ToolStrip provided to IsToolStripDocked was null.");

            return DockedToolStrips.Contains(toolStrip);
        }

        public void AcceptDroppedFloatingToolStrip(ToolStrip toolStrip)
        {
            // TODO: Add implementation code here
        }
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            if (e.Control is ToolStrip toolStrip)
            {
                toolStrip.GripStyle = ToolStripGripStyle.Visible; // only show grip in a docked ToolStrip
                DockedToolStrips.Add(e.Control as ToolStrip);

                InitializeToolStripContextMenu();
            }
        }
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            if (e.Control is ToolStrip toolStrip)
            {
                toolStrip.GripStyle = ToolStripGripStyle.Hidden; // only show grip in a docked ToolStrip
                DockedToolStrips.Remove(e.Control as ToolStrip);

                InitializeToolStripContextMenu();
            }
        }
        private void InitializeToolStripContextMenu()
        {
            ContextMenuStrip.Items.Clear();

            foreach (ToolStrip strip in DockedToolStrips)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem(
                    strip.Text,
                    null,
                    new EventHandler(ToolStrip_Toggle))
                {
                    Tag = strip,
                    Checked = strip.Visible
                };
                ContextMenuStrip.Items.Add(menuItem);
            }
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            InitializeDockingReceptionRectangle();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            InitializeDockingReceptionRectangle();
        }
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            InitializeDockingReceptionRectangle();
        }
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            InitializeDockingReceptionRectangle();
        }
    }
}
