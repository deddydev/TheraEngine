namespace TheraEditor.Windows.Forms
{
    partial class DockableTexRefControl
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
            this.texRefControl1 = new TheraEditor.Windows.Forms.TexRefControl();
            this.SuspendLayout();
            // 
            // texRefControl1
            // 
            this.texRefControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.texRefControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.texRefControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.texRefControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.texRefControl1.Location = new System.Drawing.Point(0, 0);
            this.texRefControl1.Margin = new System.Windows.Forms.Padding(0);
            this.texRefControl1.Name = "texRefControl1";
            this.texRefControl1.Size = new System.Drawing.Size(728, 585);
            this.texRefControl1.TabIndex = 0;
            // 
            // DockableTexRefControl
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.texRefControl1);
            this.Name = "DockableTexRefControl";
            this.Text = "Texture Reference";
            this.ResumeLayout(false);

        }

        #endregion

        public TexRefControl texRefControl1;
    }
}