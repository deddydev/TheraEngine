namespace TheraEditor.Windows.Forms
{
    partial class DockableScriptEditor
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
            this.TextBox = new ScintillaNET.Scintilla();
            this.SuspendLayout();
            // 
            // TextBox
            // 
            this.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.IdleStyling = ScintillaNET.IdleStyling.All;
            this.TextBox.Lexer = ScintillaNET.Lexer.Python;
            this.TextBox.Location = new System.Drawing.Point(0, 0);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(728, 585);
            this.TextBox.TabIndex = 0;
            this.TextBox.Text = "hello";
            this.TextBox.UseTabs = true;
            // 
            // DockableScriptEditor
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.TextBox);
            this.Name = "DockableScriptEditor";
            this.Text = "Script Editor";
            this.ResumeLayout(false);

        }

        #endregion

        public ScintillaNET.Scintilla TextBox;
    }
}