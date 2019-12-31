using TheraEngine;

namespace TheraEditor.Windows.Forms
{
    partial class DockablePropAnimIntGraph
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
            this.dockingHostToolStripPanel1 = new TheraEditor.Windows.Forms.DockingHostToolStripPanel();
            this.tsPropAnimInt = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.btnZoomExtents = new System.Windows.Forms.ToolStripButton();
            this.chkAutoTangents = new System.Windows.Forms.ToolStripButton();
            this.dockingHostToolStripPanel1.SuspendLayout();
            this.tsPropAnimInt.SuspendLayout();
            this.SuspendLayout();
            // 
            // dockingHostToolStripPanel1
            // 
            this.dockingHostToolStripPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dockingHostToolStripPanel1.Controls.Add(this.tsPropAnimInt);
            this.dockingHostToolStripPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dockingHostToolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockingHostToolStripPanel1.Name = "dockingHostToolStripPanel1";
            this.dockingHostToolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.dockingHostToolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dockingHostToolStripPanel1.Size = new System.Drawing.Size(359, 25);
            // 
            // tearOffToolStrip1
            // 
            this.tsPropAnimInt.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tsPropAnimInt.BottomToolStripPanel = null;
            this.tsPropAnimInt.Dock = System.Windows.Forms.DockStyle.None;
            this.tsPropAnimInt.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomExtents,
            this.chkAutoTangents});
            this.tsPropAnimInt.LeftToolStripPanel = null;
            this.tsPropAnimInt.Location = new System.Drawing.Point(3, 0);
            this.tsPropAnimInt.Name = "tearOffToolStrip1";
            this.tsPropAnimInt.RightToolStripPanel = null;
            this.tsPropAnimInt.Size = new System.Drawing.Size(214, 25);
            this.tsPropAnimInt.TabIndex = 2;
            this.tsPropAnimInt.Text = "tearOffToolStrip1";
            this.tsPropAnimInt.TopToolStripPanel = null;
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
            this.chkSnapToUnits.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            //this.chkSnapToIncrement.Image = ((System.Drawing.Image)(resources.GetObject("chkAutoTangents.Image")));
            this.chkSnapToUnits.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkSnapToUnits.Name = "chkSnapToIncrement";
            this.chkSnapToUnits.Size = new System.Drawing.Size(88, 22);
            this.chkSnapToUnits.Text = "Snap To Increment";
            this.chkSnapToUnits.Click += new System.EventHandler(this.chkSnapToUnits_Click);
            // 
            // DockablePropAnimFloatGraph
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 213);
            this.Controls.Add(this.dockingHostToolStripPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DockablePropAnimFloatGraph";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.Text = "Float Animation Editor";
            this.dockingHostToolStripPanel1.ResumeLayout(false);
            this.dockingHostToolStripPanel1.PerformLayout();
            this.tsPropAnimInt.ResumeLayout(false);
            this.tsPropAnimInt.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DockingHostToolStripPanel dockingHostToolStripPanel1;
        private TearOffToolStrip tsPropAnimInt;
        private System.Windows.Forms.ToolStripButton btnZoomExtents;
        private System.Windows.Forms.ToolStripButton chkAutoTangents;
        private System.Windows.Forms.ToolStripButton chkSnapToUnits;
    }
}