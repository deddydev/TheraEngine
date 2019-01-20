using TheraEngine.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    partial class DockableModelEditorRenderForm
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
            this.RenderPanel = new ModelEditorRenderPanel();
            this.SuspendLayout();
            // 
            // RenderPanel
            // 
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RenderPanel.Location = new System.Drawing.Point(0, 0);
            this.RenderPanel.Name = "RenderPanel";
            this.RenderPanel.Size = new System.Drawing.Size(740, 573);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.VsyncMode = TheraEngine.VSyncMode.Disabled;
            //this.RenderPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragDrop);
            //this.RenderPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragEnter);
            //this.RenderPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.RenderPanel_DragOver);
            //this.RenderPanel.DragLeave += new System.EventHandler(this.RenderPanel_DragLeave);
            // 
            // DockableRenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 573);
            this.Controls.Add(this.RenderPanel);
            this.Name = "DockableRenderForm";
            this.Text = "DockableRenderForm";
            this.ResumeLayout(false);

        }

        #endregion

        public ModelEditorRenderPanel RenderPanel;
    }
}