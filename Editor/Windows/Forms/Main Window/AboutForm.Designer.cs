namespace TheraEditor.Windows.Forms
{
    partial class AboutForm
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
            this.BodyPanel.Size = new System.Drawing.Size(581, 313);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(581, 353);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(581, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Size = new System.Drawing.Size(410, 40);
            this.FormTitle.Text = "AboutForm";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(581, 361);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 361);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "AboutForm";
            this.Text = "AboutForm";
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}