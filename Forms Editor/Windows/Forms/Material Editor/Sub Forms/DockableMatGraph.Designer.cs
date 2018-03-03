namespace TheraEditor.Windows.Forms
{
    partial class DockableMatGraph
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
            this.RenderPanel = new TheraEngine.MaterialGraphRenderPanel();
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
            this.RenderPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragDrop);
            this.RenderPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragEnter);
            this.RenderPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragOver);
            this.RenderPanel.DragLeave += new System.EventHandler(this.RenderPanel_DragLeave_1);
            // 
            // DockableMatGraph
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 332);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.RenderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DockableMatGraph";
            this.Text = "Material Graph";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragOver);
            this.DragLeave += new System.EventHandler(this.RenderPanel_DragLeave_1);
            this.ResumeLayout(false);

        }

        #endregion

        public TheraEngine.MaterialGraphRenderPanel RenderPanel;
    }
}