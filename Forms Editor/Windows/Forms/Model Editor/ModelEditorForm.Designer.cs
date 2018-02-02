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
            this.BodyPanel.Size = new System.Drawing.Size(1378, 819);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(1378, 869);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(1378, 50);
            // 
            // FormTitle
            // 
            this.FormTitle.Size = new System.Drawing.Size(1185, 50);
            this.FormTitle.Text = "Model Editor";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(1378, 879);
            // 
            // DockPanel
            // 
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.Location = new System.Drawing.Point(0, 0);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.Size = new System.Drawing.Size(1378, 819);
            this.DockPanel.TabIndex = 15;
            // 
            // ModelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1388, 879);
            this.Location = new System.Drawing.Point(0, 0);
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