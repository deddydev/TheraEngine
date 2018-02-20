using TheraEngine.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    partial class DockableWorldRenderForm
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
            this.RenderPanel = new TheraEngine.Windows.Forms.EditorWorldRenderPanel();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.RenderPanel);
            this.BodyPanel.Size = new System.Drawing.Size(650, 410);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(650, 450);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(650, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.FormTitle.Size = new System.Drawing.Size(479, 40);
            this.FormTitle.Text = "DockableRenderForm";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(650, 458);
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(650, 410);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.VsyncMode = TheraEngine.VSyncMode.Disabled;
            this.RenderPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragDrop);
            this.RenderPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragEnter);
            this.RenderPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragOver);
            this.RenderPanel.DragLeave += new System.EventHandler(this.RenderPanel_DragLeave);
            // 
            // DockableRenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 458);
            this.Location = new System.Drawing.Point(0, 0);
            this.MinimumSize = new System.Drawing.Size(320, 55);
            this.Name = "DockableRenderForm";
            this.Text = "DockableRenderForm";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public EditorWorldRenderPanel RenderPanel;
    }
}