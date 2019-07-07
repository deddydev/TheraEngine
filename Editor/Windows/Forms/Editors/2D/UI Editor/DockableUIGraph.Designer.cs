namespace TheraEditor.Windows.Forms
{
    partial class DockableUIGraph
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableUIGraph));
            this.RenderPanel = new TheraEditor.Windows.Forms.UIGraphRenderPanel();
            this.dockingHostToolStripPanel2 = new TheraEditor.Windows.Forms.DockingHostToolStripPanel();
            this.tearOffToolStrip1 = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.btnZoomExtents = new System.Windows.Forms.ToolStripButton();
            this.dockingHostToolStripPanel2.SuspendLayout();
            this.tearOffToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.RenderPanel.AllowDrop = true;
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 25);
            this.RenderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(328, 184);
            this.RenderPanel.TabIndex = 1;
            this.RenderPanel.VsyncMode = TheraEngine.EVSyncMode.Adaptive;
            // 
            // dockingHostToolStripPanel2
            // 
            this.dockingHostToolStripPanel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dockingHostToolStripPanel2.Controls.Add(this.tearOffToolStrip1);
            this.dockingHostToolStripPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.dockingHostToolStripPanel2.Location = new System.Drawing.Point(0, 0);
            this.dockingHostToolStripPanel2.Name = "dockingHostToolStripPanel2";
            this.dockingHostToolStripPanel2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.dockingHostToolStripPanel2.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dockingHostToolStripPanel2.Size = new System.Drawing.Size(328, 25);
            // 
            // tearOffToolStrip1
            // 
            this.tearOffToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tearOffToolStrip1.BottomToolStripPanel = null;
            this.tearOffToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.tearOffToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomExtents});
            this.tearOffToolStrip1.LeftToolStripPanel = null;
            this.tearOffToolStrip1.Location = new System.Drawing.Point(3, 0);
            this.tearOffToolStrip1.Name = "tearOffToolStrip1";
            this.tearOffToolStrip1.RightToolStripPanel = null;
            this.tearOffToolStrip1.Size = new System.Drawing.Size(95, 25);
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
            // DockableUIGraph
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 209);
            this.Controls.Add(this.RenderPanel);
            this.Controls.Add(this.dockingHostToolStripPanel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DockableUIGraph";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.Text = "UI View";
            this.dockingHostToolStripPanel2.ResumeLayout(false);
            this.dockingHostToolStripPanel2.PerformLayout();
            this.tearOffToolStrip1.ResumeLayout(false);
            this.tearOffToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public UIGraphRenderPanel RenderPanel;
        private DockingHostToolStripPanel dockingHostToolStripPanel2;
        private TearOffToolStrip tearOffToolStrip1;
        private System.Windows.Forms.ToolStripButton btnZoomExtents;
    }
}