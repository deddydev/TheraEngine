using TheraEngine.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    partial class ModelEditorForm
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
            this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.DockPanel);
            this.BodyPanel.Size = new System.Drawing.Size(1226, 655);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(1226, 695);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(1226, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.FormTitle.Size = new System.Drawing.Size(1055, 40);
            this.FormTitle.Text = "Model Editor";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(1226, 703);
            // 
            // DockPanel
            // 
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DockPanel.Location = new System.Drawing.Point(0, 0);
            this.DockPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.Size = new System.Drawing.Size(1226, 655);
            this.DockPanel.TabIndex = 15;
            // 
            // ModelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 703);
            this.Location = new System.Drawing.Point(0, 0);
            this.MinimumSize = new System.Drawing.Size(320, 55);
            this.Name = "ModelEditorForm";
            this.Text = "Model Editor";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public DockPanel DockPanel;
    }
}