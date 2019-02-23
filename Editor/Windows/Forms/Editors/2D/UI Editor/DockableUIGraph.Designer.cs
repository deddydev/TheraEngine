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
            this.RenderPanel = new TheraEditor.Windows.Forms.UIGraphRenderPanel();
            this.dockingHostToolStripPanel1 = new TheraEditor.Windows.Forms.DockingHostToolStripPanel();
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.RenderPanel.AllowDrop = true;
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(378, 332);
            this.RenderPanel.TabIndex = 1;
            this.RenderPanel.VsyncMode = TheraEngine.VSyncMode.Adaptive;
            // 
            // dockingHostToolStripPanel1
            // 
            this.dockingHostToolStripPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dockingHostToolStripPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockingHostToolStripPanel1.Name = "dockingHostToolStripPanel1";
            this.dockingHostToolStripPanel1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.dockingHostToolStripPanel1.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.dockingHostToolStripPanel1.Size = new System.Drawing.Size(378, 0);
            // 
            // DockableUIGraph
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 332);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.dockingHostToolStripPanel1);
            this.Controls.Add(this.RenderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DockableUIGraph";
            this.Text = "UI View";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public UIGraphRenderPanel RenderPanel;
        private DockingHostToolStripPanel dockingHostToolStripPanel1;
    }
}