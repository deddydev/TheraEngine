namespace TheraEditor.Windows.Forms
{
    partial class IssueDialog
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
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Size = new System.Drawing.Size(679, 478);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(679, 518);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(679, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Size = new System.Drawing.Size(508, 40);
            this.FormTitle.Text = "IssueDialog";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(679, 526);
            // 
            // IssueDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 526);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "IssueDialog";
            this.Text = "IssueDialog";
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}