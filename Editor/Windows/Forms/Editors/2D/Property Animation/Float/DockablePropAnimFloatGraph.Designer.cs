namespace TheraEditor.Windows.Forms
{
    partial class DockablePropAnimFloatGraph
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockablePropAnimFloatGraph));
            this.RenderPanel = new TheraEditor.Windows.Forms.PropAnimFloatGraphRenderPanel();
            this.dockingHostToolStripPanel1 = new TheraEditor.Windows.Forms.DockingHostToolStripPanel();
            this.tearOffToolStrip1 = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.btnZoomExtents = new System.Windows.Forms.ToolStripButton();
            this.chkAutoTangents = new System.Windows.Forms.ToolStripButton();
            this.chkSnapToIncrement = new System.Windows.Forms.ToolStripButton();
            this.dockingHostToolStripPanel1.SuspendLayout();
            this.tearOffToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 25);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(359, 188);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.VsyncMode = TheraEngine.VSyncMode.Adaptive;
            // 
            // dockingHostToolStripPanel1
            // 
            this.dockingHostToolStripPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dockingHostToolStripPanel1.Controls.Add(this.tearOffToolStrip1);
            this.dockingHostToolStripPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dockingHostToolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockingHostToolStripPanel1.Name = "dockingHostToolStripPanel1";
            this.dockingHostToolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.dockingHostToolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dockingHostToolStripPanel1.Size = new System.Drawing.Size(359, 25);
            // 
            // tearOffToolStrip1
            // 
            this.tearOffToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tearOffToolStrip1.BottomToolStripPanel = null;
            this.tearOffToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.tearOffToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomExtents,
            this.chkAutoTangents});
            this.tearOffToolStrip1.LeftToolStripPanel = null;
            this.tearOffToolStrip1.Location = new System.Drawing.Point(3, 0);
            this.tearOffToolStrip1.Name = "tearOffToolStrip1";
            this.tearOffToolStrip1.RightToolStripPanel = null;
            this.tearOffToolStrip1.Size = new System.Drawing.Size(214, 25);
            this.tearOffToolStrip1.TabIndex = 2;
            this.tearOffToolStrip1.Text = "tearOffToolStrip1";
            this.tearOffToolStrip1.TopToolStripPanel = null;
            // 
            // btnZoomExtents
            // 
            this.btnZoomExtents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnZoomExtents.Image = ((System.Drawing.Image)(resources.GetObject("btnZoomExtents.Image")));
            this.btnZoomExtents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnZoomExtents.Name = "btnZoomExtents";
            this.btnZoomExtents.Size = new System.Drawing.Size(83, 22);
            this.btnZoomExtents.Text = "Zoom Extents";
            this.btnZoomExtents.Click += new System.EventHandler(this.btnZoomExtents_Click);
            // 
            // chkAutoTangents
            // 
            this.chkAutoTangents.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.chkAutoTangents.Image = ((System.Drawing.Image)(resources.GetObject("chkAutoTangents.Image")));
            this.chkAutoTangents.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkAutoTangents.Name = "chkAutoTangents";
            this.chkAutoTangents.Size = new System.Drawing.Size(88, 22);
            this.chkAutoTangents.Text = "Auto Tangents";
            this.chkAutoTangents.Click += new System.EventHandler(this.chkAutoTangents_Click);
            // 
            // chkSnapToIncrement
            // 
            this.chkSnapToIncrement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            //this.chkSnapToIncrement.Image = ((System.Drawing.Image)(resources.GetObject("chkAutoTangents.Image")));
            this.chkSnapToIncrement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkSnapToIncrement.Name = "chkSnapToIncrement";
            this.chkSnapToIncrement.Size = new System.Drawing.Size(88, 22);
            this.chkSnapToIncrement.Text = "Snap To Increment";
            this.chkSnapToIncrement.Click += new System.EventHandler(this.chkSnapToIncrement_Click);
            // 
            // DockablePropAnimFloatGraph
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 213);
            this.Controls.Add(this.RenderPanel);
            this.Controls.Add(this.dockingHostToolStripPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DockablePropAnimFloatGraph";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.Text = "Float Animation Editor";
            this.dockingHostToolStripPanel1.ResumeLayout(false);
            this.dockingHostToolStripPanel1.PerformLayout();
            this.tearOffToolStrip1.ResumeLayout(false);
            this.tearOffToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PropAnimFloatGraphRenderPanel RenderPanel;
        private DockingHostToolStripPanel dockingHostToolStripPanel1;
        private TearOffToolStrip tearOffToolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoomExtents;
        private System.Windows.Forms.ToolStripButton chkAutoTangents;
        private System.Windows.Forms.ToolStripButton chkSnapToIncrement;
    }
}