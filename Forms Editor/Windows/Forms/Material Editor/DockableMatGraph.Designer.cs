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
            // renderPanel1
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Name = "renderPanel1";
            this.RenderPanel.Size = new System.Drawing.Size(504, 408);
            this.RenderPanel.TabIndex = 1;
            this.RenderPanel.VsyncMode = TheraEngine.VSyncMode.Disabled;
            // 
            // DockableMatGraph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 408);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.RenderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "DockableMatGraph";
            this.Text = "Material Graph";
            this.ResumeLayout(false);

        }

        #endregion

        public TheraEngine.MaterialGraphRenderPanel RenderPanel;
    }
}