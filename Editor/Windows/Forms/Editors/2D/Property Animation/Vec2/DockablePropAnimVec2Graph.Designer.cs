namespace TheraEditor.Windows.Forms
{
    partial class DockablePropAnimVec2Graph
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
            this.RenderPanel = new TheraEditor.Windows.Forms.PropAnimVec2GraphRenderPanel();
            this.dockingHostToolStripPanel1 = new TheraEditor.Windows.Forms.DockingHostToolStripPanel();
            this.tsPropAnimFloat = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClose = new System.Windows.Forms.ToolStripMenuItem();
            this.btnView = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnZoomExtents = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.chkAutoTangents = new System.Windows.Forms.ToolStripMenuItem();
            this.chkSnapToUnits = new System.Windows.Forms.ToolStripMenuItem();
            this.dockingHostToolStripPanel1.SuspendLayout();
            this.tsPropAnimFloat.SuspendLayout();
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 25);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(667, 383);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.VsyncMode = TheraEngine.EVSyncMode.Adaptive;
            // 
            // dockingHostToolStripPanel1
            // 
            this.dockingHostToolStripPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dockingHostToolStripPanel1.Controls.Add(this.tsPropAnimFloat);
            this.dockingHostToolStripPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dockingHostToolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockingHostToolStripPanel1.Name = "dockingHostToolStripPanel1";
            this.dockingHostToolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.dockingHostToolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dockingHostToolStripPanel1.Size = new System.Drawing.Size(667, 25);
            // 
            // tsPropAnimFloat
            // 
            this.tsPropAnimFloat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tsPropAnimFloat.BottomToolStripPanel = null;
            this.tsPropAnimFloat.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPropAnimFloat.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.btnView,
            this.btnFile});
            this.tsPropAnimFloat.LeftToolStripPanel = null;
            this.tsPropAnimFloat.Location = new System.Drawing.Point(3, 0);
            this.tsPropAnimFloat.Name = "tsPropAnimFloat";
            this.tsPropAnimFloat.RightToolStripPanel = null;
            this.tsPropAnimFloat.Size = new System.Drawing.Size(188, 25);
            this.tsPropAnimFloat.TabIndex = 2;
            this.tsPropAnimFloat.Text = "Float Property Animation Toolstrip";
            this.tsPropAnimFloat.TopToolStripPanel = null;
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSave,
            this.btnSaveAs,
            this.btnClose});
            this.toolStripDropDownButton2.ForeColor = System.Drawing.Color.White;
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(38, 22);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(114, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(114, 22);
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.Click += new System.EventHandler(this.BtnSaveAs_Click);
            // 
            // btnClose
            // 
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(114, 22);
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnView
            // 
            this.btnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomExtents});
            this.btnView.ForeColor = System.Drawing.Color.White;
            this.btnView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(45, 22);
            this.btnView.Text = "View";
            // 
            // btnZoomExtents
            // 
            this.btnZoomExtents.Name = "btnZoomExtents";
            this.btnZoomExtents.Size = new System.Drawing.Size(180, 22);
            this.btnZoomExtents.Text = "Zoom Extents";
            this.btnZoomExtents.Click += new System.EventHandler(this.btnZoomExtents_Click);
            // 
            // btnFile
            // 
            this.btnFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkAutoTangents,
            this.chkSnapToUnits});
            this.btnFile.ForeColor = System.Drawing.Color.White;
            this.btnFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(62, 22);
            this.btnFile.Text = "Options";
            // 
            // chkAutoTangents
            // 
            this.chkAutoTangents.Name = "chkAutoTangents";
            this.chkAutoTangents.Size = new System.Drawing.Size(151, 22);
            this.chkAutoTangents.Text = "Auto Tangents";
            this.chkAutoTangents.Click += new System.EventHandler(this.chkAutoTangents_Click);
            // 
            // chkSnapToUnits
            // 
            this.chkSnapToUnits.Name = "chkSnapToUnits";
            this.chkSnapToUnits.Size = new System.Drawing.Size(151, 22);
            this.chkSnapToUnits.Text = "Snap To Units";
            this.chkSnapToUnits.Click += new System.EventHandler(this.chkSnapToUnits_Click);
            // 
            // DockablePropAnimFloatGraph
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(667, 408);
            this.Controls.Add(this.RenderPanel);
            this.Controls.Add(this.dockingHostToolStripPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DockablePropAnimFloatGraph";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.Text = "Float Animation Editor";
            this.dockingHostToolStripPanel1.ResumeLayout(false);
            this.dockingHostToolStripPanel1.PerformLayout();
            this.tsPropAnimFloat.ResumeLayout(false);
            this.tsPropAnimFloat.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PropAnimVec2GraphRenderPanel RenderPanel;
        private DockingHostToolStripPanel dockingHostToolStripPanel1;
        private TearOffToolStrip tsPropAnimFloat;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.ToolStripMenuItem btnSaveAs;
        private System.Windows.Forms.ToolStripMenuItem btnClose;
        private System.Windows.Forms.ToolStripDropDownButton btnView;
        private System.Windows.Forms.ToolStripMenuItem btnZoomExtents;
        private System.Windows.Forms.ToolStripDropDownButton btnFile;
        private System.Windows.Forms.ToolStripMenuItem chkAutoTangents;
        private System.Windows.Forms.ToolStripMenuItem chkSnapToUnits;
    }
}