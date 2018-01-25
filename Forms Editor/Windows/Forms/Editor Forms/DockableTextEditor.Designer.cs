namespace TheraEditor.Windows.Forms
{
    partial class DockableTextEditor
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.cboMode = new System.Windows.Forms.ToolStripComboBox();
            this.TextBox = new TheraEditor.Core.SyntaxHighlightingTextBox.SyntaxHighlightingTextBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cboMode});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(728, 33);
            this.toolStrip1.TabIndex = 0;
            // 
            // cboMode
            // 
            this.cboMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMode.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cboMode.Name = "cboMode";
            this.cboMode.Size = new System.Drawing.Size(121, 33);
            this.cboMode.SelectedIndexChanged += new System.EventHandler(this.cboMode_SelectedIndexChanged);
            // 
            // TextBox
            // 
            this.TextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.TextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TextBox.CaseSensitive = false;
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.FilterAutoComplete = false;
            this.TextBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.TextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.TextBox.Location = new System.Drawing.Point(0, 33);
            this.TextBox.MaxUndoRedoSteps = 0;
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(728, 552);
            this.TextBox.TabIndex = 0;
            this.TextBox.Text = "";
            // 
            // DockableTextEditor
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DockableTextEditor";
            this.Text = "Text Editor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox cboMode;
        public Core.SyntaxHighlightingTextBox.SyntaxHighlightingTextBox TextBox;
    }
}